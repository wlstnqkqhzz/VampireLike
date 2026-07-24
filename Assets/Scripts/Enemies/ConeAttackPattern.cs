using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어 방향을 저장한 뒤 전방 부채꼴 범위에 피해를 주는 공통 근접 패턴이다.
    /// </summary>
    public class ConeAttackPattern : BossPattern
    {
        [SerializeField]
        private float prepareTime = 0.65f;

        [SerializeField]
        private float range = 1.8f;

        [SerializeField]
        private float angle = 70f;

        [SerializeField]
        private int damage = 2;

        [SerializeField]
        private float endLag = 0.2f;

        [SerializeField]
        private LayerMask playerLayerMask = 1 << 6;

        [SerializeField]
        private GameObject warningPrefab;

        [SerializeField]
        private GameObject impactPrefab;

        [SerializeField]
        private float effectLifetime = 0.25f;

        private readonly Collider2D[] hitResults = new Collider2D[8];
        private GameObject activeWarning;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 direction = ((Vector2)Player.position - (Vector2)transform.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector2.down;

            activeWarning = SpawnEffect(warningPrefab, direction);
            yield return new WaitForSeconds(prepareTime);
            DestroyActiveWarning();

            if (!Boss.IsDead)
            {
                SpawnEffect(impactPrefab, direction, true);
                ApplyDamage(direction);
            }

            yield return new WaitForSeconds(endLag);
        }

        private void OnDisable()
        {
            DestroyActiveWarning();
        }

        private GameObject SpawnEffect(GameObject prefab, Vector2 direction, bool autoDestroy = false)
        {
            if (prefab == null)
                return null;

            Vector2 effectPosition = (Vector2)transform.position + direction * range * 0.5f;
            GameObject effect = Instantiate(prefab, effectPosition, Quaternion.FromToRotation(Vector3.right, direction));
            effect.transform.localScale = Vector3.one * range;

            if (autoDestroy)
                Destroy(effect, effectLifetime);

            return effect;
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

        private void DestroyActiveWarning()
        {
            if (activeWarning == null)
                return;

            Destroy(activeWarning);
            activeWarning = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            range = Mathf.Max(0.1f, range);
            angle = Mathf.Clamp(angle, 1f, 180f);
            damage = Mathf.Max(1, damage);
            endLag = Mathf.Max(0f, endLag);
            effectLifetime = Mathf.Max(0.05f, effectLifetime);
        }
    }
}
