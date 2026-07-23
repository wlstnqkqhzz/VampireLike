using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 중심에서 확장되는 원형 충격파를 발생시키는 패턴이다.
    /// </summary>
    public class ShockwavePattern : BossPattern
    {
        [SerializeField]
        private GameObject shockwaveVisualPrefab;

        [SerializeField]
        private float prepareTime = 0.55f;

        [SerializeField]
        private float maxRadius = 1.6f;

        [SerializeField]
        private float phaseBonusRadius = 0.35f;

        [SerializeField]
        private float expandDuration = 0.45f;

        [SerializeField]
        private int damage = 2;

        [SerializeField]
        private float slowMultiplier = 0.65f;

        [SerializeField]
        private float slowDuration = 0.8f;

        [SerializeField]
        private LayerMask playerLayerMask = ~0;

        private readonly Collider2D[] hitResults = new Collider2D[4];
        private readonly HashSet<global::PlayerController> slowedPlayers = new HashSet<global::PlayerController>();

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);

            if (prepareTime > 0f)
                yield return new WaitForSeconds(prepareTime);

            yield return ExpandShockwave();
        }

        private IEnumerator ExpandShockwave()
        {
            Vector2 center = transform.position;
            float targetRadius = maxRadius + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusRadius;
            float elapsedTime = 0f;
            bool hasHitPlayer = false;
            GameObject visual = CreateVisual(center);

            while (elapsedTime < expandDuration && !Boss.IsDead)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / expandDuration);
                float currentRadius = Mathf.Lerp(0.1f, targetRadius, progress);

                if (visual != null)
                    visual.transform.localScale = Vector3.one * currentRadius * 2f;

                if (!hasHitPlayer && TryDamagePlayer(center, currentRadius))
                    hasHitPlayer = true;

                yield return null;
            }

            if (visual != null)
                Destroy(visual);
        }

        private GameObject CreateVisual(Vector2 center)
        {
            if (shockwaveVisualPrefab == null)
                return null;

            return Instantiate(shockwaveVisualPrefab, center, Quaternion.identity);
        }

        private bool TryDamagePlayer(Vector2 center, float radius)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(center, radius, hitResults, playerLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                PlayerHealth playerHealth = hitResults[i].GetComponentInParent<PlayerHealth>();

                if (playerHealth == null)
                    continue;

                playerHealth.TakeDamage(damage);
                global::PlayerController playerController = hitResults[i].GetComponentInParent<global::PlayerController>();

                if (playerController != null)
                    StartCoroutine(ApplySlow(playerController));

                return true;
            }

            return false;
        }

        private IEnumerator ApplySlow(global::PlayerController playerController)
        {
            slowedPlayers.Add(playerController);
            playerController.AddMoveSpeedMultiplier(this, slowMultiplier);
            yield return new WaitForSeconds(slowDuration);
            playerController.RemoveMoveSpeedMultiplier(this);
            slowedPlayers.Remove(playerController);
        }

        private void OnDisable()
        {
            foreach (global::PlayerController playerController in slowedPlayers)
            {
                if (playerController != null)
                    playerController.RemoveMoveSpeedMultiplier(this);
            }

            slowedPlayers.Clear();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            maxRadius = Mathf.Max(0.1f, maxRadius);
            phaseBonusRadius = Mathf.Max(0f, phaseBonusRadius);
            expandDuration = Mathf.Max(0.05f, expandDuration);
            damage = Mathf.Max(1, damage);
            slowMultiplier = Mathf.Clamp(slowMultiplier, 0.25f, 1f);
            slowDuration = Mathf.Max(0f, slowDuration);
        }
    }
}
