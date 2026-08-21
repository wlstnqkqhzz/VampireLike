using System.Collections;
using UnityEngine;
using VampireLike.Audio;

namespace VampireLike.Combat
{
    /// <summary>
    /// 일정 시간마다 공격 범위 안의 가장 가까운 적을 찾아 투사체를 발사한다.
    /// </summary>
    public class PlayerAutoAttack : MonoBehaviour
    {
        // 발사할 투사체 프리팹이다.
        [SerializeField]
        private ProjectileController projectilePrefab;

        // 발사 위치다. 비어 있으면 플레이어 위치를 사용한다.
        [SerializeField]
        private Transform firePoint;

        // 공격 사이의 대기 시간이다. 공격 속도 강화는 이 값을 줄인다.
        [SerializeField]
        private float attackInterval = 1f;

        // 가장 가까운 적을 찾을 최대 거리다.
        [SerializeField]
        private float attackRange = 6f;

        // 공격 속도 강화가 누적되어도 이 값보다 빠르게는 공격하지 않는다.
        [SerializeField]
        private float minimumAttackInterval = 0.25f;

        // 투사체 피해 강화로 누적되는 공격력 배율이다.
        [SerializeField]
        private float projectileDamageMultiplier = 1f;

        [SerializeField]
        private int projectileCount = 1;

        // 다중 발사 강화 시 같은 방향으로 이어서 쏘는 탄 사이의 시간 간격이다.
        [SerializeField]
        private float projectileBurstDelay = 0.12f;

        // 투사체 연속 발사 강화로 추가 발사되는 탄 수다.
        [SerializeField]
        private int sequentialShotCount;

        // 투사체 연속 발사 강화 시 추가 탄 사이의 시간 간격이다.
        [SerializeField]
        private float sequentialShotDelay = 0.16f;

        [SerializeField]
        private int projectilePierceCount;

        [SerializeField]
        private SfxType attackSfxType = SfxType.PlayerShoot;

        private Sprite projectileSpriteOverride;
        private float projectileVisualScale = 1f;
        private float projectileColliderRadius = -1f;
        private float attackTimer;
        private bool isStopped;
        private Coroutine burstRoutine;
        private global::PlayerSpriteAnimator spriteAnimator;
        private global::PlayerController playerController;
        private PlayerSpecialUpgradeController specialUpgradeController;
        private PlayerEffectAnchors effectAnchors;

        private void Awake()
        {
            if (firePoint == null)
                firePoint = transform;

            spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();
            playerController = GetComponent<global::PlayerController>();
            specialUpgradeController = GetComponent<PlayerSpecialUpgradeController>();
            effectAnchors = GetComponent<PlayerEffectAnchors>();

            if (effectAnchors == null)
                effectAnchors = gameObject.AddComponent<PlayerEffectAnchors>();
        }

        private void Update()
        {
            if (isStopped || GameState.IsGameOver || projectilePrefab == null || burstRoutine != null)
                return;

            attackTimer += Time.deltaTime;

            if (attackTimer < attackInterval)
                return;

            EnemyHealth target = FindClosestEnemyInRange();

            if (target == null)
                return;

            attackTimer = 0f;
            burstRoutine = StartCoroutine(FireBurstAt(target.transform));
        }

        private void OnValidate()
        {
            minimumAttackInterval = Mathf.Max(0.05f, minimumAttackInterval);
            attackInterval = Mathf.Max(minimumAttackInterval, attackInterval);
            attackRange = Mathf.Max(0f, attackRange);
            projectileDamageMultiplier = Mathf.Max(0.1f, projectileDamageMultiplier);
            projectileCount = Mathf.Max(1, projectileCount);
            projectileBurstDelay = Mathf.Max(0f, projectileBurstDelay);
            sequentialShotCount = Mathf.Max(0, sequentialShotCount);
            sequentialShotDelay = Mathf.Max(0.02f, sequentialShotDelay);
            projectilePierceCount = Mathf.Max(0, projectilePierceCount);
        }

        private void OnDisable()
        {
            if (burstRoutine == null)
                return;

            StopCoroutine(burstRoutine);
            burstRoutine = null;
        }

        public void StopAttacking()
        {
            isStopped = true;
        }

        /// <summary>
        /// 공격 간격 강화에서 호출한다. 예: 0.88을 곱하면 공격 간격이 12% 줄어든다.
        /// </summary>
        public void MultiplyAttackInterval(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            attackInterval = Mathf.Max(minimumAttackInterval, attackInterval * multiplier);
        }

        /// <summary>
        /// 투사체 공격력 강화에서 호출한다.
        /// </summary>
        public void MultiplyProjectileDamage(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            projectileDamageMultiplier *= multiplier;
        }

        /// <summary>
        /// 다중 발사 강화에서 호출한다.
        /// </summary>
        public void AddProjectileCount(int amount)
        {
            projectileCount = Mathf.Max(1, projectileCount + amount);
        }

        /// <summary>
        /// 투사체 연속 발사 강화에서 호출한다.
        /// </summary>
        public void AddSequentialShotCount(int amount)
        {
            sequentialShotCount = Mathf.Max(0, sequentialShotCount + amount);
        }

        /// <summary>
        /// 관통탄 강화에서 호출한다.
        /// </summary>
        public void AddProjectilePierceCount(int amount)
        {
            projectilePierceCount = Mathf.Max(0, projectilePierceCount + amount);
        }

        public void SetProjectileVisual(Sprite sprite, float visualScale, float colliderRadius)
        {
            projectileSpriteOverride = sprite;
            projectileVisualScale = Mathf.Max(0.1f, visualScale);
            projectileColliderRadius = colliderRadius;
        }

        public void SetAttackSfx(SfxType sfxType)
        {
            attackSfxType = sfxType;
        }

        private EnemyHealth FindClosestEnemyInRange()
        {
            // EnemyHealth.ActiveEnemies를 순회해 매 프레임 FindObject 계열 호출을 피한다.
            EnemyHealth closestEnemy = null;
            float closestSqrDistance = attackRange * attackRange;
            Vector2 origin = GetFirePosition();

            foreach (EnemyHealth enemy in EnemyHealth.ActiveEnemies)
            {
                if (enemy == null || enemy.IsDead)
                    continue;

                float sqrDistance = ((Vector2)enemy.transform.position - origin).sqrMagnitude;

                if (sqrDistance > closestSqrDistance)
                    continue;

                closestEnemy = enemy;
                closestSqrDistance = sqrDistance;
            }

            return closestEnemy;
        }

        private IEnumerator FireBurstAt(Transform target)
        {
            // 발사 시점의 방향을 기준으로 직선 투사체를 만든다. 유도탄은 아니다.
            Vector2 firePosition = GetFirePosition();
            Vector2 direction = ((Vector2)target.position - firePosition).normalized;

            if (direction.sqrMagnitude <= 0f)
            {
                burstRoutine = null;
                yield break;
            }

            if (specialUpgradeController == null)
                specialUpgradeController = GetComponent<PlayerSpecialUpgradeController>();

            if (spriteAnimator == null)
                spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();

            if (spriteAnimator != null && (playerController == null || !playerController.IsMoving))
                spriteAnimator.PlayAttack();

            // 한 번의 자동 공격에서 나온 산탄/다중/연속 발사를 같은 묶음으로 취급해 같은 적 중복 피해를 감쇠한다.
            int attackGroupId = ProjectileController.CreateAttackGroupId();
            int shotCount = Mathf.Max(1, projectileCount);
            for (int i = 0; i < shotCount; i++)
            {
                firePosition = GetFirePosition();
                GameSfx.Play(attackSfxType);

                Vector2[] directions = specialUpgradeController == null
                    ? new[] { direction }
                    : specialUpgradeController.GetProjectileDirections(direction);

                if (directions.Length > 1)
                    GameSfx.Play(SfxType.SkillScatter);

                float scatterDamageMultiplier = specialUpgradeController == null
                    ? 1f
                    : specialUpgradeController.GetProjectileDamageMultiplierForDirections(directions.Length);

                foreach (Vector2 shotDirection in directions)
                {
                    ProjectileController projectile = Instantiate(projectilePrefab, firePosition, Quaternion.identity);
                    projectile.SetVisual(projectileSpriteOverride, projectileVisualScale, projectileColliderRadius);
                    projectile.Launch(shotDirection, projectileDamageMultiplier * scatterDamageMultiplier, projectilePierceCount, specialUpgradeController, attackGroupId);
                }

                if (i < shotCount - 1 && projectileBurstDelay > 0f)
                    yield return new WaitForSeconds(projectileBurstDelay);
            }

            int extraShotCount = Mathf.Max(0, sequentialShotCount);
            for (int i = 0; i < extraShotCount; i++)
            {
                if (sequentialShotDelay > 0f)
                    yield return new WaitForSeconds(sequentialShotDelay);

                firePosition = GetFirePosition();
                GameSfx.Play(attackSfxType);

                ProjectileController projectile = Instantiate(projectilePrefab, firePosition, Quaternion.identity);
                projectile.SetVisual(projectileSpriteOverride, projectileVisualScale, projectileColliderRadius);
                projectile.Launch(direction, projectileDamageMultiplier, projectilePierceCount, specialUpgradeController, attackGroupId);
            }

            burstRoutine = null;
        }

        private Vector2 GetFirePosition()
        {
            if (firePoint != null && firePoint != transform)
                return firePoint.position;

            if (effectAnchors == null)
                effectAnchors = GetComponent<PlayerEffectAnchors>();

            return effectAnchors == null ? transform.position : effectAnchors.EffectCenterPosition;
        }
    }
}
