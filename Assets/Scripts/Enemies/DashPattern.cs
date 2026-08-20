using System.Collections;
using UnityEngine;
using VampireLike.Audio;
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

            Boss.SetState(BossState.Preparing, false);
            Vector2 dashDirection = ((Vector2)Player.position - BossRigidbody.position).normalized;

            if (dashDirection.sqrMagnitude <= 0.001f)
                dashDirection = Vector2.down;

            Boss.FaceDirection(dashDirection);
            Boss.ShowAttackFrame(0);
            CombatVFX.PlayBossCastAura(transform, CombatVFXKind.TargetWarning, 0.9f, GetEffectivePrepareTime(), 1500);

            yield return new WaitForSeconds(GetEffectivePrepareTime());

            Boss.SetState(BossState.Attacking, false);
            Boss.FaceDirection(dashDirection);
            Boss.ShowAttackFrame(1);
            GameSfx.Play(SfxType.BossDash);
            float elapsedTime = 0f;
            float effectiveDashDuration = GetEffectiveDashDuration(dashDirection);
            float nextTrailTime = 0f;

            while (elapsedTime < effectiveDashDuration && !Boss.IsDead)
            {
                Vector2 nextPosition = BossRigidbody.position + dashDirection * GetEffectiveDashSpeed() * Time.fixedDeltaTime;
                nextPosition = ClampDashPosition(nextPosition);
                BossRigidbody.MovePosition(nextPosition);

                if (elapsedTime >= nextTrailTime)
                {
                    CombatVFX.PlayDirectionalStreak(BossRigidbody.position - dashDirection * 0.18f, -dashDirection, CombatVFXKind.TargetImpact, 0.62f, 0.1f, 0.12f, 1400);
                    nextTrailTime = elapsedTime + 0.08f;
                }

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            BossRigidbody.linearVelocity = Vector2.zero;
            Boss.SetState(BossState.Recovering, false);
            yield return new WaitForSeconds(endLag);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            dashSpeed = Mathf.Max(0f, dashSpeed);
            dashDuration = Mathf.Max(0f, dashDuration);
            endLag = Mathf.Max(0f, endLag);
            minimumTriggerDistance = Mathf.Max(0f, minimumTriggerDistance);
            maximumTriggerDistance = Mathf.Max(minimumTriggerDistance, maximumTriggerDistance);
            mobilePortraitPrepareMultiplier = Mathf.Max(1f, mobilePortraitPrepareMultiplier);
            mobilePortraitDashSpeedMultiplier = Mathf.Clamp(mobilePortraitDashSpeedMultiplier, 0.6f, 1f);
            mobilePortraitMaximumTriggerDistance = Mathf.Max(minimumTriggerDistance, mobilePortraitMaximumTriggerDistance);
            boundaryStopPadding = Mathf.Max(0f, boundaryStopPadding);
            maximumMapEdgeDashTime = Mathf.Max(dashDuration, maximumMapEdgeDashTime);
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
    }
}
