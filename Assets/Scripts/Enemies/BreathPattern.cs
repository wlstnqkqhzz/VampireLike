using System.Collections;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.VFX;
using VampireLike.World;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 일정 시간 동안 한 방향으로 유지되는 부채꼴 반복 피해 패턴이다.
    /// </summary>
    public class BreathPattern : BossPattern, IBossDamageScaler
    {
        [SerializeField]
        private float prepareTime = 0.75f;

        [SerializeField]
        private float duration = 1.4f;

        [SerializeField]
        private float range = 2.3f;

        [SerializeField]
        private float angle = 55f;

        [SerializeField]
        private int damagePerTick = 1;

        [SerializeField]
        private float damageInterval = 0.35f;

        [SerializeField]
        private LayerMask playerLayerMask = 1 << 6;

        [SerializeField]
        private GameObject warningPrefab;

        [SerializeField]
        private GameObject breathPrefab;

        [SerializeField]
        private bool moveDuringBreath;

        [SerializeField]
        private float breathMoveSpeed = 1.6f;

        [SerializeField]
        private bool trackPlayerDuringBreath = true;

        [SerializeField]
        private float breathTurnSpeed = 120f;

        private readonly Collider2D[] hitResults = new Collider2D[8];
        private GameObject activeWarning;
        private GameObject activeBreath;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 direction = ((Vector2)Player.position - (Vector2)transform.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector2.down;

            CombatVFX.PlayBossCastAura(transform, CombatVFXKind.FireZone, 0.86f, prepareTime, 1500);
            activeWarning = SpawnEffect(warningPrefab, direction, false);
            yield return new WaitForSeconds(prepareTime);

            DestroyActiveEffects(false);

            activeBreath = SpawnEffect(breathPrefab, direction, true);
            Boss.SetState(BossState.Attacking, false);
            GameSfx.Play(SfxType.BossZone);
            CombatVFX.PlayDirectionalStreak(transform.position, direction, CombatVFXKind.ConeImpact, range * 0.7f, 0.22f, 0.18f, 1650);

            float elapsedTime = 0f;
            float nextDamageTime = 0f;

            while (elapsedTime < duration && !Boss.IsDead)
            {
                direction = UpdateBreathMovement(direction);
                UpdateActiveBreathPosition(direction);

                if (elapsedTime >= nextDamageTime)
                {
                    ApplyDamage(direction);
                    nextDamageTime = elapsedTime + damageInterval;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            DestroyActiveEffects(true);
        }

        private void OnDisable()
        {
            DestroyActiveEffects(true);
        }

        protected override void OnPatternCancelled()
        {
            DestroyActiveEffects(true);
        }

        private GameObject SpawnEffect(GameObject prefab, Vector2 direction, bool isBreath)
        {
            Vector2 effectPosition = (Vector2)transform.position + direction * range * 0.5f;
            return CombatVFX.PlayCone(
                effectPosition,
                direction,
                isBreath ? CombatVFXKind.FireZone : CombatVFXKind.ConeWarning,
                range,
                isBreath,
                isBreath ? duration : prepareTime,
                isBreath ? 15 : 12);
        }

        private Vector2 UpdateBreathMovement(Vector2 direction)
        {
            if (!moveDuringBreath || BossRigidbody == null || breathMoveSpeed <= 0f)
                return direction;

            Vector2 nextDirection = direction;

            if (trackPlayerDuringBreath && Player != null)
            {
                Vector2 toPlayer = ((Vector2)Player.position - BossRigidbody.position).normalized;

                if (toPlayer.sqrMagnitude > 0.001f)
                    nextDirection = Vector2.MoveTowards(direction, toPlayer, breathTurnSpeed * Mathf.Deg2Rad * Time.deltaTime).normalized;
            }

            Vector2 nextPosition = BossRigidbody.position + nextDirection * breathMoveSpeed * Time.deltaTime;
            BossRigidbody.MovePosition(MapBoundary.ClampToPlayableArea(nextPosition));
            Boss.FaceDirection(nextDirection);
            return nextDirection;
        }

        private void UpdateActiveBreathPosition(Vector2 direction)
        {
            if (activeBreath == null)
                return;

            activeBreath.transform.position = (Vector2)transform.position + direction * range * 0.5f;
            float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            activeBreath.transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);
        }

        private void DestroyActiveEffects(bool includeBreath)
        {
            if (activeWarning != null)
            {
                Destroy(activeWarning);
                activeWarning = null;
            }

            if (!includeBreath || activeBreath == null)
                return;

            Destroy(activeBreath);
            activeBreath = null;
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

                playerHealth.TakeDamage(damagePerTick);
                return;
            }
        }

        public void ScaleBossDamage(float multiplier)
        {
            damagePerTick = Mathf.Max(1, Mathf.RoundToInt(damagePerTick * Mathf.Max(0.1f, multiplier)));
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            duration = Mathf.Max(0f, duration);
            range = Mathf.Max(0.1f, range);
            angle = Mathf.Clamp(angle, 1f, 180f);
            damagePerTick = Mathf.Max(1, damagePerTick);
            damageInterval = Mathf.Max(0.05f, damageInterval);
            breathMoveSpeed = Mathf.Max(0f, breathMoveSpeed);
            breathTurnSpeed = Mathf.Max(0f, breathTurnSpeed);
        }
    }
}
