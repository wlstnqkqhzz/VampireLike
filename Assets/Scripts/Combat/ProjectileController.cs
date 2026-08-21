using UnityEngine;
using System.Collections.Generic;
using VampireLike.VFX;

namespace VampireLike.Combat
{
    /// <summary>
    /// 발사된 투사체의 직선 이동, 수명, 적 충돌 피해, 관통 처리를 담당한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileController : MonoBehaviour
    {
        // 투사체 이동 속도다.
        [SerializeField]
        private float moveSpeed = 8f;

        // 기본 피해량이다. 발사 시 공격력 배율을 곱해 실제 피해량을 계산한다.
        [SerializeField]
        private float damage = 1f;

        // 충돌하지 않아도 자동 제거되는 시간이다.
        [SerializeField]
        private float lifeTime = 3f;

        // 같은 공격 묶음에서 한 적에게 여러 투사체가 동시에 맞을 때 후속 피해에 적용할 배율이다.
        [SerializeField]
        private float repeatedSameAttackDamageMultiplier = 0.35f;

        // 공격 묶음별 중복 타격 기록을 정리하기 전까지 유지하는 시간이다.
        [SerializeField]
        private float repeatedSameAttackHistorySeconds = 6f;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D circleCollider;
        private Vector2 moveDirection = Vector2.right;
        private float lifeTimer;
        private float effectiveDamage;
        private int remainingPierceCount;
        private int remainingReflectCount;
        private PlayerSpecialUpgradeController specialUpgradeController;
        private CombatVFXKind vfxKind = CombatVFXKind.ArcaneImpact;
        private bool isDestroying;
        private int attackGroupId;
        private readonly HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();
        private const int ProjectileSortingOrder = 1800;
        private static int nextAttackGroupId = 1;
        private static readonly Dictionary<int, Dictionary<int, int>> attackGroupHitCounts = new Dictionary<int, Dictionary<int, int>>();
        private static readonly Dictionary<int, float> attackGroupLastUsedTimes = new Dictionary<int, float>();
        private static readonly List<int> expiredAttackGroupIds = new List<int>();

        private void Awake()
        {
            // 투사체는 중력 없이 회전 고정 상태로 물리 이동한다.
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = ProjectileSortingOrder;

            circleCollider = projectileCollider as CircleCollider2D;
            effectiveDamage = damage;
        }

        private void FixedUpdate()
        {
            if (GameState.IsGameOver)
                return;

            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime)
                DestroyProjectile(true, 0.28f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 적과 충돌했을 때만 피해를 주고, 같은 적을 중복 타격하지 않게 막는다.
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth == null)
                return;

            if (hitEnemies.Contains(enemyHealth))
                return;

            hitEnemies.Add(enemyHealth);
            float appliedDamage = GetDamageForEnemy(enemyHealth);
            enemyHealth.TakeDamage(appliedDamage);
            CombatVFX.PlayBurst(transform.position, vfxKind, 0.42f, 0.18f);
            specialUpgradeController?.HandleProjectileHit(enemyHealth, appliedDamage, transform.position);

            if (enemyHealth.IsDead)
                specialUpgradeController?.HandleProjectileKill(enemyHealth, appliedDamage, transform.position);

            if (remainingPierceCount > 0)
            {
                remainingPierceCount--;
                return;
            }

            if (TryReflectProjectile())
                return;

            DestroyProjectile(false, 0f);
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            damage = Mathf.Max(0.1f, damage);
            lifeTime = Mathf.Max(0.1f, lifeTime);
            repeatedSameAttackDamageMultiplier = Mathf.Clamp(repeatedSameAttackDamageMultiplier, 0.05f, 1f);
            repeatedSameAttackHistorySeconds = Mathf.Max(0.5f, repeatedSameAttackHistorySeconds);
        }

        public static int CreateAttackGroupId()
        {
            if (nextAttackGroupId == int.MaxValue)
                nextAttackGroupId = 1;

            return nextAttackGroupId++;
        }

        public void Launch(Vector2 direction)
        {
            Launch(direction, 1f, 0);
        }

        /// <summary>
        /// 발사 순간의 방향, 공격력 배율, 관통 횟수를 설정한다.
        /// </summary>
        public void Launch(Vector2 direction, float damageMultiplier, int pierceCount)
        {
            Launch(direction, damageMultiplier, pierceCount, null);
        }

        public void Launch(Vector2 direction, float damageMultiplier, int pierceCount, PlayerSpecialUpgradeController ownerSpecialUpgradeController)
        {
            Launch(direction, damageMultiplier, pierceCount, ownerSpecialUpgradeController, 0);
        }

        public void Launch(Vector2 direction, float damageMultiplier, int pierceCount, PlayerSpecialUpgradeController ownerSpecialUpgradeController, int ownerAttackGroupId)
        {
            if (direction.sqrMagnitude <= 0f)
                return;

            specialUpgradeController = ownerSpecialUpgradeController;
            attackGroupId = ownerAttackGroupId;
            moveDirection = direction.normalized;
            transform.right = moveDirection;
            effectiveDamage = Mathf.Max(0.1f, damage * Mathf.Max(0.1f, damageMultiplier));
            remainingPierceCount = Mathf.Max(0, pierceCount);
            remainingReflectCount = specialUpgradeController == null ? 0 : specialUpgradeController.GetProjectileReflectCount();
            hitEnemies.Clear();
            CombatVFX.AttachTrail(gameObject, vfxKind, 0.08f, 0.16f);
        }

        private float GetDamageForEnemy(EnemyHealth enemyHealth)
        {
            if (attackGroupId <= 0)
                return effectiveDamage;

            PruneAttackGroups();

            int enemyId = enemyHealth.GetInstanceID();
            if (!attackGroupHitCounts.TryGetValue(attackGroupId, out Dictionary<int, int> hitCounts))
            {
                hitCounts = new Dictionary<int, int>();
                attackGroupHitCounts[attackGroupId] = hitCounts;
            }

            attackGroupLastUsedTimes[attackGroupId] = Time.time;
            int previousHitCount = hitCounts.TryGetValue(enemyId, out int count) ? count : 0;
            hitCounts[enemyId] = previousHitCount + 1;

            if (previousHitCount == 0)
                return effectiveDamage;

            return Mathf.Max(0.1f, effectiveDamage * repeatedSameAttackDamageMultiplier);
        }

        private void PruneAttackGroups()
        {
            if (attackGroupLastUsedTimes.Count == 0)
                return;

            float cutoffTime = Time.time - repeatedSameAttackHistorySeconds;
            expiredAttackGroupIds.Clear();

            foreach (KeyValuePair<int, float> pair in attackGroupLastUsedTimes)
            {
                if (pair.Value < cutoffTime)
                    expiredAttackGroupIds.Add(pair.Key);
            }

            for (int i = 0; i < expiredAttackGroupIds.Count; i++)
            {
                int expiredId = expiredAttackGroupIds[i];
                attackGroupLastUsedTimes.Remove(expiredId);
                attackGroupHitCounts.Remove(expiredId);
            }
        }

        private bool TryReflectProjectile()
        {
            if (remainingReflectCount <= 0 || specialUpgradeController == null)
                return false;

            if (!specialUpgradeController.TryGetProjectileReflectDirection(transform.position, hitEnemies, out Vector2 reflectedDirection))
                return false;

            remainingReflectCount--;
            moveDirection = reflectedDirection.normalized;
            transform.right = moveDirection;
            lifeTimer = Mathf.Min(lifeTimer, lifeTime * 0.45f);
            return true;
        }

        public void SetVisual(Sprite sprite, float visualScale, float colliderRadius)
        {
            if (sprite == null)
                return;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (circleCollider == null)
                circleCollider = GetComponent<CircleCollider2D>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                vfxKind = CombatVFX.KindFromProjectileSprite(sprite);
            }

            transform.localScale = new Vector3(Mathf.Max(0.1f, visualScale), Mathf.Max(0.1f, visualScale), 1f);

            if (circleCollider != null && colliderRadius > 0f)
                circleCollider.radius = colliderRadius;
        }

        private void DestroyProjectile(bool playFadeEffect, float effectSize)
        {
            if (isDestroying)
                return;

            isDestroying = true;

            if (playFadeEffect)
                CombatVFX.PlayBurst(transform.position, vfxKind, effectSize, 0.14f);

            Destroy(gameObject);
        }
    }
}
