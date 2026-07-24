using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스를 잠복 상태로 숨긴 뒤 플레이어 주변에서 다시 등장시키는 샌드웜용 패턴이다.
    /// </summary>
    public class BurrowPattern : BossPattern
    {
        [SerializeField]
        private float burrowDelay = 0.45f;

        [SerializeField]
        private float undergroundDuration = 0.9f;

        [SerializeField]
        private float phaseUndergroundDurationReduction = 0.18f;

        [SerializeField]
        private int burrowCount = 1;

        [SerializeField]
        private int phase3BonusBurrowCount = 1;

        [SerializeField]
        private float repeatDelay = 0.45f;

        [SerializeField]
        private float warningDuration = 0.55f;

        [SerializeField]
        private float reappearDistance = 1.4f;

        [SerializeField]
        private GameObject warningPrefab;

        [SerializeField]
        private bool disableColliderWhileBurrowed = true;

        [SerializeField]
        private bool disableContactDamageWhileBurrowed = true;

        private SpriteRenderer[] spriteRenderers;
        private Collider2D bossCollider;
        private EnemyContactDamage contactDamage;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null || BossRigidbody == null)
                yield break;

            CacheComponents();
            int count = burrowCount + (Boss.CurrentPhase >= 3 ? phase3BonusBurrowCount : 0);

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                Boss.SetState(BossState.Burrowed, false);
                yield return new WaitForSeconds(burrowDelay);

                SetBurrowedVisualState(true);
                yield return new WaitForSeconds(GetCurrentUndergroundDuration());

                Vector2 reappearPosition = GetReappearPosition();
                GameObject warning = CreateWarning(reappearPosition);
                yield return new WaitForSeconds(warningDuration);

                if (warning != null)
                    Destroy(warning);

                BossRigidbody.position = reappearPosition;
                SetBurrowedVisualState(false);

                if (i < count - 1)
                    yield return new WaitForSeconds(repeatDelay);
            }
        }

        private void OnDisable()
        {
            SetBurrowedVisualState(false);
        }

        private void CacheComponents()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
                spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            if (bossCollider == null)
                bossCollider = GetComponent<Collider2D>();

            if (contactDamage == null)
                contactDamage = GetComponent<EnemyContactDamage>();
        }

        private Vector2 GetReappearPosition()
        {
            Vector2 bossPosition = transform.position;
            Vector2 playerPosition = Player.position;
            Vector2 fromPlayer = (bossPosition - playerPosition).normalized;

            if (fromPlayer.sqrMagnitude <= 0.001f)
                fromPlayer = Random.insideUnitCircle.normalized;

            return playerPosition + fromPlayer * reappearDistance;
        }

        private float GetCurrentUndergroundDuration()
        {
            float reduction = Mathf.Max(0, Boss.CurrentPhase - 1) * phaseUndergroundDurationReduction;
            return Mathf.Max(0.2f, undergroundDuration - reduction);
        }

        private GameObject CreateWarning(Vector2 position)
        {
            if (warningPrefab == null)
                return null;

            return Instantiate(warningPrefab, position, Quaternion.identity);
        }

        private void SetBurrowedVisualState(bool isBurrowed)
        {
            CacheComponents();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                    spriteRenderer.enabled = !isBurrowed;
            }

            if (disableColliderWhileBurrowed && bossCollider != null)
                bossCollider.enabled = !isBurrowed;

            if (disableContactDamageWhileBurrowed && contactDamage != null)
                contactDamage.enabled = !isBurrowed;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            burrowDelay = Mathf.Max(0f, burrowDelay);
            undergroundDuration = Mathf.Max(0f, undergroundDuration);
            phaseUndergroundDurationReduction = Mathf.Max(0f, phaseUndergroundDurationReduction);
            burrowCount = Mathf.Max(1, burrowCount);
            phase3BonusBurrowCount = Mathf.Max(0, phase3BonusBurrowCount);
            repeatDelay = Mathf.Max(0f, repeatDelay);
            warningDuration = Mathf.Max(0f, warningDuration);
            reappearDistance = Mathf.Max(0f, reappearDistance);
        }
    }
}
