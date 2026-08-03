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
        private SpriteRenderer runeRenderer;
        private SpriteRenderer[] spiritRenderers;
        private Sprite[] runeFrames;
        private Sprite[] spiritFrames;
        private Color baseColor;
        private float radius = 1f;
        private float pulseTimer;
        private float damagePulse;
        private int animationFrame;
        private float animationTimer;

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

            AnimateImportedEffects();
            PositionSpiritParticles();
            ApplyColors();
        }

        private void CreateRenderers()
        {
            LoadImportedEffectFrames();

            fillRenderer = CreateRenderer("Eclipse Fill", SpecialUpgradePulse.GetFilledCircleSprite(), 6);
            outerRingRenderer = CreateRenderer("Eclipse Edge", SpecialUpgradePulse.GetCircleSprite(), 7);
            innerRingRenderer = CreateRenderer("Eclipse Inner Ring", SpecialUpgradePulse.GetCircleSprite(), 8);
            glyphRenderer = CreateRenderer("Eclipse Glyph", SpecialUpgradePulse.GetStarSprite(), 9);
            runeRenderer = CreateRenderer("Eclipse Rune Edge", GetFrameOrFallback(runeFrames, SpecialUpgradePulse.GetStarSprite()), 10);

            fillRenderer.transform.localScale = Vector3.one * 0.98f;
            outerRingRenderer.transform.localScale = Vector3.one * 1.02f;
            innerRingRenderer.transform.localScale = Vector3.one * 0.72f;
            glyphRenderer.transform.localScale = Vector3.one * 0.48f;
            runeRenderer.transform.localScale = Vector3.one * 0.72f;

            CreateSpiritParticles();
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

        private void LoadImportedEffectFrames()
        {
            runeFrames = LoadFrames("Effects/EclipseAura/EclipseCircleSpark", 5);
            spiritFrames = LoadFrames("Effects/EclipseAura/EclipseSpiritBlue", 5);
        }

        private Sprite[] LoadFrames(string resourcePath, int frameCount)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null || frameCount <= 0)
                return null;

            int frameWidth = texture.width / frameCount;
            Sprite[] frames = new Sprite[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                Rect rect = new Rect(i * frameWidth, 0f, frameWidth, texture.height);
                frames[i] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), frameWidth);
            }

            return frames;
        }

        private Sprite GetFrameOrFallback(Sprite[] frames, Sprite fallback)
        {
            return frames == null || frames.Length == 0 || frames[0] == null ? fallback : frames[0];
        }

        private void CreateSpiritParticles()
        {
            const int particleCount = 5;
            spiritRenderers = new SpriteRenderer[particleCount];

            for (int i = 0; i < particleCount; i++)
            {
                SpriteRenderer renderer = CreateRenderer(
                    "Eclipse Spirit Particle",
                    GetFrameOrFallback(spiritFrames, SpecialUpgradePulse.GetDiamondSprite()),
                    11);

                renderer.transform.localScale = Vector3.one * (0.045f + (i % 2) * 0.01f);
                spiritRenderers[i] = renderer;
            }
        }

        private void AnimateImportedEffects()
        {
            animationTimer += Time.deltaTime;
            if (animationTimer < 0.11f)
                return;

            animationTimer = 0f;
            animationFrame++;

            if (runeRenderer != null && runeFrames != null && runeFrames.Length > 0)
                runeRenderer.sprite = runeFrames[animationFrame % runeFrames.Length];

            if (spiritRenderers == null || spiritFrames == null || spiritFrames.Length == 0)
                return;

            for (int i = 0; i < spiritRenderers.Length; i++)
            {
                if (spiritRenderers[i] != null)
                    spiritRenderers[i].sprite = spiritFrames[(animationFrame + i) % spiritFrames.Length];
            }
        }

        private void PositionSpiritParticles()
        {
            if (spiritRenderers == null)
                return;

            for (int i = 0; i < spiritRenderers.Length; i++)
            {
                if (spiritRenderers[i] == null)
                    continue;

                float angle = pulseTimer * (0.52f + i * 0.035f) + i * Mathf.PI * 2f / spiritRenderers.Length;
                float orbit = 0.34f + Mathf.Sin(pulseTimer * 1.3f + i) * 0.025f;
                Vector3 localPosition = new Vector3(Mathf.Cos(angle) * orbit, Mathf.Sin(angle) * orbit, 0f);
                spiritRenderers[i].transform.localPosition = localPosition;
                spiritRenderers[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + 90f);
            }
        }

        private void ApplyColors()
        {
            float edgePulse = 0.82f + Mathf.Sin(pulseTimer * 3.2f) * 0.12f + damagePulse * 0.42f;
            Color darkFill = new Color(0.03f, 0.01f, 0.06f, 0.16f + damagePulse * 0.08f);
            Color main = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(baseColor.a * 0.32f));
            Color edge = new Color(0.64f, 0.32f, 0.9f, Mathf.Clamp01(0.2f * edgePulse));
            Color inner = new Color(0.28f, 0.1f, 0.48f, Mathf.Clamp01(0.1f + damagePulse * 0.1f));
            Color glyph = new Color(0.88f, 0.65f, 1f, Mathf.Clamp01(0.04f + damagePulse * 0.16f));
            Color rune = new Color(0.72f, 0.42f, 0.95f, Mathf.Clamp01(0.08f + damagePulse * 0.14f));
            Color spirit = new Color(0.46f, 0.62f, 0.92f, Mathf.Clamp01(0.1f + damagePulse * 0.1f));

            if (fillRenderer != null)
                fillRenderer.color = Color.Lerp(darkFill, main, 0.35f);

            if (outerRingRenderer != null)
                outerRingRenderer.color = edge;

            if (innerRingRenderer != null)
                innerRingRenderer.color = inner;

            if (glyphRenderer != null)
                glyphRenderer.color = glyph;

            if (runeRenderer != null)
                runeRenderer.color = rune;

            if (spiritRenderers == null)
                return;

            for (int i = 0; i < spiritRenderers.Length; i++)
            {
                if (spiritRenderers[i] == null)
                    continue;

                float alphaPulse = 0.75f + Mathf.Sin(pulseTimer * 2.4f + i) * 0.25f;
                spiritRenderers[i].color = new Color(spirit.r, spirit.g, spirit.b, spirit.a * alphaPulse);
            }
        }
    }
}
