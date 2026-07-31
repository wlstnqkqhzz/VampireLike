using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어 주변 지속 피해 특수 강화의 어둠/월식 오라 시각 효과를 담당한다.
    /// </summary>
    public class EclipseAuraVFX : MonoBehaviour
    {
        private Transform followTarget;
        private SpriteRenderer fillRenderer;
        private SpriteRenderer outerRingRenderer;
        private SpriteRenderer innerRingRenderer;
        private SpriteRenderer glyphRenderer;
        private Color baseColor;
        private float radius = 1f;
        private float pulseTimer;
        private float damagePulse;

        public void Initialize(Transform target, float initialRadius, Color color)
        {
            followTarget = target;
            baseColor = color;

            CreateRenderers();
            SetRadius(initialRadius);
            transform.position = followTarget == null ? transform.position : followTarget.position;
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.3f, value);
            transform.localScale = Vector3.one * radius * 2f;
        }

        public void PlayDamagePulse()
        {
            damagePulse = 1f;
        }

        public void StopAura()
        {
            Destroy(gameObject);
        }

        private void LateUpdate()
        {
            if (followTarget == null || GameState.IsGameOver)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = followTarget.position;

            if (Time.timeScale <= 0f)
                return;

            pulseTimer += Time.deltaTime;
            damagePulse = Mathf.MoveTowards(damagePulse, 0f, Time.deltaTime * 3.8f);

            float breathe = 1f + Mathf.Sin(pulseTimer * 2.1f) * 0.035f + damagePulse * 0.08f;
            transform.localScale = Vector3.one * radius * 2f * breathe;

            if (outerRingRenderer != null)
                outerRingRenderer.transform.Rotate(0f, 0f, 10f * Time.deltaTime);

            if (innerRingRenderer != null)
                innerRingRenderer.transform.Rotate(0f, 0f, -18f * Time.deltaTime);

            if (glyphRenderer != null)
                glyphRenderer.transform.Rotate(0f, 0f, 24f * Time.deltaTime);

            ApplyColors();
        }

        private void CreateRenderers()
        {
            fillRenderer = CreateRenderer("Eclipse Fill", SpecialUpgradePulse.GetFilledCircleSprite(), 6);
            outerRingRenderer = CreateRenderer("Eclipse Edge", SpecialUpgradePulse.GetCircleSprite(), 7);
            innerRingRenderer = CreateRenderer("Eclipse Inner Ring", SpecialUpgradePulse.GetCircleSprite(), 8);
            glyphRenderer = CreateRenderer("Eclipse Glyph", SpecialUpgradePulse.GetStarSprite(), 9);

            fillRenderer.transform.localScale = Vector3.one * 0.98f;
            outerRingRenderer.transform.localScale = Vector3.one * 1.02f;
            innerRingRenderer.transform.localScale = Vector3.one * 0.72f;
            glyphRenderer.transform.localScale = Vector3.one * 0.58f;
        }

        private SpriteRenderer CreateRenderer(string objectName, Sprite sprite, int sortingOrder)
        {
            GameObject child = new GameObject(objectName);
            child.transform.SetParent(transform, false);
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void ApplyColors()
        {
            float edgePulse = 0.82f + Mathf.Sin(pulseTimer * 3.2f) * 0.12f + damagePulse * 0.42f;
            Color darkFill = new Color(0.03f, 0.01f, 0.06f, 0.16f + damagePulse * 0.08f);
            Color main = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(baseColor.a * 0.32f));
            Color edge = new Color(0.74f, 0.42f, 1f, Mathf.Clamp01(0.34f * edgePulse));
            Color inner = new Color(0.32f, 0.12f, 0.58f, Mathf.Clamp01(0.18f + damagePulse * 0.16f));
            Color glyph = new Color(0.94f, 0.78f, 1f, Mathf.Clamp01(0.08f + damagePulse * 0.22f));

            if (fillRenderer != null)
                fillRenderer.color = Color.Lerp(darkFill, main, 0.35f);

            if (outerRingRenderer != null)
                outerRingRenderer.color = edge;

            if (innerRingRenderer != null)
                innerRingRenderer.color = inner;

            if (glyphRenderer != null)
                glyphRenderer.color = glyph;
        }
    }
}
