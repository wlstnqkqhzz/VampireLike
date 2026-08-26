using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Audio;
using VampireLike.Combat;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 중심에서 확장되는 원형 충격파를 발생시키는 패턴이다.
    /// </summary>
    public class ShockwavePattern : BossPattern, IBossDamageScaler
    {
        protected override bool UseSkillAnimation => true;

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

        [Header("중심 보정")]
        [SerializeField]
        private bool useColliderCenter = true;

        [SerializeField]
        private Vector2 shockwaveCenterOffset = new Vector2(0f, -0.18f);

        [Header("경고/충격 연출")]
        [SerializeField]
        private Color telegraphColor = new Color(0.65f, 0.9f, 1f, 0.36f);

        [SerializeField]
        private int telegraphSortingOrder = 1480;

        [SerializeField]
        private float impactSizeMultiplier = 1f;

        private readonly Collider2D[] hitResults = new Collider2D[4];
        private readonly HashSet<global::PlayerController> slowedPlayers = new HashSet<global::PlayerController>();
        private Collider2D bossCollider;

        protected override IEnumerator ExecutePattern()
        {
            float targetRadius = GetTargetRadius();
            Vector2 center = GetShockwaveCenter();
            Boss.SetState(BossState.Preparing, false);
            CombatVFX.PlayBossCastAura(transform, center - (Vector2)transform.position, CombatVFXKind.Shockwave, 0.9f, prepareTime, 1500);
            BossTelegraph.ShowCircle(center, targetRadius, prepareTime, telegraphColor, telegraphSortingOrder);

            if (prepareTime > 0f)
                yield return new WaitForSeconds(prepareTime);

            yield return ExpandShockwave();
        }

        private IEnumerator ExpandShockwave()
        {
            Vector2 center = GetShockwaveCenter();
            float targetRadius = GetTargetRadius();
            float elapsedTime = 0f;
            bool hasHitPlayer = false;
            GameObject visual = CreateVisual(center);
            CombatVFX.PlayExpandingRing(center, CombatVFXKind.Shockwave, 0.18f, targetRadius * 2f, expandDuration, 1550);
            BossImpact.PlayShockwaveImpact(center, targetRadius * impactSizeMultiplier);
            GameSfx.Play(SfxType.BossZone);

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
                return CombatVFX.PlayWarning(center, CombatVFXKind.Shockwave, 0.4f, expandDuration, 13);

            CombatVFX.PlayBurst(center, CombatVFXKind.Shockwave, 0.55f, 0.22f, 14);
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

        private float GetTargetRadius()
        {
            return maxRadius + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusRadius;
        }

        private Vector2 GetShockwaveCenter()
        {
            Vector2 center = transform.position;

            if (useColliderCenter)
            {
                if (bossCollider == null)
                    bossCollider = GetComponent<Collider2D>();

                if (bossCollider != null)
                    center = bossCollider.bounds.center;
            }

            return center + (Vector2)transform.TransformVector(shockwaveCenterOffset);
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

        public void ScaleBossDamage(float multiplier)
        {
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * Mathf.Max(0.1f, multiplier)));
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
            shockwaveCenterOffset.x = Mathf.Clamp(shockwaveCenterOffset.x, -1f, 1f);
            shockwaveCenterOffset.y = Mathf.Clamp(shockwaveCenterOffset.y, -1f, 1f);
            telegraphSortingOrder = Mathf.Max(0, telegraphSortingOrder);
            impactSizeMultiplier = Mathf.Max(0.1f, impactSizeMultiplier);
        }
    }
}
