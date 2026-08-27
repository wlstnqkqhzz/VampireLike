using System.Collections;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어 방향을 고정한 뒤 전방으로 화염을 세 번 연속 분출한다.
    /// </summary>
    public class TripleFlameBurstPattern : BossPattern, IBossDamageScaler
    {
        [SerializeField]
        private float prepareTime = 0.45f;

        [SerializeField]
        private int burstCount = 3;

        [SerializeField]
        private float burstInterval = 0.22f;

        [SerializeField]
        private float range = 2.8f;

        [SerializeField]
        private float angle = 42f;

        [SerializeField]
        private int damage = 2;

        [SerializeField]
        private LayerMask playerLayerMask = 1 << 6;

        [SerializeField]
        private float warningLifetime = 0.16f;

        [SerializeField]
        private float impactLifetime = 0.2f;

        [SerializeField]
        private float effectCameraPadding = 0.7f;

        private readonly Collider2D[] hitResults = new Collider2D[8];

        protected override bool UseSkillAnimation => true;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 direction = GetDirectionToPlayer();
            Boss.FaceDirection(direction);
            CombatVFX.PlayBossCastAura(transform, CombatVFXKind.FireZone, 0.78f, prepareTime, 1500);

            if (prepareTime > 0f)
                yield return new WaitForSeconds(prepareTime);

            Boss.SetState(BossState.Attacking, false);

            int count = Mathf.Max(1, burstCount);
            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                SpawnCone(direction, CombatVFXKind.ConeWarning, warningLifetime, false);

                if (warningLifetime > 0f)
                    yield return new WaitForSeconds(warningLifetime);

                GameSfx.Play(SfxType.BossZone);
                SpawnCone(direction, CombatVFXKind.FireZone, impactLifetime, true);
                CombatVFX.PlayDirectionalStreak(transform.position, direction, CombatVFXKind.ConeImpact, range * 0.58f, 0.16f, 0.14f, 1650);
                ApplyDamage(direction);

                if (i < count - 1 && burstInterval > 0f)
                    yield return new WaitForSeconds(burstInterval);
            }
        }

        private Vector2 GetDirectionToPlayer()
        {
            Vector2 direction = ((Vector2)Player.position - (Vector2)transform.position).normalized;
            return direction.sqrMagnitude <= 0.001f ? Vector2.down : direction;
        }

        private void SpawnCone(Vector2 direction, CombatVFXKind kind, float lifetime, bool autoDestroy)
        {
            Vector2 effectPosition = (Vector2)transform.position + direction * range * 0.5f;
            effectPosition = ClampEffectToCamera(effectPosition);
            CombatVFX.PlayCone(effectPosition, direction, kind, range, autoDestroy, lifetime, autoDestroy ? 16 : 11);
        }

        private Vector2 ClampEffectToCamera(Vector2 position)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null || !mainCamera.orthographic)
                return position;

            Vector2 center = mainCamera.transform.position;
            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;
            float padding = Mathf.Max(0f, effectCameraPadding);
            float minX = center.x - halfWidth + padding;
            float maxX = center.x + halfWidth - padding;
            float minY = center.y - halfHeight + padding;
            float maxY = center.y + halfHeight - padding;

            if (minX > maxX || minY > maxY)
                return position;

            return new Vector2(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY));
        }

        private void ApplyDamage(Vector2 direction)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, range, hitResults, playerLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                PlayerHealth playerHealth = hitResults[i].GetComponentInParent<PlayerHealth>();

                if (playerHealth == null)
                    continue;

                Vector2 toPlayer = ((Vector2)playerHealth.transform.position - (Vector2)transform.position).normalized;

                if (Vector2.Angle(direction, toPlayer) > angle * 0.5f)
                    continue;

                playerHealth.TakeDamage(damage);
                return;
            }
        }

        public void ScaleBossDamage(float multiplier)
        {
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * Mathf.Max(0.1f, multiplier)));
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            burstCount = Mathf.Max(1, burstCount);
            burstInterval = Mathf.Max(0f, burstInterval);
            range = Mathf.Max(0.1f, range);
            angle = Mathf.Clamp(angle, 1f, 180f);
            damage = Mathf.Max(1, damage);
            warningLifetime = Mathf.Max(0.02f, warningLifetime);
            impactLifetime = Mathf.Max(0.05f, impactLifetime);
            effectCameraPadding = Mathf.Max(0f, effectCameraPadding);
        }
    }
}
