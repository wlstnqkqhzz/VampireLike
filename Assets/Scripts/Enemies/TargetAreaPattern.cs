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
        [SerializeField]
        private float warningDuration = 0.9f;

        [SerializeField]
        private float radius = 0.9f;

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
        private GameObject activeWarning;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 targetPosition = Player.position;
            activeWarning = CreateWarning(targetPosition);

            yield return new WaitForSeconds(warningDuration);

            if (!Boss.IsDead)
            {
                SpawnImpact(targetPosition);
                ApplyDamage(targetPosition);
            }

            DestroyActiveWarning();
        }

        private void OnDisable()
        {
            DestroyActiveWarning();
        }

        private GameObject CreateWarning(Vector2 position)
        {
            if (warningPrefab == null)
                return CreateWarningCircle(position);

            GameObject warning = Instantiate(warningPrefab, position, Quaternion.identity);
            ScaleEffectToRadius(warning);
            return warning;
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

        private void DestroyActiveWarning()
        {
            if (activeWarning == null)
                return;

            LineRenderer lineRenderer = activeWarning.GetComponent<LineRenderer>();

            if (lineRenderer != null && lineRenderer.material != null)
                Destroy(lineRenderer.material);

            Destroy(activeWarning);
            activeWarning = null;
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
            damage = Mathf.Max(1, damage);
            impactLifetime = Mathf.Max(0.05f, impactLifetime);
        }
    }
}
