using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 잠시 사라진 뒤 플레이어 주변으로 이동하는 공통 순간이동 패턴이다.
    /// </summary>
    public class TeleportPattern : BossPattern
    {
        [SerializeField]
        private float vanishDelay = 0.45f;

        [SerializeField]
        private float reappearDelay = 0.25f;

        [SerializeField]
        private int teleportCount = 1;

        [SerializeField]
        private int phaseBonusTeleportCount = 1;

        [SerializeField]
        private int maxTeleportCount = 2;

        [SerializeField]
        private float minDistanceFromPlayer = 1.1f;

        [SerializeField]
        private float maxDistanceFromPlayer = 2.2f;

        [SerializeField]
        private LayerMask obstacleLayerMask;

        [SerializeField]
        private bool disableContactDamageDuringTeleport = true;

        private SpriteRenderer[] spriteRenderers;
        private EnemyContactDamage contactDamage;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null || BossRigidbody == null)
                yield break;

            CacheComponents();
            int count = Mathf.Min(maxTeleportCount, teleportCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusTeleportCount);

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                Boss.SetState(BossState.Teleporting, false);
                SetVisible(false);
                SetContactDamageEnabled(false);

                yield return new WaitForSeconds(vanishDelay);

                BossRigidbody.position = FindTeleportPosition();

                yield return new WaitForSeconds(reappearDelay);

                SetVisible(true);
                SetContactDamageEnabled(true);

                if (i < count - 1)
                    yield return new WaitForSeconds(reappearDelay);
            }
        }

        private void OnDisable()
        {
            SetVisible(true);
            SetContactDamageEnabled(true);
        }

        private void CacheComponents()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
                spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            if (contactDamage == null)
                contactDamage = GetComponent<EnemyContactDamage>();
        }

        private Vector2 FindTeleportPosition()
        {
            Vector2 playerPosition = Player.position;

            for (int i = 0; i < 12; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
                Vector2 position = playerPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (Physics2D.OverlapCircle(position, 0.25f, obstacleLayerMask) == null)
                    return position;
            }

            Vector2 fallbackDirection = ((Vector2)transform.position - playerPosition).normalized;
            return playerPosition + fallbackDirection * minDistanceFromPlayer;
        }

        private void SetVisible(bool isVisible)
        {
            CacheComponents();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                    spriteRenderer.enabled = isVisible;
            }
        }

        private void SetContactDamageEnabled(bool isEnabled)
        {
            if (!disableContactDamageDuringTeleport)
                return;

            CacheComponents();

            if (contactDamage != null)
                contactDamage.enabled = isEnabled;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            vanishDelay = Mathf.Max(0f, vanishDelay);
            reappearDelay = Mathf.Max(0f, reappearDelay);
            teleportCount = Mathf.Max(1, teleportCount);
            phaseBonusTeleportCount = Mathf.Max(0, phaseBonusTeleportCount);
            maxTeleportCount = Mathf.Max(teleportCount, maxTeleportCount);
            minDistanceFromPlayer = Mathf.Max(0f, minDistanceFromPlayer);
            maxDistanceFromPlayer = Mathf.Max(minDistanceFromPlayer, maxDistanceFromPlayer);
        }
    }
}
