using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VampireLike.Audio;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어 체력, 접촉 피해, 무적 시간, 피격 연출, 사망 처리를 관리한다.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        // 플레이어 최대 체력이다.
        [SerializeField]
        private int maxHealth = 10;

        // 적과 접촉했을 때 받는 피해량이다.
        [SerializeField]
        private int contactDamage = 1;

        // 피해를 받은 뒤 다시 피해를 받을 수 없도록 막는 시간이다.
        [SerializeField]
        private float invincibleDuration = 1f;

        [SerializeField]
        private float shieldBlockInvincibleDuration = 0.35f;

        // 피격 시 빨간색으로 깜빡이는 전체 시간이다.
        [SerializeField]
        private float hitFlashDuration = 0.6f;

        // 피격 점멸의 한 번 깜빡임 간격이다.
        [SerializeField]
        private float hitFlashInterval = 0.08f;

        [Header("Hit Detection")]
        // 이동용 Collider와 별개로 플레이어 몸통 피격 범위를 조절한다.
        [SerializeField]
        private bool useCustomHurtbox = true;

        // 플레이어 위치 기준 피격 범위 중심 보정값이다.
        [SerializeField]
        private Vector2 hurtboxOffset = Vector2.zero;

        // 실제 피해를 받을 몸통 판정 크기다. 무기/머리카락보다 몸통 중심에 맞춘다.
        [SerializeField]
        private Vector2 hurtboxSize = new Vector2(0.42f, 0.58f);

        // 보스 대쉬는 시각상 몸통 중심에 맞도록 일반 접촉 판정보다 살짝 위로 보정한다.
        [SerializeField]
        private Vector2 dashHurtboxOffset = Vector2.zero;

        [SerializeField]
        private bool alignDashHurtboxToVisibleSprite = true;

        // Collider를 찾지 못했을 때만 사용하는 예비 접촉 검사 반경이다.
        [SerializeField]
        private float contactCheckRadius = 0.35f;

        // 접촉 피해를 검사할 Enemy 레이어다.
        [SerializeField]
        private LayerMask enemyLayerMask = 1 << 7;

        [SerializeField]
        private float deathSlowMotionScale = 0.25f;

        [SerializeField]
        private float gameOverDelay = 1.2f;

        private int currentHealth;
        private float invincibleTimer;
        private bool isDead;
        private readonly Collider2D[] contactResults = new Collider2D[8];
        private Collider2D playerContactCollider;
        private ContactFilter2D enemyContactFilter;
        private SpriteRenderer[] spriteRenderers;
        private Color[] originalColors;
        private bool[] originalRendererEnabledStates;
        private Coroutine hitFlashRoutine;
        private Coroutine deathRoutine;
        private global::PlayerSpriteAnimator spriteAnimator;
        private SfxType[] hitSfxTypes = { SfxType.PlayerHit };
        private SfxType deathSfxType = SfxType.GameOver;

        public bool IsDead => isDead;
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public float HealthProgress => maxHealth <= 0 ? 0f : Mathf.Clamp01((float)currentHealth / maxHealth);

        public void SetCharacterSfx(IReadOnlyList<SfxType> characterHitSfxTypes, SfxType characterDeathSfxType)
        {
            if (characterHitSfxTypes != null && characterHitSfxTypes.Count > 0)
            {
                hitSfxTypes = new SfxType[characterHitSfxTypes.Count];
                for (int i = 0; i < characterHitSfxTypes.Count; i++)
                    hitSfxTypes[i] = characterHitSfxTypes[i];
            }

            deathSfxType = characterDeathSfxType;
        }

        private void Awake()
        {
            // Play 재시작 시 이전 게임 오버 상태가 남지 않게 초기화한다.
            GameState.ResetGame();
            currentHealth = maxHealth;
            spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();
            playerContactCollider = GetComponent<Collider2D>();
            RefreshEnemyContactFilter();

            if (GetComponent<GameOverUI>() == null)
                gameObject.AddComponent<GameOverUI>();

            if (GetComponent<PlayerHealthUI>() == null)
                gameObject.AddComponent<PlayerHealthUI>();
        }

        private void Start()
        {
            CacheSpriteRenderer();
        }

        private void Update()
        {
            // 무적 시간은 일반 시간 흐름을 따른다. 일시정지 중에는 Time.deltaTime이 0이다.
            if (invincibleTimer > 0f)
                invincibleTimer -= Time.deltaTime;

            CheckEnemyContact();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryApplyContactDamage(collision.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryApplyContactDamage(collision.gameObject);
        }

        private void TryApplyContactDamage(GameObject other)
        {
            // 일시정지/사망 중에는 접촉 피해를 처리하지 않는다.
            if (isDead || Time.timeScale <= 0f)
                return;

            if (!IsEnemyObject(other))
                return;

            TakeDamage(GetContactDamage(other), GetHitDirection(other));
        }

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            contactDamage = Mathf.Max(1, contactDamage);
            invincibleDuration = Mathf.Max(0f, invincibleDuration);
            shieldBlockInvincibleDuration = Mathf.Max(0f, shieldBlockInvincibleDuration);
            hitFlashDuration = Mathf.Max(0f, hitFlashDuration);
            hitFlashInterval = Mathf.Max(0.01f, hitFlashInterval);
            hurtboxSize.x = Mathf.Max(0.05f, hurtboxSize.x);
            hurtboxSize.y = Mathf.Max(0.05f, hurtboxSize.y);
            contactCheckRadius = Mathf.Max(0.01f, contactCheckRadius);
            deathSlowMotionScale = Mathf.Clamp(deathSlowMotionScale, 0.05f, 1f);
            gameOverDelay = Mathf.Max(0f, gameOverDelay);
            RefreshEnemyContactFilter();
        }

        private void OnDrawGizmosSelected()
        {
            if (!useCustomHurtbox)
                return;

            // Scene 뷰에서 실제 피격 범위를 확인하며 캐릭터별로 조절할 수 있게 표시한다.
            Gizmos.color = new Color(1f, 0.2f, 0.15f, 0.55f);
            Vector3 center = transform.position + (Vector3)hurtboxOffset;
            Gizmos.DrawWireCube(center, new Vector3(hurtboxSize.x, hurtboxSize.y, 0f));

            Gizmos.color = new Color(1f, 0.75f, 0.05f, 0.55f);
            Vector3 dashCenter = center + (Vector3)dashHurtboxOffset;
            Gizmos.DrawWireCube(dashCenter, new Vector3(hurtboxSize.x, hurtboxSize.y, 0f));
        }

        public void TakeDamage(int damage)
        {
            TakeDamage(damage, Vector2.zero);
        }

        public void TakeDamage(int damage, Vector2 hitDirection)
        {
            ApplyDamage(damage, hitDirection, false);
        }

        public bool TakeDashDamage(int damage, Vector2 hitDirection)
        {
            return ApplyDamage(damage, hitDirection, true);
        }

        private bool ApplyDamage(int damage, Vector2 hitDirection, bool ignoreInvincibility)
        {
            // 무적 시간 중에는 반복 피해를 막는다.
            if (isDead || damage <= 0 || (!ignoreInvincibility && invincibleTimer > 0f))
                return false;

            PlayerSpecialUpgradeController specialUpgradeController = GetComponent<PlayerSpecialUpgradeController>();

            if (specialUpgradeController != null && specialUpgradeController.TryBlockDamage(hitDirection))
            {
                invincibleTimer = Mathf.Max(invincibleTimer, shieldBlockInvincibleDuration);
                return true;
            }

            currentHealth -= damage;
            invincibleTimer = invincibleDuration;

            if (specialUpgradeController != null)
            {
                invincibleTimer += specialUpgradeController.GetBonusInvincibleDuration();
                specialUpgradeController.NotifyPlayerDamaged();
            }

            GameSfx.PlayRandom(hitSfxTypes);

            if (spriteAnimator == null)
                spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();

            if (spriteAnimator != null)
                spriteAnimator.PlayHit();

            PlayHitFlash();

            if (currentHealth <= 0)
                Die();

            return true;
        }

        /// <summary>
        /// 최대 체력 강화에서 호출한다. 최대 체력과 현재 체력을 함께 올린다.
        /// </summary>
        public void SetMaxHealth(int value)
        {
            if (isDead)
                return;

            maxHealth = Mathf.Max(1, value);
            currentHealth = maxHealth;
        }

        public void IncreaseMaxHealth(int amount)
        {
            if (amount <= 0 || isDead)
                return;

            maxHealth += amount;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        public void MultiplyMaxHealth(float multiplier)
        {
            if (multiplier <= 1f || isDead)
                return;

            int previousMaxHealth = maxHealth;
            int increasedMaxHealth = Mathf.Max(previousMaxHealth + 1, Mathf.CeilToInt(previousMaxHealth * multiplier));
            int increasedAmount = increasedMaxHealth - previousMaxHealth;
            maxHealth = increasedMaxHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + increasedAmount);
        }

        /// <summary>
        /// 회복 강화에서 호출한다. 현재 체력을 최대 체력 안에서 회복한다.
        /// </summary>
        public void Heal(int amount)
        {
            Heal(amount, SfxType.Heal);
        }

        public void Heal(int amount, SfxType healSfxType)
        {
            if (amount <= 0 || isDead)
                return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            GameSfx.Play(healSfxType);
        }

        public bool IsHitByProjectileSweep(Vector2 startPosition, Vector2 endPosition, float projectileRadius)
        {
            return IsHitBySweepBounds(startPosition, endPosition, projectileRadius, GetHurtboxBounds());
        }

        public bool IsHitByDashSweep(Vector2 startPosition, Vector2 endPosition, float dashRadius)
        {
            return IsHitBySweepBounds(startPosition, endPosition, dashRadius, GetDashHurtboxBounds());
        }

        public bool IsHitByDashSweep(Vector2 startPosition, Vector2 endPosition, float forwardPadding, float perpendicularPadding)
        {
            return IsHitByOrientedSweepBounds(startPosition, endPosition, forwardPadding, perpendicularPadding, GetDashHurtboxBounds());
        }

        private static bool IsHitBySweepBounds(Vector2 startPosition, Vector2 endPosition, float sweepRadius, Bounds hurtboxBounds)
        {
            float radius = Mathf.Max(0f, sweepRadius);
            Rect expandedBounds = new Rect(
                hurtboxBounds.min.x - radius,
                hurtboxBounds.min.y - radius,
                hurtboxBounds.size.x + radius * 2f,
                hurtboxBounds.size.y + radius * 2f);

            if (expandedBounds.Contains(startPosition) || expandedBounds.Contains(endPosition))
                return true;

            return SegmentIntersectsRect(startPosition, endPosition, expandedBounds);
        }

        private static bool IsHitByOrientedSweepBounds(
            Vector2 startPosition,
            Vector2 endPosition,
            float forwardPadding,
            float perpendicularPadding,
            Bounds hurtboxBounds)
        {
            Vector2 delta = endPosition - startPosition;
            float length = delta.magnitude;

            if (length <= 0.0001f)
                return IsPointInsideExpandedBounds(startPosition, hurtboxBounds, Mathf.Max(forwardPadding, perpendicularPadding));

            Vector2 forward = delta / length;
            Vector2 perpendicular = new Vector2(-forward.y, forward.x);
            float minForward = float.PositiveInfinity;
            float maxForward = float.NegativeInfinity;
            float minPerpendicular = float.PositiveInfinity;
            float maxPerpendicular = float.NegativeInfinity;

            Vector2 center = hurtboxBounds.center;
            Vector2 extents = hurtboxBounds.extents;

            for (int y = -1; y <= 1; y += 2)
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    Vector2 corner = center + new Vector2(extents.x * x, extents.y * y);
                    Vector2 fromStart = corner - startPosition;
                    float forwardDistance = Vector2.Dot(fromStart, forward);
                    float perpendicularDistance = Vector2.Dot(fromStart, perpendicular);

                    minForward = Mathf.Min(minForward, forwardDistance);
                    maxForward = Mathf.Max(maxForward, forwardDistance);
                    minPerpendicular = Mathf.Min(minPerpendicular, perpendicularDistance);
                    maxPerpendicular = Mathf.Max(maxPerpendicular, perpendicularDistance);
                }
            }

            float safeForwardPadding = Mathf.Max(0f, forwardPadding);
            float safePerpendicularPadding = Mathf.Max(0f, perpendicularPadding);
            bool overlapsForward = maxForward >= -safeForwardPadding && minForward <= length + safeForwardPadding;
            bool overlapsPerpendicular = maxPerpendicular >= -safePerpendicularPadding && minPerpendicular <= safePerpendicularPadding;
            return overlapsForward && overlapsPerpendicular;
        }

        private static bool IsPointInsideExpandedBounds(Vector2 point, Bounds bounds, float padding)
        {
            Rect expandedBounds = new Rect(
                bounds.min.x - padding,
                bounds.min.y - padding,
                bounds.size.x + padding * 2f,
                bounds.size.y + padding * 2f);

            return expandedBounds.Contains(point);
        }

        private void CacheSpriteRenderer()
        {
            // PlayerVisual을 포함한 모든 자식 SpriteRenderer의 원래 상태를 저장한다.
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            originalColors = new Color[spriteRenderers.Length];
            originalRendererEnabledStates = new bool[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
                originalRendererEnabledStates[i] = spriteRenderers[i].enabled;
            }
        }

        private void PlayHitFlash()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
                CacheSpriteRenderer();

            if (spriteRenderers == null || spriteRenderers.Length == 0 || hitFlashDuration <= 0f)
                return;

            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash());
        }

        private IEnumerator HitFlash()
        {
            float elapsedTime = 0f;

            while (elapsedTime < hitFlashDuration)
            {
                SetVisualColors(new Color(1f, 0.15f, 0.1f, 1f));
                yield return new WaitForSeconds(hitFlashInterval);
                elapsedTime += hitFlashInterval;

                RestoreVisualColors();
                yield return new WaitForSeconds(hitFlashInterval);
                elapsedTime += hitFlashInterval;
            }

            RestoreVisuals();
            hitFlashRoutine = null;
        }

        private void CheckEnemyContact()
        {
            // 적과 겹쳐 있는 상태에서도 일정 무적 시간마다 피해를 받도록 보완 검사한다.
            if (isDead || Time.timeScale <= 0f || invincibleTimer > 0f)
                return;

            int hitCount = GetEnemyContactCount();

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D enemyCollider = contactResults[i];

                if (enemyCollider != null && IsEnemyObject(enemyCollider.gameObject))
                {
                    TakeDamage(GetContactDamage(enemyCollider.gameObject), GetHitDirection(enemyCollider.gameObject));
                    return;
                }
            }
        }

        private int GetEnemyContactCount()
        {
            if (useCustomHurtbox)
            {
                Vector2 hurtboxCenter = (Vector2)transform.position + hurtboxOffset;

                // 실제 피격 범위를 별도 Capsule로 검사해 캐릭터 몸통 중심에 맞는 판정을 만든다.
                return Physics2D.OverlapCapsule(
                    hurtboxCenter,
                    hurtboxSize,
                    CapsuleDirection2D.Vertical,
                    0f,
                    enemyContactFilter,
                    contactResults);
            }

            if (playerContactCollider == null)
                playerContactCollider = GetComponent<Collider2D>();

            if (playerContactCollider != null)
            {
                // 실제 플레이어 Collider와 겹친 적만 접촉 피해 후보로 본다.
                // transform 기준 원형 검사보다 시각적으로 납득되는 피격 판정을 만든다.
                return playerContactCollider.Overlap(enemyContactFilter, contactResults);
            }

            return Physics2D.OverlapCircle(transform.position, contactCheckRadius, enemyContactFilter, contactResults);
        }

        private Bounds GetHurtboxBounds()
        {
            if (useCustomHurtbox)
            {
                Vector3 center = transform.position + (Vector3)hurtboxOffset;
                return new Bounds(center, new Vector3(hurtboxSize.x, hurtboxSize.y, 0f));
            }

            if (playerContactCollider == null)
                playerContactCollider = GetComponent<Collider2D>();

            if (playerContactCollider != null)
                return playerContactCollider.bounds;

            return new Bounds(transform.position, Vector3.one * contactCheckRadius * 2f);
        }

        private Bounds GetDashHurtboxBounds()
        {
            Bounds hurtboxBounds = GetHurtboxBounds();

            if (alignDashHurtboxToVisibleSprite && TryGetVisibleSpriteBounds(out Bounds visualBounds))
            {
                Vector3 visualCenter = visualBounds.center;
                hurtboxBounds.center = new Vector3(visualCenter.x, visualCenter.y, hurtboxBounds.center.z);
            }

            hurtboxBounds.center += (Vector3)dashHurtboxOffset;
            return hurtboxBounds;
        }

        private bool TryGetVisibleSpriteBounds(out Bounds visualBounds)
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
                CacheSpriteRenderer();

            visualBounds = default;
            bool hasBounds = false;

            if (spriteRenderers == null)
                return false;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];

                if (spriteRenderer == null
                    || !spriteRenderer.enabled
                    || !spriteRenderer.gameObject.activeInHierarchy
                    || spriteRenderer.sprite == null)
                    continue;

                if (!hasBounds)
                {
                    visualBounds = spriteRenderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    visualBounds.Encapsulate(spriteRenderer.bounds);
                }
            }

            return hasBounds;
        }

        private static bool SegmentIntersectsRect(Vector2 startPosition, Vector2 endPosition, Rect rect)
        {
            Vector2 delta = endPosition - startPosition;
            float minT = 0f;
            float maxT = 1f;

            return ClipSegmentAxis(startPosition.x, delta.x, rect.xMin, rect.xMax, ref minT, ref maxT)
                && ClipSegmentAxis(startPosition.y, delta.y, rect.yMin, rect.yMax, ref minT, ref maxT);
        }

        private static bool ClipSegmentAxis(float start, float delta, float min, float max, ref float minT, ref float maxT)
        {
            if (Mathf.Abs(delta) < 0.0001f)
                return start >= min && start <= max;

            float inverseDelta = 1f / delta;
            float t1 = (min - start) * inverseDelta;
            float t2 = (max - start) * inverseDelta;

            if (t1 > t2)
            {
                float swap = t1;
                t1 = t2;
                t2 = swap;
            }

            minT = Mathf.Max(minT, t1);
            maxT = Mathf.Min(maxT, t2);
            return minT <= maxT;
        }

        private void RefreshEnemyContactFilter()
        {
            enemyContactFilter = new ContactFilter2D();
            enemyContactFilter.SetLayerMask(enemyLayerMask);
            enemyContactFilter.useTriggers = true;
        }

        private bool IsEnemyObject(GameObject target)
        {
            return target != null && (target.CompareTag("Enemy") || target.GetComponentInParent<EnemyHealth>() != null);
        }

        private Vector2 GetHitDirection(GameObject attacker)
        {
            if (attacker == null)
                return Vector2.zero;

            if (useCustomHurtbox)
            {
                Vector2 hurtboxCenter = (Vector2)transform.position + hurtboxOffset;
                Vector2 directionFromAttacker = hurtboxCenter - (Vector2)attacker.transform.position;

                if (directionFromAttacker.sqrMagnitude > 0.0001f)
                    return directionFromAttacker.normalized;
            }

            if (playerContactCollider != null)
            {
                Vector2 closestPoint = playerContactCollider.ClosestPoint(attacker.transform.position);
                Vector2 directionFromContact = (Vector2)transform.position - closestPoint;

                if (directionFromContact.sqrMagnitude > 0.0001f)
                    return directionFromContact.normalized;
            }

            return ((Vector2)transform.position - (Vector2)attacker.transform.position).normalized;
        }

        private int GetContactDamage(GameObject enemyObject)
        {
            EnemyContactDamage enemyContactDamage = enemyObject.GetComponentInParent<EnemyContactDamage>();
            if (enemyContactDamage == null)
                return contactDamage;

            return enemyContactDamage.enabled ? enemyContactDamage.ContactDamage : 0;
        }

        private void SetVisualColors(Color color)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                if (!originalRendererEnabledStates[i])
                    continue;

                spriteRenderers[i].color = color;
            }
        }

        private void RestoreVisuals()
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                spriteRenderers[i].color = originalColors[i];
                spriteRenderers[i].enabled = originalRendererEnabledStates[i];
            }

        }

        private void RestoreVisualColors()
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                spriteRenderers[i].color = originalColors[i];
            }
        }

        private void Die()
        {
            // 현재는 게임 오버 상태 전환과 이동/공격 정지만 처리한다. UI는 이후 단계에서 연결한다.
            isDead = true;
            currentHealth = 0;
            GameSfx.Play(deathSfxType);

            if (spriteAnimator == null)
                spriteAnimator = GetComponent<global::PlayerSpriteAnimator>();

            if (hitFlashRoutine != null)
            {
                StopCoroutine(hitFlashRoutine);
                hitFlashRoutine = null;
                RestoreVisualColors();
            }

            if (spriteAnimator != null)
                spriteAnimator.PlayDeath();

            global::PlayerController playerController = GetComponent<global::PlayerController>();
            PlayerAutoAttack playerAutoAttack = GetComponent<PlayerAutoAttack>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (playerController != null)
                playerController.enabled = false;

            if (playerAutoAttack != null)
                playerAutoAttack.StopAttacking();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            if (deathRoutine != null)
                StopCoroutine(deathRoutine);

            deathRoutine = StartCoroutine(ShowGameOverAfterDeath());
        }

        private IEnumerator ShowGameOverAfterDeath()
        {
            Time.timeScale = deathSlowMotionScale;

            yield return new WaitForSecondsRealtime(gameOverDelay);

            if (!GameState.IsGameOver)
                GameState.SetGameOver();

            GameBgm.Play(BgmType.GameOver);

            deathRoutine = null;
        }
    }
}
