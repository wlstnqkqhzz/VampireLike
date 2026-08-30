using System.Collections;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.Settings;
using VampireLike.VFX;
using VampireLike.World;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 플레이어 방향을 미리 고정한 뒤 빠르게 돌진하는 패턴이다.
    /// </summary>
    public class DashPattern : BossPattern
    {
        protected override bool UseAutomaticAnimation => false;

        [SerializeField]
        private float prepareTime = 0.38f;

        [SerializeField]
        private float dashSpeed = 12f;

        [SerializeField]
        private float dashDuration = 0.45f;

        [SerializeField]
        private float endLag = 0.12f;

        [SerializeField]
        private int dashChainCount = 1;

        [SerializeField]
        private float chainedDashPrepareMultiplier = 0f;

        [SerializeField]
        private float chainedDashDelay = 0f;

        [SerializeField]
        private float minimumTriggerDistance = 0.55f;

        [SerializeField]
        private float maximumTriggerDistance = 4.8f;

        [SerializeField]
        private float mobilePortraitPrepareMultiplier = 1f;

        [SerializeField]
        private float mobilePortraitDashSpeedMultiplier = 1f;

        [SerializeField]
        private float mobilePortraitMaximumTriggerDistance = 4.2f;

        [SerializeField]
        private bool dashTowardMapEdge = true;

        [SerializeField]
        private float boundaryStopPadding = 0.55f;

        [SerializeField]
        private float maximumMapEdgeDashTime = 1.3f;

        [Header("돌진 경고/충격 연출")]
        [SerializeField]
        private bool showDashPathIndicator = false;

        [SerializeField]
        private float telegraphWidth = 0.08f;

        [SerializeField]
        private float telegraphLength = 5.5f;

        [SerializeField]
        private Color telegraphColor = new Color(1f, 0.56f, 0.18f, 0.34f);

        [SerializeField]
        private float impactSize = 0.85f;

        [Header("돌진 피격 판정")]
        [SerializeField]
        private bool useSweptDashHitbox = true;

        [SerializeField]
        private bool disableContactDamageDuringDash = true;

        [SerializeField]
        private float dashHitRadius = 0.58f;

        [SerializeField]
        private float dashHitForwardPadding = 0.36f;

        [SerializeField]
        private float dashHitPerpendicularPadding = 0.26f;

        private EnemyContactDamage contactDamage;
        private PlayerHealth playerHealth;
        private Collider2D bossCollider;

        protected override bool CanExecutePattern()
        {
            if (Player == null || BossRigidbody == null)
                return false;

            float sqrDistance = ((Vector2)Player.position - BossRigidbody.position).sqrMagnitude;
            float currentMaximumTriggerDistance = GetEffectiveMaximumTriggerDistance();
            return sqrDistance >= minimumTriggerDistance * minimumTriggerDistance
                && sqrDistance <= currentMaximumTriggerDistance * currentMaximumTriggerDistance;
        }

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null || BossRigidbody == null)
                yield break;

            int chainCount = Mathf.Max(1, dashChainCount);

            for (int i = 0; i < chainCount && !Boss.IsDead; i++)
            {
                float prepareMultiplier = i == 0 ? 1f : chainedDashPrepareMultiplier;
                yield return ExecuteSingleDash(prepareMultiplier);

                if (i < chainCount - 1 && chainedDashDelay > 0f)
                    yield return new WaitForSeconds(chainedDashDelay);
            }

            Boss.SetState(BossState.Recovering, false);
            yield return new WaitForSeconds(endLag);
        }

        private IEnumerator ExecuteSingleDash(float prepareMultiplier)
        {
            Vector2 dashDirection = GetDashDirection();
            float effectivePrepareTime = GetEffectivePrepareTime();
            effectivePrepareTime *= Mathf.Max(0f, prepareMultiplier);
            float effectiveDashDuration = GetEffectiveDashDuration(dashDirection);

            if (effectivePrepareTime > 0f)
            {
                Boss.SetState(BossState.Preparing, false);
                Boss.FaceDirection(dashDirection);
                Boss.ShowAttackFrame(0);
                CombatVFX.PlayBossCastAura(transform, CombatVFXKind.TargetWarning, 0.9f, effectivePrepareTime, 1500);

                if (showDashPathIndicator)
                    BossTelegraph.ShowLine(BossRigidbody.position, dashDirection, GetTelegraphDistance(effectiveDashDuration), telegraphWidth, effectivePrepareTime, telegraphColor, 1480);

                yield return new WaitForSeconds(effectivePrepareTime);
            }

            dashDirection = GetDashDirection();
            effectiveDashDuration = GetEffectiveDashDuration(dashDirection);
            Boss.SetState(BossState.Attacking, false);
            Boss.FaceDirection(dashDirection);
            Boss.ShowAttackFrame(1);
            GameSfx.Play(SfxType.BossDash);
            float elapsedTime = 0f;
            float nextTrailTime = 0f;
            bool hasHitPlayer = false;
            CacheHitboxComponents();
            Vector2 dashHitboxCenterOffset = GetDashHitboxCenterOffset();
            SetContactDamageEnabled(false);

            while (elapsedTime < effectiveDashDuration && !Boss.IsDead)
            {
                Vector2 previousPosition = BossRigidbody.position;
                Vector2 nextPosition = previousPosition + dashDirection * GetEffectiveDashSpeed() * Time.fixedDeltaTime;
                nextPosition = ClampDashPosition(nextPosition);
                BossRigidbody.MovePosition(nextPosition);

                if (!hasHitPlayer && TryApplySweptDashDamage(previousPosition + dashHitboxCenterOffset, nextPosition + dashHitboxCenterOffset, dashDirection))
                    hasHitPlayer = true;

                if (elapsedTime >= nextTrailTime)
                {
                    CombatVFX.PlayDirectionalStreak(BossRigidbody.position - dashDirection * 0.18f, -dashDirection, CombatVFXKind.TargetImpact, 0.62f, 0.1f, 0.12f, 1400);
                    nextTrailTime = elapsedTime + 0.08f;
                }

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            BossRigidbody.linearVelocity = Vector2.zero;
            SetContactDamageEnabled(true);
            BossImpact.PlayDashImpact(BossRigidbody.position, dashDirection, impactSize);
        }

        private bool TryApplySweptDashDamage(Vector2 previousPosition, Vector2 nextPosition, Vector2 dashDirection)
        {
            if (!useSweptDashHitbox || playerHealth == null || playerHealth.IsDead)
                return false;

            if (!playerHealth.IsHitByDashSweep(
                previousPosition,
                nextPosition,
                GetEffectiveDashHitForwardPadding(),
                GetEffectiveDashHitPerpendicularPadding()))
                return false;

            int damage = contactDamage == null ? 1 : contactDamage.ContactDamage;
            playerHealth.TakeDamage(damage, dashDirection);
            return true;
        }

        private Vector2 GetDashDirection()
        {
            if (Player == null || BossRigidbody == null)
                return Vector2.down;

            Vector2 dashDirection = ((Vector2)Player.position - BossRigidbody.position).normalized;
            return dashDirection.sqrMagnitude <= 0.001f ? Vector2.down : dashDirection;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            dashSpeed = Mathf.Max(0f, dashSpeed);
            dashDuration = Mathf.Max(0f, dashDuration);
            endLag = Mathf.Max(0f, endLag);
            dashChainCount = Mathf.Max(1, dashChainCount);
            chainedDashPrepareMultiplier = Mathf.Max(0f, chainedDashPrepareMultiplier);
            chainedDashDelay = Mathf.Max(0f, chainedDashDelay);
            minimumTriggerDistance = Mathf.Max(0f, minimumTriggerDistance);
            maximumTriggerDistance = Mathf.Max(minimumTriggerDistance, maximumTriggerDistance);
            mobilePortraitPrepareMultiplier = Mathf.Max(1f, mobilePortraitPrepareMultiplier);
            mobilePortraitDashSpeedMultiplier = Mathf.Clamp(mobilePortraitDashSpeedMultiplier, 0.6f, 1f);
            mobilePortraitMaximumTriggerDistance = Mathf.Max(minimumTriggerDistance, mobilePortraitMaximumTriggerDistance);
            boundaryStopPadding = Mathf.Max(0f, boundaryStopPadding);
            maximumMapEdgeDashTime = Mathf.Max(dashDuration, maximumMapEdgeDashTime);
            telegraphWidth = Mathf.Max(0.01f, telegraphWidth);
            telegraphLength = Mathf.Max(0.1f, telegraphLength);
            impactSize = Mathf.Max(0.1f, impactSize);
            dashHitRadius = Mathf.Max(0.01f, dashHitRadius);
            dashHitForwardPadding = Mathf.Max(0.01f, dashHitForwardPadding);
            dashHitPerpendicularPadding = Mathf.Max(0.01f, dashHitPerpendicularPadding);
        }

        private void OnDisable()
        {
            SetContactDamageEnabled(true);
        }

        protected override void OnPatternCancelled()
        {
            SetContactDamageEnabled(true);
        }

        private float GetEffectivePrepareTime()
        {
            return ShouldUseMobilePortraitTuning() ? prepareTime * mobilePortraitPrepareMultiplier : prepareTime;
        }

        private float GetEffectiveDashSpeed()
        {
            return ShouldUseMobilePortraitTuning() ? dashSpeed * mobilePortraitDashSpeedMultiplier : dashSpeed;
        }

        private float GetEffectiveMaximumTriggerDistance()
        {
            return ShouldUseMobilePortraitTuning() ? Mathf.Min(maximumTriggerDistance, mobilePortraitMaximumTriggerDistance) : maximumTriggerDistance;
        }

        private float GetEffectiveDashDuration(Vector2 dashDirection)
        {
            if (!dashTowardMapEdge || !MapBoundary.TryGetWorldBounds(out Bounds bounds))
                return dashDuration;

            float dashSpeedValue = Mathf.Max(0.01f, GetEffectiveDashSpeed());
            Vector2 currentPosition = BossRigidbody.position;
            float distanceToEdge = float.PositiveInfinity;

            if (Mathf.Abs(dashDirection.x) > 0.001f)
            {
                float edgeX = dashDirection.x > 0f ? bounds.max.x - boundaryStopPadding : bounds.min.x + boundaryStopPadding;
                distanceToEdge = Mathf.Min(distanceToEdge, Mathf.Abs((edgeX - currentPosition.x) / dashDirection.x));
            }

            if (Mathf.Abs(dashDirection.y) > 0.001f)
            {
                float edgeY = dashDirection.y > 0f ? bounds.max.y - boundaryStopPadding : bounds.min.y + boundaryStopPadding;
                distanceToEdge = Mathf.Min(distanceToEdge, Mathf.Abs((edgeY - currentPosition.y) / dashDirection.y));
            }

            if (float.IsInfinity(distanceToEdge))
                return dashDuration;

            float durationToEdge = distanceToEdge / dashSpeedValue;
            return Mathf.Clamp(durationToEdge, dashDuration, maximumMapEdgeDashTime);
        }

        private float GetTelegraphDistance(float effectiveDashDuration)
        {
            float estimatedDistance = GetEffectiveDashSpeed() * effectiveDashDuration;
            return Mathf.Clamp(estimatedDistance, 0.1f, telegraphLength);
        }

        private Vector2 ClampDashPosition(Vector2 position)
        {
            if (!MapBoundary.TryGetWorldBounds(out Bounds bounds))
                return position;

            return new Vector2(
                Mathf.Clamp(position.x, bounds.min.x + boundaryStopPadding, bounds.max.x - boundaryStopPadding),
                Mathf.Clamp(position.y, bounds.min.y + boundaryStopPadding, bounds.max.y - boundaryStopPadding));
        }

        private static bool ShouldUseMobilePortraitTuning()
        {
            return GameOptions.IsMobileDisplayMode && Screen.height > Screen.width;
        }

        private void CacheHitboxComponents()
        {
            if (contactDamage == null)
                contactDamage = GetComponent<EnemyContactDamage>();

            if (bossCollider == null)
                bossCollider = GetComponent<Collider2D>();

            if (playerHealth == null && Player != null)
                playerHealth = Player.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        private float GetEffectiveDashHitRadius()
        {
            float radius = dashHitRadius;

            if (bossCollider != null)
            {
                Bounds bounds = bossCollider.bounds;
                float bodyRadius = Mathf.Max(bounds.extents.x, bounds.extents.y) * 0.72f;
                radius = Mathf.Max(radius, bodyRadius);
            }

            return radius;
        }

        private float GetEffectiveDashHitForwardPadding()
        {
            if (dashHitForwardPadding > 0f)
                return dashHitForwardPadding;

            return GetEffectiveDashHitRadius();
        }

        private float GetEffectiveDashHitPerpendicularPadding()
        {
            if (dashHitPerpendicularPadding > 0f)
                return dashHitPerpendicularPadding;

            return GetEffectiveDashHitRadius();
        }

        private Vector2 GetDashHitboxCenterOffset()
        {
            if (bossCollider == null || BossRigidbody == null)
                return Vector2.zero;

            return (Vector2)bossCollider.bounds.center - BossRigidbody.position;
        }

        private void SetContactDamageEnabled(bool isEnabled)
        {
            if (!disableContactDamageDuringDash)
                return;

            CacheHitboxComponents();

            if (contactDamage != null)
                contactDamage.enabled = isEnabled;
        }
    }
}
