using System.Collections;
using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어를 감싸는 보호막의 생성, 유지, 피격, 파괴 연출을 담당한다.
    /// 실제 피해 차단 로직은 PlayerSpecialUpgradeController에 남겨두고 시각 효과만 처리한다.
    /// </summary>
    public class ShieldVFXController : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField]
        private Transform followTarget;

        [Header("Look")]
        [SerializeField]
        private float shieldSize = 1.36f;

        [SerializeField]
        private Color baseColor = new Color(0.34f, 0.82f, 1f, 1f);

        [SerializeField]
        private float edgeBrightness = 1.55f;

        [SerializeField]
        private float overallAlpha = 0.5f;

        [Header("Animation")]
        [SerializeField]
        private float breathingSpeed = 2.2f;

        [SerializeField]
        private float orbitParticleSpeed = 70f;

        [SerializeField]
        private float waveInterval = 0.95f;

        [SerializeField]
        private float hitFlashIntensity = 1.55f;

        [SerializeField]
        private float appearDuration = 0.28f;

        [SerializeField]
        private float breakDuration = 0.34f;

        [Header("Sorting")]
        [SerializeField]
        private int sortingOrderOffset = 14;

        [Header("Performance")]
        [SerializeField]
        private int orbitParticleCount = 8;

        private SpriteRenderer backRenderer;
        private SpriteRenderer coreRenderer;
        private SpriteRenderer rimRenderer;
        private SpriteRenderer frontRenderer;
        private SpriteRenderer flowRenderer;
        private SpriteRenderer groundGlowRenderer;
        private SpriteRenderer hitRenderer;
        private SpriteRenderer crackRenderer;
        private Transform particleRoot;
        private SpriteRenderer[] orbitParticles;
        private float waveTimer;
        private float ratio = 1f;
        private bool breaking;
        private Coroutine appearRoutine;
        private Coroutine hitRoutine;
        private Coroutine breakRoutine;

        public void Initialize(Transform owner)
        {
            followTarget = owner;
            BuildVisuals();
            ApplySortingOrders();
            PlayAppear();
        }

        private void Awake()
        {
            BuildVisuals();
        }

        private void Update()
        {
            if (followTarget == null || GameState.IsGameOver)
            {
                if (!breaking)
                    PlayBreak();

                return;
            }

            transform.position = followTarget.position;

            if (Time.timeScale <= 0f || breaking)
                return;

            AnimateIdle();
        }

        public void PlayAppear()
        {
            BuildVisuals();

            if (appearRoutine != null)
                StopCoroutine(appearRoutine);

            appearRoutine = StartCoroutine(AppearRoutine());
        }

        public void PlayIdle()
        {
            breaking = false;
            SetRenderersEnabled(true);
        }

        public void PlayHit(Vector2 hitDirection)
        {
            BuildVisuals();

            if (hitRoutine != null)
                StopCoroutine(hitRoutine);

            hitRoutine = StartCoroutine(HitRoutine(hitDirection));
        }

        public void PlayBreak()
        {
            BuildVisuals();

            if (breaking)
                return;

            breaking = true;

            if (breakRoutine != null)
                StopCoroutine(breakRoutine);

            breakRoutine = StartCoroutine(BreakRoutine());
        }

        public void SetShieldRatio(float shieldRatio)
        {
            ratio = Mathf.Clamp01(shieldRatio);
        }

        private void BuildVisuals()
        {
            if (backRenderer != null)
                return;

            backRenderer = CreateRenderer("ShieldBack", ShieldSprites.BackShell, sortingOrderOffset - 1);
            coreRenderer = CreateRenderer("ShieldCore", ShieldSprites.CoreShell, sortingOrderOffset);
            flowRenderer = CreateRenderer("EnergyFlow", ShieldSprites.FlowShell, sortingOrderOffset + 1);
            rimRenderer = CreateRenderer("ShieldRim", ShieldSprites.FullRim, sortingOrderOffset + 2);
            frontRenderer = CreateRenderer("ShieldFront", ShieldSprites.FrontShell, sortingOrderOffset + 3);
            groundGlowRenderer = CreateRenderer("GroundGlow", ShieldSprites.GroundGlow, sortingOrderOffset - 2);
            hitRenderer = CreateRenderer("HitEffect", ShieldSprites.HitArc, sortingOrderOffset + 4);
            crackRenderer = CreateRenderer("BreakEffect", ShieldSprites.Cracks, sortingOrderOffset + 5);

            hitRenderer.enabled = false;
            crackRenderer.enabled = false;

            groundGlowRenderer.transform.localScale = new Vector3(0.95f, 0.26f, 1f);
            particleRoot = new GameObject("OrbitParticles").transform;
            particleRoot.SetParent(transform, false);
            CreateOrbitParticles();
            ApplySortingOrders();
            ApplyBaseColors(0f);
        }

        private SpriteRenderer CreateRenderer(string objectName, Sprite sprite, int order)
        {
            GameObject child = new GameObject(objectName);
            child.transform.SetParent(transform, false);
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            return renderer;
        }

        private void CreateOrbitParticles()
        {
            orbitParticleCount = Mathf.Clamp(orbitParticleCount, 0, 24);
            orbitParticles = new SpriteRenderer[orbitParticleCount];

            for (int i = 0; i < orbitParticleCount; i++)
            {
                GameObject particle = new GameObject($"Particle_{i:00}");
                particle.transform.SetParent(particleRoot, false);
                SpriteRenderer renderer = particle.AddComponent<SpriteRenderer>();
                renderer.sprite = ShieldSprites.Particle;
                renderer.sortingOrder = sortingOrderOffset + 4;
                orbitParticles[i] = renderer;
            }
        }

        private void ApplySortingOrders()
        {
            int baseOrder = 0;
            int gap = Mathf.Max(1, Mathf.Abs(sortingOrderOffset));

            if (followTarget != null)
            {
                Transform rendererRoot = followTarget.parent == null ? followTarget : followTarget.parent;
                SpriteRenderer[] ownerRenderers = rendererRoot.GetComponentsInChildren<SpriteRenderer>(true);

                for (int i = 0; i < ownerRenderers.Length; i++)
                    baseOrder = Mathf.Max(baseOrder, ownerRenderers[i].sortingOrder);
            }

            if (backRenderer != null)
                backRenderer.sortingOrder = baseOrder - gap;

            if (coreRenderer != null)
                coreRenderer.sortingOrder = baseOrder - 1;

            if (groundGlowRenderer != null)
                groundGlowRenderer.sortingOrder = baseOrder - gap - 1;

            if (flowRenderer != null)
                flowRenderer.sortingOrder = baseOrder + gap - 1;

            if (rimRenderer != null)
                rimRenderer.sortingOrder = baseOrder + gap;

            if (frontRenderer != null)
                frontRenderer.sortingOrder = baseOrder + gap + 1;

            if (hitRenderer != null)
                hitRenderer.sortingOrder = baseOrder + gap + 2;

            if (crackRenderer != null)
                crackRenderer.sortingOrder = baseOrder + gap + 3;

            if (orbitParticles == null)
                return;

            for (int i = 0; i < orbitParticles.Length; i++)
            {
                if (orbitParticles[i] != null)
                    orbitParticles[i].sortingOrder = baseOrder + gap + 2;
            }
        }

        private void AnimateIdle()
        {
            float time = Time.time;
            float pulse = 1f + Mathf.Sin(time * breathingSpeed) * 0.018f;
            float lowRatioFlicker = ratio < 0.35f ? Mathf.Abs(Mathf.Sin(time * 11f)) * 0.28f : 0f;
            float alpha = overallAlpha * (0.92f + Mathf.Sin(time * breathingSpeed * 0.75f) * 0.06f + lowRatioFlicker);

            transform.localScale = Vector3.one * shieldSize * pulse;
            flowRenderer.transform.Rotate(0f, 0f, -12f * Time.deltaTime);
            particleRoot.Rotate(0f, 0f, orbitParticleSpeed * Time.deltaTime);
            ApplyBaseColors(alpha);
            AnimateParticles(time, alpha);

            waveTimer += Time.deltaTime;

            if (waveTimer >= waveInterval)
            {
                waveTimer = 0f;
                StartCoroutine(WaveRoutine());
            }
        }

        private void AnimateParticles(float time, float alpha)
        {
            if (orbitParticles == null || orbitParticles.Length == 0)
                return;

            for (int i = 0; i < orbitParticles.Length; i++)
            {
                SpriteRenderer particle = orbitParticles[i];

                if (particle == null)
                    continue;

                float angle = (Mathf.PI * 2f * i / orbitParticles.Length) + time * 0.45f;
                float x = Mathf.Cos(angle) * 0.45f;
                float y = Mathf.Sin(angle) * 0.53f;
                particle.transform.localPosition = new Vector3(x, y, 0f);
                particle.transform.localScale = Vector3.one * (0.58f + Mathf.Sin(time * 2.4f + i) * 0.12f);
                particle.color = WithAlpha(Boost(baseColor, 1.55f), alpha * 0.5f);
            }
        }

        private void ApplyBaseColors(float alpha)
        {
            if (backRenderer == null)
                return;

            backRenderer.color = WithAlpha(Boost(baseColor, edgeBrightness * 0.92f), alpha * 0.34f);
            coreRenderer.color = WithAlpha(baseColor, alpha * 0.07f);
            rimRenderer.color = WithAlpha(Boost(baseColor, edgeBrightness * 1.35f), alpha * 0.86f);
            frontRenderer.color = WithAlpha(Boost(baseColor, edgeBrightness * 1.55f), alpha * 0.72f);
            flowRenderer.color = WithAlpha(Boost(baseColor, edgeBrightness * 1.16f), alpha * 0.22f);
            groundGlowRenderer.color = WithAlpha(baseColor, alpha * 0.07f);

            if (crackRenderer != null && !breaking)
            {
                crackRenderer.enabled = ratio < 0.35f;
                crackRenderer.color = WithAlpha(Color.white, (0.35f - ratio) * 0.9f);
            }
        }

        private IEnumerator AppearRoutine()
        {
            SetRenderersEnabled(true);
            crackRenderer.enabled = false;
            hitRenderer.enabled = false;

            float elapsed = 0f;

            while (elapsed < appearDuration)
            {
                float t = Mathf.Clamp01(elapsed / appearDuration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                transform.localScale = Vector3.one * shieldSize * Mathf.Lerp(0.18f, 1.08f, eased);
                ApplyBaseColors(overallAlpha * eased);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.one * shieldSize;
            ApplyBaseColors(overallAlpha);
            StartCoroutine(WaveRoutine());
            appearRoutine = null;
        }

        private IEnumerator HitRoutine(Vector2 hitDirection)
        {
            hitRenderer.enabled = true;
            hitRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, DirectionToAngle(hitDirection));

            Vector3 originalLocalPosition = transform.localPosition;
            float elapsed = 0f;
            const float duration = 0.16f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float fade = 1f - t;
                float shake = Mathf.Sin(t * Mathf.PI * 6f) * 0.025f * fade;
                transform.localPosition = originalLocalPosition + (Vector3)(hitDirection.normalized * shake);
                hitRenderer.color = WithAlpha(Boost(baseColor, hitFlashIntensity), fade * 0.92f);
                rimRenderer.color = WithAlpha(Boost(baseColor, hitFlashIntensity * 1.2f), overallAlpha * (0.9f + fade * 0.75f));
                frontRenderer.color = WithAlpha(Boost(baseColor, hitFlashIntensity), overallAlpha * (0.62f + fade * 0.46f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalLocalPosition;
            hitRenderer.enabled = false;
            hitRoutine = null;
        }

        private IEnumerator BreakRoutine()
        {
            if (hitRoutine != null)
                yield return new WaitForSeconds(0.08f);

            crackRenderer.enabled = true;
            float elapsed = 0f;

            while (elapsed < breakDuration)
            {
                float t = Mathf.Clamp01(elapsed / breakDuration);
                float flash = Mathf.Sin(t * Mathf.PI);
                transform.localScale = Vector3.one * shieldSize * Mathf.Lerp(1.06f, 1.32f, t);
                ApplyBaseColors(overallAlpha * (1f - t) * 0.75f);
                crackRenderer.color = WithAlpha(Color.white, Mathf.Lerp(0.95f, 0f, t) + flash * 0.15f);
                ScatterParticles(t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private IEnumerator WaveRoutine()
        {
            SpriteRenderer waveRenderer = CreateRenderer("ShieldWave", ShieldSprites.RingWave, sortingOrderOffset + 2);
            float elapsed = 0f;
            const float duration = 0.42f;

            while (elapsed < duration && waveRenderer != null)
            {
                float t = elapsed / duration;
                waveRenderer.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1.08f, t);
                waveRenderer.color = WithAlpha(Boost(baseColor, 1.55f), (1f - t) * overallAlpha * 0.46f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (waveRenderer != null)
                Destroy(waveRenderer.gameObject);
        }

        private void ScatterParticles(float progress)
        {
            if (orbitParticles == null)
                return;

            for (int i = 0; i < orbitParticles.Length; i++)
            {
                SpriteRenderer particle = orbitParticles[i];

                if (particle == null)
                    continue;

                Vector3 direction = particle.transform.localPosition.sqrMagnitude <= 0.001f
                    ? Quaternion.Euler(0f, 0f, i * 37f) * Vector3.right
                    : particle.transform.localPosition.normalized;

                particle.transform.localPosition += direction * Time.deltaTime * (1.15f + i * 0.03f);
                particle.color = WithAlpha(Color.white, (1f - progress) * 0.72f);
            }
        }

        private void SetRenderersEnabled(bool enabled)
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer renderer in renderers)
                renderer.enabled = enabled;
        }

        private static float DirectionToAngle(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
                return 0f;

            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
        }

        private static Color Boost(Color color, float brightness)
        {
            return new Color(
                Mathf.Clamp01(color.r * brightness),
                Mathf.Clamp01(color.g * brightness),
                Mathf.Clamp01(color.b * brightness),
                color.a);
        }
    }

    internal static class ShieldSprites
    {
        private const int ShellWidth = 160;
        private const int ShellHeight = 192;
        private const float ShellPixelsPerUnit = 160f;

        private static Sprite backShell;
        private static Sprite coreShell;
        private static Sprite fullRim;
        private static Sprite frontShell;
        private static Sprite flowShell;
        private static Sprite groundGlow;
        private static Sprite hitArc;
        private static Sprite cracks;
        private static Sprite particle;
        private static Sprite ringWave;

        public static Sprite BackShell => backShell ??= CreateEllipseArcSprite(ShellWidth, ShellHeight, 0.42f, 0.39f, 200f, 340f, 0.052f);
        public static Sprite CoreShell => coreShell ??= CreateCoreSprite();
        public static Sprite FullRim => fullRim ??= CreateFullRimSprite();
        public static Sprite FrontShell => frontShell ??= CreateEllipseArcSprite(ShellWidth, ShellHeight, 0.425f, 0.395f, 20f, 160f, 0.058f);
        public static Sprite FlowShell => flowShell ??= CreateFlowSprite();
        public static Sprite GroundGlow => groundGlow ??= CreateGroundGlowSprite();
        public static Sprite HitArc => hitArc ??= CreateEllipseArcSprite(ShellWidth, ShellHeight, 0.435f, 0.405f, -35f, 35f, 0.078f);
        public static Sprite Cracks => cracks ??= CreateCrackSprite();
        public static Sprite Particle => particle ??= CreateParticleSprite();
        public static Sprite RingWave => ringWave ??= CreateEllipseArcSprite(ShellWidth, ShellHeight, 0.43f, 0.4f, 0f, 360f, 0.038f);

        private static Sprite CreateCoreSprite()
        {
            const int width = ShellWidth;
            const int height = ShellHeight;
            Texture2D texture = CreateTexture(width, height, FilterMode.Bilinear);
            Vector2 center = GetShellCenter(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = (x - center.x) / (width * 0.39f);
                    float ny = (y - center.y) / (height * 0.36f);
                    float distance = nx * nx + ny * ny;
                    float shell = Mathf.Sqrt(distance);
                    float centerFade = Mathf.SmoothStep(0.7f, 1f, shell);
                    float alpha = distance <= 1f ? centerFade * 0.12f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture, ShellPixelsPerUnit);
        }

        private static Sprite CreateFlowSprite()
        {
            const int width = ShellWidth;
            const int height = ShellHeight;
            Texture2D texture = CreateTexture(width, height, FilterMode.Bilinear);
            Vector2 center = GetShellCenter(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = (x - center.x) / (width * 0.39f);
                    float ny = (y - center.y) / (height * 0.36f);
                    float distance = Mathf.Sqrt(nx * nx + ny * ny);
                    float angle = Mathf.Atan2(ny, nx);
                    float shellMask = Mathf.SmoothStep(0.62f, 0.9f, distance) * (1f - Mathf.SmoothStep(1f, 1.06f, distance));
                    float stream = Mathf.Abs(Mathf.Sin(angle * 4.5f + distance * 10f));
                    bool band = shellMask > 0f && stream < 0.085f;
                    float alpha = band ? Mathf.Lerp(0.25f, 0.62f, shellMask) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture, ShellPixelsPerUnit);
        }

        private static Sprite CreateFullRimSprite()
        {
            const int width = ShellWidth;
            const int height = ShellHeight;
            Texture2D texture = CreateTexture(width, height, FilterMode.Bilinear);
            Vector2 center = GetShellCenter(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = (x - center.x) / (width * 0.42f);
                    float ny = (y - center.y) / (height * 0.39f);
                    float distance = Mathf.Sqrt(nx * nx + ny * ny);
                    float angle = Mathf.Atan2(ny, nx);
                    float outer = 1f - Mathf.Clamp01(Mathf.Abs(distance - 1f) / 0.035f);
                    float inner = 1f - Mathf.Clamp01(Mathf.Abs(distance - 0.9f) / 0.018f);
                    float sideHighlight = Mathf.Pow(Mathf.Abs(Mathf.Cos(angle)), 5f) * 0.34f;
                    float topHighlight = Mathf.Clamp01(Mathf.Sin(angle) * 1.8f) * 0.28f;
                    float alpha = Mathf.Max(outer, inner * 0.36f);

                    if (distance > 0.78f && distance < 1.04f)
                        alpha += (sideHighlight + topHighlight) * outer;

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
                }
            }

            return ToSprite(texture, ShellPixelsPerUnit);
        }

        private static Sprite CreateGroundGlowSprite()
        {
            const int size = 128;
            Texture2D texture = CreateTexture(size, size, FilterMode.Bilinear);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x - center.x) / (size * 0.46f);
                    float ny = (y - center.y) / (size * 0.46f);
                    float distance = nx * nx + ny * ny;
                    float ring = Mathf.Abs(Mathf.Sqrt(distance) - 0.78f) < 0.02f ? 0.42f : 0f;
                    float glow = distance <= 1f ? Mathf.Pow(1f - distance, 3.2f) * 0.22f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Max(ring, glow)));
                }
            }

            return ToSprite(texture, size);
        }

        private static Sprite CreateCrackSprite()
        {
            const int width = ShellWidth;
            const int height = ShellHeight;
            Texture2D texture = CreateTexture(width, height, FilterMode.Bilinear);
            DrawLine(texture, 81, 38, 72, 68, 2);
            DrawLine(texture, 72, 68, 90, 98, 2);
            DrawLine(texture, 90, 98, 78, 148, 2);
            DrawLine(texture, 50, 65, 72, 93, 2);
            DrawLine(texture, 112, 58, 91, 95, 2);
            DrawLine(texture, 64, 130, 40, 160, 2);
            DrawLine(texture, 96, 130, 124, 162, 2);
            texture.Apply();
            return ToSprite(texture, ShellPixelsPerUnit);
        }

        private static Sprite CreateParticleSprite()
        {
            const int size = 16;
            Texture2D texture = CreateTexture(size, size, FilterMode.Bilinear);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.45f);
                    float alpha = distance <= 1f ? Mathf.Pow(1f - distance, 1.2f) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture, size);
        }

        private static Sprite CreateEllipseArcSprite(int width, int height, float radiusX, float radiusY, float startDegree, float endDegree, float thickness)
        {
            Texture2D texture = CreateTexture(width, height, FilterMode.Bilinear);
            Vector2 center = GetShellCenter(width, height);
            float start = Mathf.Repeat(startDegree, 360f);
            float end = Mathf.Repeat(endDegree, 360f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = (x - center.x) / (width * radiusX);
                    float ny = (y - center.y) / (height * radiusY);
                    float distance = Mathf.Sqrt(nx * nx + ny * ny);
                    float angle = Mathf.Repeat(Mathf.Atan2(ny, nx) * Mathf.Rad2Deg + 360f, 360f);
                    bool inArc = IsAngleBetween(angle, start, end);
                    float edge = 1f - Mathf.Clamp01(Mathf.Abs(distance - 1f) / thickness);
                    float inner = 1f - Mathf.Clamp01(Mathf.Abs(distance - 0.88f) / (thickness * 0.45f));
                    float alpha = inArc ? Mathf.Max(edge, inner * 0.42f) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return ToSprite(texture, ShellPixelsPerUnit);
        }

        private static Vector2 GetShellCenter(int width, int height)
        {
            return new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
        }

        private static bool IsAngleBetween(float angle, float start, float end)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(start, end)) < 0.01f)
                return true;

            if (start <= end)
                return angle >= start && angle <= end;

            return angle >= start || angle <= end;
        }

        private static Texture2D CreateTexture(int width, int height, FilterMode filterMode)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    texture.SetPixel(x, y, Color.clear);
            }

            return texture;
        }

        private static Sprite ToSprite(Texture2D texture, float pixelsPerUnit)
        {
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }

        private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, int radius)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawDot(texture, x0, y0, radius);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = err * 2;

                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private static void DrawDot(Texture2D texture, int centerX, int centerY, int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px < 0 || px >= texture.width || py < 0 || py >= texture.height)
                        continue;

                    float distance = Mathf.Sqrt(x * x + y * y);

                    if (distance > radius)
                        continue;

                    float alpha = 1f - distance / (radius + 0.01f);
                    texture.SetPixel(px, py, new Color(1f, 1f, 1f, Mathf.Max(texture.GetPixel(px, py).a, alpha)));
                }
            }
        }
    }
}
