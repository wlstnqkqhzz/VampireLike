using System.Collections;
using UnityEngine;
using VampireLike.Combat;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 일정 시간 동안 한 방향으로 유지되는 부채꼴 반복 피해 패턴이다.
    /// </summary>
    public class BreathPattern : BossPattern
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

        private readonly Collider2D[] hitResults = new Collider2D[8];

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 direction = ((Vector2)Player.position - (Vector2)transform.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector2.down;

            GameObject warning = SpawnEffect(warningPrefab, direction, false);
            yield return new WaitForSeconds(prepareTime);

            if (warning != null)
                Destroy(warning);

            GameObject breath = SpawnEffect(breathPrefab, direction, true);
            Boss.SetState(BossState.Attacking, false);

            float elapsedTime = 0f;
            float nextDamageTime = 0f;

            while (elapsedTime < duration && !Boss.IsDead)
            {
                if (elapsedTime >= nextDamageTime)
                {
                    ApplyDamage(direction);
                    nextDamageTime = elapsedTime + damageInterval;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (breath != null)
                Destroy(breath);
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

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            duration = Mathf.Max(0f, duration);
            range = Mathf.Max(0.1f, range);
            angle = Mathf.Clamp(angle, 1f, 180f);
            damagePerTick = Mathf.Max(1, damagePerTick);
            damageInterval = Mathf.Max(0.05f, damageInterval);
        }
    }
}
