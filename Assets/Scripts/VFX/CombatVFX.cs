using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.VFX
{
    public enum CombatVFXKind
    {
        ArcaneImpact,
        Explosion,
        Frost,
        Shockwave,
        Ricochet,
        ChainLightning,
        Vampirism,
        ConeWarning,
        ConeImpact,
        TargetWarning,
        TargetImpact,
        WebZone,
        FireZone,
        FrostZone,
        Buff
    }

    public static class CombatVFX
    {
        private static readonly int DefaultSortingOrder = 15;
        private static CombatVFXSettings settings;
        private static Material sharedTrailMaterial;
        private static CombatVFXSettings Settings
        {
            get
            {
                if (settings != null)
                    return settings;

                GameObject prefab = Resources.Load<GameObject>("VFX/CombatVFXSettings");
                settings = prefab == null ? null : prefab.GetComponent<CombatVFXSettings>();
                return settings;
            }
        }

        public static void PlayBurst(Vector2 position, CombatVFXKind kind, float size, float duration = 0.28f, int sortingOrder = 15)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            Color main = GetMainColor(kind);
            Color secondary = GetSecondaryColor(kind);
            GameObject root = new GameObject($"VFX {kind}");
            root.transform.position = position;

            SpriteRenderer core = CreateRenderer(root.transform, "Core", VFXSprites.GetBurstSprite(kind), main, sortingOrder);
            SpriteRenderer ring = CreateRenderer(root.transform, "Ring", VFXSprites.Ring, secondary, sortingOrder + 1);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Sparks", VFXSprites.Sparks, secondary, sortingOrder + 2);

            core.transform.localScale = Vector3.one * size * 0.72f;
            ring.transform.localScale = Vector3.one * size;
            sparks.transform.localScale = Vector3.one * size * 0.9f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 1f, 1.22f, 120f, true);
        }

        public static GameObject PlayWarning(Vector2 position, CombatVFXKind kind, float size, float duration = 0.9f, int sortingOrder = 12)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            GameObject root = new GameObject($"VFX {kind} Warning");
            root.transform.position = position;
            Color color = GetMainColor(kind);

            SpriteRenderer fill = CreateRenderer(root.transform, "Soft Fill", VFXSprites.SoftDisc, WithAlpha(color, 0.16f), sortingOrder);
            SpriteRenderer ring = CreateRenderer(root.transform, "Edge", VFXSprites.WarningRing, WithAlpha(color, 0.46f), sortingOrder + 1);
            SpriteRenderer glyph = CreateRenderer(root.transform, "Glyph", VFXSprites.Glyph, WithAlpha(GetSecondaryColor(kind), 0.34f), sortingOrder + 2);

            fill.transform.localScale = Vector3.one * size;
            ring.transform.localScale = Vector3.one * size;
            glyph.transform.localScale = Vector3.one * size * 0.72f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.92f, 1.02f, 42f, false);
            return root;
        }

        public static GameObject PlayCone(Vector2 position, Vector2 direction, CombatVFXKind kind, float range, bool impact, float duration, int sortingOrder = 14)
        {
            range *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            GameObject root = new GameObject(impact ? "VFX Cone Impact" : "VFX Cone Warning");
            root.transform.position = position;
            root.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized);

            Color color = impact ? GetSecondaryColor(kind) : GetMainColor(kind);
            float alpha = impact ? 0.66f : 0.28f;
            SpriteRenderer cone = CreateRenderer(root.transform, "Cone", VFXSprites.Cone, WithAlpha(color, alpha), sortingOrder);
            SpriteRenderer edge = CreateRenderer(root.transform, "Edge", VFXSprites.ConeEdge, WithAlpha(GetSecondaryColor(kind), alpha * 0.8f), sortingOrder + 1);

            cone.transform.localScale = Vector3.one * range;
            edge.transform.localScale = Vector3.one * range;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, impact ? 0.88f : 0.94f, impact ? 1.08f : 1.01f, impact ? 30f : 8f, true);
            return root;
        }

        public static void PlayLine(Vector2 from, Vector2 to, CombatVFXKind kind, float duration = 0.16f, float width = 0.08f, int sortingOrder = 16)
        {
            duration *= DurationMultiplier;
            width *= SizeMultiplier;
            sortingOrder += SortingOffset;
            Vector2 direction = to - from;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            GameObject root = new GameObject($"VFX {kind} Line");
            root.transform.position = (from + to) * 0.5f;
            root.transform.right = direction.normalized;

            SpriteRenderer line = CreateRenderer(root.transform, "Line", VFXSprites.Line, GetMainColor(kind), sortingOrder);
            SpriteRenderer core = CreateRenderer(root.transform, "Core", VFXSprites.LineCore, GetSecondaryColor(kind), sortingOrder + 1);
            line.transform.localScale = new Vector3(direction.magnitude, width, 1f);
            core.transform.localScale = new Vector3(direction.magnitude, width * 0.35f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 1f, 1f, 0f, true);
        }

        public static void PlayChainLightning(Vector2 from, Vector2 to, float duration = 0.22f, float width = 0.07f, int sortingOrder = 19)
        {
            duration *= DurationMultiplier;
            width *= SizeMultiplier;
            sortingOrder += SortingOffset;
            Vector2 direction = to - from;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            GameObject root = new GameObject("VFX Chain Lightning Line");
            LineRenderer glow = CreateLightningRenderer(root, "Glow", width * 2.8f, WithAlpha(GetMainColor(CombatVFXKind.ChainLightning), 0.34f), sortingOrder);
            LineRenderer core = CreateLightningRenderer(root, "Core", width, GetSecondaryColor(CombatVFXKind.ChainLightning), sortingOrder + 1);
            Vector3[] points = CreateLightningPoints(from, to, 7, width * 3.5f);

            glow.positionCount = points.Length;
            core.positionCount = points.Length;
            glow.SetPositions(points);
            core.SetPositions(points);

            CombatVFXLineEffect effect = root.AddComponent<CombatVFXLineEffect>();
            effect.Play(duration, glow, core);
        }

        public static GameObject CreateZoneVisual(Transform parent, CombatVFXKind kind, float radius, Color tint, int sortingOrder = 10)
        {
            radius *= SizeMultiplier;
            sortingOrder += SortingOffset;
            GameObject root = new GameObject($"VFX {kind} Zone");
            root.transform.SetParent(parent, false);

            Color main = tint.a > 0f ? tint : GetMainColor(kind);
            SpriteRenderer fill = CreateRenderer(root.transform, "Zone Fill", VFXSprites.ZoneFill, WithAlpha(main, 0.24f), sortingOrder);
            SpriteRenderer edge = CreateRenderer(root.transform, "Zone Edge", GetZoneEdgeSprite(kind), WithAlpha(GetSecondaryColor(kind), 0.68f), sortingOrder + 1);
            SpriteRenderer detail = CreateRenderer(root.transform, "Zone Detail", GetZoneDetailSprite(kind), WithAlpha(GetSecondaryColor(kind), 0.46f), sortingOrder + 2);

            float diameter = radius * 2f;
            fill.transform.localScale = Vector3.one * diameter;
            edge.transform.localScale = Vector3.one * diameter;
            detail.transform.localScale = Vector3.one * diameter * 0.92f;

            CombatVFXLoop loop = root.AddComponent<CombatVFXLoop>();
            loop.Configure(detail.transform, edge, fill, 16f, 0.08f);
            return root;
        }

        public static void AttachTrail(GameObject host, CombatVFXKind kind, float width = 0.08f, float lifeTime = 0.18f)
        {
            if (host == null || host.GetComponent<TrailRenderer>() != null)
                return;

            if (Settings != null && !Settings.EnableProjectileTrails)
                return;

            TrailRenderer trail = host.AddComponent<TrailRenderer>();
            trail.time = lifeTime * DurationMultiplier;
            trail.startWidth = width * TrailWidthMultiplier;
            trail.endWidth = 0f;
            trail.numCapVertices = 2;
            trail.sortingOrder = DefaultSortingOrder - 1 + SortingOffset;
            trail.material = SharedTrailMaterial;
            trail.startColor = WithAlpha(GetSecondaryColor(kind), 0.65f);
            trail.endColor = WithAlpha(GetMainColor(kind), 0f);
        }

        public static CombatVFXKind KindFromProjectileSprite(Sprite sprite)
        {
            if (sprite != null && sprite.name.ToLowerInvariant().Contains("selene"))
                return CombatVFXKind.Frost;

            if (sprite != null && sprite.name.ToLowerInvariant().Contains("kael"))
                return CombatVFXKind.ArcaneImpact;

            return CombatVFXKind.ArcaneImpact;
        }

        public static Color GetMainColor(CombatVFXKind kind)
        {
            switch (kind)
            {
                case CombatVFXKind.Explosion:
                case CombatVFXKind.FireZone:
                    return new Color(1f, 0.36f, 0.08f, 0.82f);
                case CombatVFXKind.Frost:
                case CombatVFXKind.FrostZone:
                    return new Color(0.45f, 0.88f, 1f, 0.78f);
                case CombatVFXKind.Shockwave:
                    return new Color(0.78f, 0.95f, 1f, 0.62f);
                case CombatVFXKind.Ricochet:
                    return new Color(0.5f, 0.95f, 1f, 0.86f);
                case CombatVFXKind.ChainLightning:
                    return new Color(0.52f, 0.9f, 1f, 0.88f);
                case CombatVFXKind.Vampirism:
                    return new Color(0.18f, 1f, 0.46f, 0.74f);
                case CombatVFXKind.WebZone:
                    return new Color(0.82f, 0.88f, 1f, 0.58f);
                case CombatVFXKind.ConeWarning:
                case CombatVFXKind.TargetWarning:
                    return new Color(1f, 0.18f, 0.12f, 0.38f);
                default:
                    return new Color(0.72f, 0.42f, 1f, 0.78f);
            }
        }

        public static Color GetSecondaryColor(CombatVFXKind kind)
        {
            switch (kind)
            {
                case CombatVFXKind.Explosion:
                case CombatVFXKind.FireZone:
                    return new Color(1f, 0.86f, 0.22f, 0.9f);
                case CombatVFXKind.Frost:
                case CombatVFXKind.FrostZone:
                    return new Color(0.86f, 1f, 1f, 0.92f);
                case CombatVFXKind.Vampirism:
                    return new Color(0.92f, 1f, 0.65f, 0.86f);
                case CombatVFXKind.ChainLightning:
                    return new Color(0.98f, 1f, 0.58f, 0.92f);
                case CombatVFXKind.WebZone:
                    return new Color(0.95f, 0.98f, 1f, 0.76f);
                default:
                    return new Color(0.95f, 0.88f, 1f, 0.9f);
            }
        }

        private static SpriteRenderer CreateRenderer(Transform parent, string name, Sprite sprite, Color color, int sortingOrder)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static Sprite GetZoneEdgeSprite(CombatVFXKind kind)
        {
            return kind == CombatVFXKind.WebZone ? VFXSprites.Web : VFXSprites.WarningRing;
        }

        private static Sprite GetZoneDetailSprite(CombatVFXKind kind)
        {
            return kind == CombatVFXKind.FireZone ? VFXSprites.FlameGlyph : VFXSprites.Glyph;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha * AlphaMultiplier));
        }

        private static LineRenderer CreateLightningRenderer(GameObject parent, string name, float width, Color color, int sortingOrder)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.material = SharedTrailMaterial;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.numCapVertices = 2;
            lineRenderer.numCornerVertices = 2;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width * 0.72f;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.sortingOrder = sortingOrder;
            return lineRenderer;
        }

        private static Vector3[] CreateLightningPoints(Vector2 from, Vector2 to, int segments, float jitter)
        {
            segments = Mathf.Max(2, segments);
            Vector3[] points = new Vector3[segments + 1];
            Vector2 direction = to - from;
            Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 point = Vector2.Lerp(from, to, t);

                if (i > 0 && i < segments)
                {
                    float fade = Mathf.Sin(t * Mathf.PI);
                    point += normal * Random.Range(-jitter, jitter) * fade;
                }

                points[i] = point;
            }

            return points;
        }

        private static float SizeMultiplier => Settings == null ? 1f : Settings.SizeMultiplier;
        private static float DurationMultiplier => Settings == null ? 1f : Settings.DurationMultiplier;
        private static float AlphaMultiplier => Settings == null ? 1f : Settings.AlphaMultiplier;
        private static int SortingOffset => Settings == null ? 0 : Settings.SortingOrderOffset;
        private static float TrailWidthMultiplier => Settings == null ? 1f : Settings.TrailWidthMultiplier;
        private static Material SharedTrailMaterial => sharedTrailMaterial ??= new Material(Shader.Find("Sprites/Default"));
    }

    public class CombatVFXEffect : MonoBehaviour
    {
        private SpriteRenderer[] renderers;
        private float duration;
        private float elapsed;
        private float startScale;
        private float endScale;
        private float rotateSpeed;
        private bool destroyWhenDone;
        private Color[] initialColors;

        public void Play(float effectDuration, float fromScale, float toScale, float rotationSpeed, bool destroyOnComplete)
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
            duration = Mathf.Max(0.02f, effectDuration);
            startScale = fromScale;
            endScale = toScale;
            rotateSpeed = rotationSpeed;
            destroyWhenDone = destroyOnComplete;
            initialColors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
                initialColors[i] = renderers[i].color;
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, eased);
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            float alpha = Mathf.Lerp(1f, 0f, progress);

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = initialColors[i];
                color.a *= alpha;
                renderers[i].color = color;
            }

            if (progress >= 1f && destroyWhenDone)
                Destroy(gameObject);
        }
    }

    public class CombatVFXLineEffect : MonoBehaviour
    {
        private LineRenderer[] lineRenderers;
        private Color[] startColors;
        private Color[] endColors;
        private float duration;
        private float elapsed;

        public void Play(float effectDuration, params LineRenderer[] targets)
        {
            lineRenderers = targets;
            duration = Mathf.Max(0.03f, effectDuration);
            elapsed = 0f;
            startColors = new Color[lineRenderers.Length];
            endColors = new Color[lineRenderers.Length];

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                if (lineRenderers[i] == null)
                    continue;

                startColors[i] = lineRenderers[i].startColor;
                endColors[i] = lineRenderers[i].endColor;
            }
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(1f, 0f, progress);

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                LineRenderer lineRenderer = lineRenderers[i];

                if (lineRenderer == null)
                    continue;

                Color startColor = startColors[i];
                Color endColor = endColors[i];
                startColor.a *= alpha;
                endColor.a *= alpha;
                lineRenderer.startColor = startColor;
                lineRenderer.endColor = endColor;
                lineRenderer.widthMultiplier = Mathf.Lerp(1f, 0.55f, progress);
            }

            if (progress >= 1f)
                Destroy(gameObject);
        }
    }

    public class CombatVFXLoop : MonoBehaviour
    {
        private Transform rotatingPart;
        private SpriteRenderer edge;
        private SpriteRenderer fill;
        private float rotateSpeed;
        private float pulseAmount;
        private Color edgeBaseColor;
        private Color fillBaseColor;

        public void Configure(Transform detailTransform, SpriteRenderer edgeRenderer, SpriteRenderer fillRenderer, float speed, float pulse)
        {
            rotatingPart = detailTransform;
            edge = edgeRenderer;
            fill = fillRenderer;
            rotateSpeed = speed;
            pulseAmount = pulse;

            if (edge != null)
                edgeBaseColor = edge.color;

            if (fill != null)
                fillBaseColor = fill.color;
        }

        private void Update()
        {
            if (GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (rotatingPart != null)
                rotatingPart.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

            float pulse = 1f + Mathf.Sin(Time.time * 2.2f) * pulseAmount;
            transform.localScale = Vector3.one * pulse;

            if (edge != null)
                edge.color = SetAlpha(edgeBaseColor, edgeBaseColor.a * (0.82f + Mathf.Sin(Time.time * 3f) * 0.14f));

            if (fill != null)
                fill.color = SetAlpha(fillBaseColor, fillBaseColor.a * (0.82f + Mathf.Sin(Time.time * 2f) * 0.1f));
        }

        private static Color SetAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }
    }

    internal static class VFXSprites
    {
        private static Sprite ring;
        private static Sprite warningRing;
        private static Sprite softDisc;
        private static Sprite sparks;
        private static Sprite starBurst;
        private static Sprite iceBurst;
        private static Sprite shockwaveBurst;
        private static Sprite glyph;
        private static Sprite flameGlyph;
        private static Sprite cone;
        private static Sprite coneEdge;
        private static Sprite line;
        private static Sprite lineCore;
        private static Sprite zoneFill;
        private static Sprite web;

        public static Sprite Ring => ring ??= CreateRingSprite(128, 0.37f, 0.43f);
        public static Sprite WarningRing => warningRing ??= CreateRingSprite(128, 0.39f, 0.43f, true);
        public static Sprite SoftDisc => softDisc ??= CreateSoftDiscSprite();
        public static Sprite Sparks => sparks ??= CreateSparkSprite();
        public static Sprite Glyph => glyph ??= CreateGlyphSprite(false);
        public static Sprite FlameGlyph => flameGlyph ??= CreateGlyphSprite(true);
        public static Sprite Cone => cone ??= CreateConeSprite(false);
        public static Sprite ConeEdge => coneEdge ??= CreateConeSprite(true);
        public static Sprite Line => line ??= CreateLineSprite(false);
        public static Sprite LineCore => lineCore ??= CreateLineSprite(true);
        public static Sprite ZoneFill => zoneFill ??= CreateSoftDiscSprite(96);
        public static Sprite Web => web ??= CreateWebSprite();

        public static Sprite GetBurstSprite(CombatVFXKind kind)
        {
            switch (kind)
            {
                case CombatVFXKind.Explosion:
                case CombatVFXKind.TargetImpact:
                    return starBurst ??= CreateStarBurstSprite();
                case CombatVFXKind.Frost:
                case CombatVFXKind.FrostZone:
                    return iceBurst ??= CreateIceBurstSprite();
                case CombatVFXKind.Shockwave:
                    return shockwaveBurst ??= CreateRingSprite(128, 0.22f, 0.4f, true);
                default:
                    return sparks ??= CreateSparkSprite();
            }
        }

        private static Sprite CreateRingSprite(int size, float inner, float outer, bool dashed = false)
        {
            Texture2D texture = CreateTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 offset = new Vector2(x, y) - center;
                    float distance = offset.magnitude / size;
                    float angle = Mathf.Atan2(offset.y, offset.x);
                    bool draw = distance >= inner && distance <= outer;

                    if (dashed)
                        draw &= Mathf.Sin(angle * 12f) > -0.35f;

                    float alpha = draw ? 1f - Mathf.Abs(distance - (inner + outer) * 0.5f) / Mathf.Max(0.01f, outer - inner) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
                }
            }

            return ToSprite(texture);
        }

        private static Sprite CreateSoftDiscSprite(int size = 128)
        {
            Texture2D texture = CreateTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.48f);
                    float alpha = distance <= 1f ? Mathf.Pow(1f - distance, 1.8f) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture);
        }

        private static Sprite CreateSparkSprite()
        {
            Texture2D texture = CreateTexture(96, 96);
            Vector2 center = new Vector2(47.5f, 47.5f);

            for (int i = 0; i < 12; i++)
            {
                float angle = Mathf.PI * 2f * i / 12f;
                DrawLine(texture, center, center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * RandomRadius(i), 2);
            }

            return ToSprite(texture);
        }

        private static Sprite CreateStarBurstSprite()
        {
            Texture2D texture = CreateTexture(128, 128);
            Vector2 center = new Vector2(63.5f, 63.5f);

            for (int i = 0; i < 18; i++)
            {
                float angle = Mathf.PI * 2f * i / 18f;
                float length = i % 2 == 0 ? 52f : 34f;
                DrawLine(texture, center, center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length, i % 2 == 0 ? 3 : 2);
            }

            return ToSprite(texture);
        }

        private static Sprite CreateIceBurstSprite()
        {
            Texture2D texture = CreateTexture(96, 96);
            Vector2 center = new Vector2(47.5f, 47.5f);

            for (int i = 0; i < 8; i++)
            {
                float angle = Mathf.PI * 2f * i / 8f;
                Vector2 tip = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 37f;
                DrawLine(texture, center, tip, 2);
                DrawLine(texture, tip, tip - new Vector2(Mathf.Cos(angle + 0.55f), Mathf.Sin(angle + 0.55f)) * 10f, 1);
                DrawLine(texture, tip, tip - new Vector2(Mathf.Cos(angle - 0.55f), Mathf.Sin(angle - 0.55f)) * 10f, 1);
            }

            return ToSprite(texture);
        }

        private static Sprite CreateGlyphSprite(bool flame)
        {
            Texture2D texture = CreateTexture(96, 96);
            Vector2 center = new Vector2(47.5f, 47.5f);

            for (int i = 0; i < 6; i++)
            {
                float angleA = Mathf.PI * 2f * i / 6f;
                float angleB = Mathf.PI * 2f * ((i + 2) % 6) / 6f;
                DrawLine(texture, center + Direction(angleA) * 26f, center + Direction(angleB) * 26f, flame ? 2 : 1);
            }

            return ToSprite(texture);
        }

        private static Sprite CreateConeSprite(bool edgeOnly)
        {
            const int width = 128;
            const int height = 96;
            Texture2D texture = CreateTexture(width, height);
            Vector2 origin = new Vector2(8f, height * 0.5f);
            float maxAngle = 32f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 toPoint = new Vector2(x, y) - origin;
                    float angle = Mathf.Abs(Mathf.Atan2(toPoint.y, toPoint.x) * Mathf.Rad2Deg);
                    float distance = toPoint.magnitude / width;
                    bool inside = toPoint.x > 0f && angle <= maxAngle && distance <= 0.92f;
                    bool edge = Mathf.Abs(angle - maxAngle) <= 2.2f || Mathf.Abs(distance - 0.92f) <= 0.025f;
                    float alpha = inside ? (edgeOnly ? (edge ? 0.9f : 0f) : Mathf.Lerp(0.55f, 0.1f, distance)) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture, 96f);
        }

        private static Sprite CreateLineSprite(bool core)
        {
            Texture2D texture = CreateTexture(64, 16);
            float halfHeight = core ? 1.5f : 5f;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    float distance = Mathf.Abs(y - 7.5f);
                    float fadeX = Mathf.Sin(x / 63f * Mathf.PI);
                    float alpha = distance <= halfHeight ? fadeX : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            return ToSprite(texture, 64f);
        }

        private static Sprite CreateWebSprite()
        {
            Texture2D texture = CreateTexture(128, 128);
            Vector2 center = new Vector2(63.5f, 63.5f);

            for (int i = 0; i < 10; i++)
            {
                float angle = Mathf.PI * 2f * i / 10f;
                DrawLine(texture, center, center + Direction(angle) * 52f, 1);
            }

            for (int ringIndex = 1; ringIndex <= 4; ringIndex++)
            {
                float radius = 12f + ringIndex * 10f;

                for (int i = 0; i < 10; i++)
                {
                    float angleA = Mathf.PI * 2f * i / 10f + ringIndex * 0.08f;
                    float angleB = Mathf.PI * 2f * (i + 1) / 10f + ringIndex * 0.08f;
                    DrawLine(texture, center + Direction(angleA) * radius, center + Direction(angleB) * radius, 1);
                }
            }

            return ToSprite(texture);
        }

        private static Texture2D CreateTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    texture.SetPixel(x, y, Color.clear);
            }

            return texture;
        }

        private static Sprite ToSprite(Texture2D texture, float pixelsPerUnit = 96f)
        {
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }

        private static void DrawLine(Texture2D texture, Vector2 from, Vector2 to, int radius)
        {
            int steps = Mathf.CeilToInt(Vector2.Distance(from, to) * 1.5f);

            for (int i = 0; i <= steps; i++)
                DrawDot(texture, Vector2.Lerp(from, to, i / (float)Mathf.Max(1, steps)), radius);
        }

        private static void DrawDot(Texture2D texture, Vector2 position, int radius)
        {
            int cx = Mathf.RoundToInt(position.x);
            int cy = Mathf.RoundToInt(position.y);

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px < 0 || px >= texture.width || py < 0 || py >= texture.height)
                        continue;

                    float distance = Mathf.Sqrt(x * x + y * y);

                    if (distance > radius)
                        continue;

                    float alpha = 1f - distance / (radius + 0.01f);
                    Color current = texture.GetPixel(px, py);
                    texture.SetPixel(px, py, new Color(1f, 1f, 1f, Mathf.Max(current.a, alpha)));
                }
            }
        }

        private static Vector2 Direction(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        private static float RandomRadius(int index)
        {
            return 22f + (index * 19 % 31);
        }
    }
}
