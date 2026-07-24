using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어의 현재 위치를 겨냥해 경고 원을 표시한 뒤 범위 피해를 주는 패턴이다.
    /// </summary>
    public class TargetAreaPattern : BossPattern
    {
        protected override bool UseSkillAnimation => true;

        [SerializeField]
        private float warningDuration = 0.9f;

        [SerializeField]
        private float radius = 0.9f;

        [SerializeField]
        private int targetAreaCount = 1;

        [SerializeField]
        private int phaseBonusTargetAreaCount;

        [SerializeField]
        private float targetAreaInterval = 0.18f;

        [SerializeField]
        private float minimumTargetDistance = 0.6f;

        [SerializeField]
        private int damage = 2;

        [SerializeField]
        private GameObject warningPrefab;

        [SerializeField]
        private GameObject impactPrefab;

        [SerializeField]
        private float impactLifetime = 0.35f;

        [SerializeField]
        private Color warningColor = new Color(1f, 0.1f, 0.05f, 0.35f);

        [SerializeField]
        private LayerMask playerLayerMask = ~0;

        private readonly Collider2D[] hitResults = new Collider2D[4];
        private readonly System.Collections.Generic.List<GameObject> activeWarnings = new System.Collections.Generic.List<GameObject>();

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2[] targetPositions = GetTargetPositions();

            foreach (Vector2 targetPosition in targetPositions)
            {
                GameObject warning = CreateWarning(targetPosition);

                if (warning != null)
                    activeWarnings.Add(warning);
            }

            yield return new WaitForSeconds(warningDuration);

            if (!Boss.IsDead)
            {
                for (int i = 0; i < targetPositions.Length && !Boss.IsDead; i++)
                {
                    SpawnImpact(targetPositions[i]);
                    ApplyDamage(targetPositions[i]);

                    if (targetAreaInterval > 0f && i < targetPositions.Length - 1)
                        yield return new WaitForSeconds(targetAreaInterval);
                }
            }

            DestroyActiveWarnings();
        }

        private void OnDisable()
        {
            DestroyActiveWarnings();
        }

        private GameObject CreateWarning(Vector2 position)
        {
            if (warningPrefab == null)
                return CreateWarningCircle(position);

            GameObject warning = Instantiate(warningPrefab, position, Quaternion.identity);
            ScaleEffectToRadius(warning);
            return warning;
        }

        private Vector2[] GetTargetPositions()
        {
            int count = Mathf.Max(1, targetAreaCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusTargetAreaCount);
            Vector2[] positions = new Vector2[count];
            positions[0] = Player.position;

            for (int i = 1; i < count; i++)
                positions[i] = GetSeparatedPosition(positions, i);

            return positions;
        }

        private Vector2 GetSeparatedPosition(Vector2[] positions, int filledCount)
        {
            Vector2 center = Player.position;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector2 candidate = center + Random.insideUnitCircle * radius * 1.8f;
                bool isFarEnough = true;

                for (int i = 0; i < filledCount; i++)
                {
                    if (Vector2.Distance(candidate, positions[i]) < minimumTargetDistance)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                    return candidate;
            }

            return center + Random.insideUnitCircle * minimumTargetDistance;
        }

        private void SpawnImpact(Vector2 position)
        {
            if (impactPrefab == null)
                return;

            GameObject impact = Instantiate(impactPrefab, position, Quaternion.identity);
            ScaleEffectToRadius(impact);
            Destroy(impact, impactLifetime);
        }

        private void ScaleEffectToRadius(GameObject effect)
        {
            const float fallbackSpriteSize = 1f;
            float targetDiameter = radius * 2f;
            float spriteSize = fallbackSpriteSize;
            SpriteRenderer spriteRenderer = effect.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.sprite != null)
                spriteSize = Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);

            effect.transform.localScale = Vector3.one * (targetDiameter / Mathf.Max(0.01f, spriteSize));
        }

        private GameObject CreateWarningCircle(Vector2 position)
        {
            GameObject warning = new GameObject("Boss Target Area Warning");
            warning.transform.position = position;

            LineRenderer lineRenderer = warning.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = 48;
            lineRenderer.startWidth = 0.04f;
            lineRenderer.endWidth = 0.04f;
            lineRenderer.sortingOrder = 12;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = warningColor;
            lineRenderer.endColor = warningColor;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float angle = Mathf.PI * 2f * i / lineRenderer.positionCount;
                lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            return warning;
        }

        private void DestroyActiveWarnings()
        {
            for (int i = activeWarnings.Count - 1; i >= 0; i--)
            {
                GameObject warning = activeWarnings[i];

                if (warning == null)
                    continue;

                LineRenderer lineRenderer = warning.GetComponent<LineRenderer>();

                if (lineRenderer != null && lineRenderer.material != null)
                    Destroy(lineRenderer.material);

                Destroy(warning);
            }

            activeWarnings.Clear();
        }

        private void ApplyDamage(Vector2 position)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(position, radius, hitResults, playerLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                PlayerHealth playerHealth = hitResults[i].GetComponentInParent<PlayerHealth>();

                if (playerHealth == null)
                    continue;

                playerHealth.TakeDamage(damage);
                return;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            warningDuration = Mathf.Max(0f, warningDuration);
            radius = Mathf.Max(0.05f, radius);
            targetAreaCount = Mathf.Max(1, targetAreaCount);
            phaseBonusTargetAreaCount = Mathf.Max(0, phaseBonusTargetAreaCount);
            targetAreaInterval = Mathf.Max(0f, targetAreaInterval);
            minimumTargetDistance = Mathf.Max(0f, minimumTargetDistance);
            damage = Mathf.Max(1, damage);
            impactLifetime = Mathf.Max(0.05f, impactLifetime);
        }
    }
}
