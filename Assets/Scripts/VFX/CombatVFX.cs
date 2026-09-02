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
        MoonMeteor,
        Burrow,
        Buff
    }

    public static class CombatVFX
    {
        private static readonly int DefaultSortingOrder = 1800;
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

        public static void PlayBurst(Vector2 position, CombatVFXKind kind, float size, float duration = 0.28f, int sortingOrder = 1800)
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

        public static GameObject PlayWarning(Vector2 position, CombatVFXKind kind, float size, float duration = 0.9f, int sortingOrder = 650)
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

        public static GameObject PlayCone(Vector2 position, Vector2 direction, CombatVFXKind kind, float range, bool impact, float duration, int sortingOrder = 1600)
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

        public static GameObject PlayBurrowWarning(Vector2 position, float size, float duration = 0.8f, int sortingOrder = 1450)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            GameObject root = new GameObject("VFX Burrow Warning");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.Burrow);
            Color secondary = GetSecondaryColor(CombatVFXKind.Burrow);
            SpriteRenderer dust = CreateRenderer(root.transform, "Dust", VFXSprites.SoftDisc, WithAlpha(main, 0.13f), sortingOrder);
            SpriteRenderer cracks = CreateRenderer(root.transform, "Ground Cracks", VFXSprites.GroundCracks, WithAlpha(secondary, 0.42f), sortingOrder + 1);
            SpriteRenderer ring = CreateRenderer(root.transform, "Pressure Ring", VFXSprites.WarningRing, WithAlpha(secondary, 0.24f), sortingOrder + 2);

            dust.transform.localScale = Vector3.one * size * 1.15f;
            cracks.transform.localScale = Vector3.one * size * 0.9f;
            ring.transform.localScale = Vector3.one * size;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.86f, 1.08f, 18f, false);
            return root;
        }

        public static void PlayBurrowEmerge(Vector2 position, float size, float duration = 0.36f, int sortingOrder = 1480)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            GameObject root = new GameObject("VFX Burrow Emerge");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.Burrow);
            Color secondary = GetSecondaryColor(CombatVFXKind.Burrow);
            SpriteRenderer dust = CreateRenderer(root.transform, "Dust Burst", VFXSprites.SoftDisc, WithAlpha(main, 0.3f), sortingOrder);
            SpriteRenderer cracks = CreateRenderer(root.transform, "Crack Burst", VFXSprites.GroundCracks, WithAlpha(secondary, 0.72f), sortingOrder + 1);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Pebbles", VFXSprites.Sparks, WithAlpha(secondary, 0.38f), sortingOrder + 2);

            dust.transform.localScale = Vector3.one * size * 1.2f;
            cracks.transform.localScale = Vector3.one * size;
            sparks.transform.localScale = Vector3.one * size * 0.72f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.78f, 1.28f, 90f, true);
        }

        public static void PlayLine(Vector2 from, Vector2 to, CombatVFXKind kind, float duration = 0.16f, float width = 0.08f, int sortingOrder = 1850)
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

        public static GameObject PlayBossCastAura(Transform target, CombatVFXKind kind, float size, float duration = 0.45f, int sortingOrder = 1500)
        {
            return PlayBossCastAura(target, Vector2.zero, kind, size, duration, sortingOrder);
        }

        public static GameObject PlayBossCastAura(Transform target, Vector2 offset, CombatVFXKind kind, float size, float duration = 0.45f, int sortingOrder = 1500)
        {
            if (target == null)
                return null;

            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject($"VFX Boss {kind} Cast Aura");
            root.transform.position = target.position + (Vector3)offset;

            Color main = GetMainColor(kind);
            Color secondary = GetSecondaryColor(kind);
            SpriteRenderer fill = CreateRenderer(root.transform, "Gather Fill", VFXSprites.SoftDisc, WithAlpha(main, 0.14f), sortingOrder);
            SpriteRenderer ring = CreateRenderer(root.transform, "Gather Ring", VFXSprites.WarningRing, WithAlpha(secondary, 0.58f), sortingOrder + 1);
            SpriteRenderer glyph = CreateRenderer(root.transform, "Gather Glyph", VFXSprites.Glyph, WithAlpha(secondary, 0.36f), sortingOrder + 2);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Gather Sparks", VFXSprites.Sparks, WithAlpha(main, 0.42f), sortingOrder + 3);

            fill.transform.localScale = Vector3.one * size * 0.95f;
            ring.transform.localScale = Vector3.one * size;
            glyph.transform.localScale = Vector3.one * size * 0.64f;
            sparks.transform.localScale = Vector3.one * size * 0.78f;

            CombatVFXFollow follow = root.AddComponent<CombatVFXFollow>();
            follow.Configure(target, offset);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.78f, 1.15f, 115f, true);
            return root;
        }

        public static void PlayExpandingRing(Vector2 position, CombatVFXKind kind, float startSize, float endSize, float duration = 0.35f, int sortingOrder = 1600)
        {
            startSize *= SizeMultiplier;
            endSize *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject($"VFX {kind} Expanding Ring");
            root.transform.position = position;

            Color main = GetMainColor(kind);
            Color secondary = GetSecondaryColor(kind);
            SpriteRenderer fill = CreateRenderer(root.transform, "Pressure Fill", VFXSprites.SoftDisc, WithAlpha(main, 0.11f), sortingOrder);
            SpriteRenderer edge = CreateRenderer(root.transform, "Pressure Edge", VFXSprites.WarningRing, WithAlpha(secondary, 0.72f), sortingOrder + 1);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Pressure Sparks", VFXSprites.Sparks, WithAlpha(secondary, 0.38f), sortingOrder + 2);

            fill.transform.localScale = Vector3.one;
            edge.transform.localScale = Vector3.one;
            sparks.transform.localScale = Vector3.one * 0.82f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, Mathf.Max(0.02f, startSize), Mathf.Max(startSize, endSize), 36f, true);
        }

        public static void PlayDirectionalStreak(Vector2 position, Vector2 direction, CombatVFXKind kind, float length = 0.8f, float width = 0.12f, float duration = 0.14f, int sortingOrder = 1700)
        {
            direction = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;
            length *= SizeMultiplier;
            width *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject($"VFX {kind} Directional Streak");
            root.transform.position = position;
            root.transform.right = direction;

            Color main = WithAlpha(GetMainColor(kind), 0.42f);
            Color secondary = WithAlpha(GetSecondaryColor(kind), 0.68f);
            SpriteRenderer trail = CreateRenderer(root.transform, "Motion Trail", VFXSprites.Line, main, sortingOrder);
            SpriteRenderer core = CreateRenderer(root.transform, "Motion Core", VFXSprites.LineCore, secondary, sortingOrder + 1);
            trail.transform.localScale = new Vector3(length, width, 1f);
            core.transform.localScale = new Vector3(length * 0.72f, width * 0.32f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 1f, 0.92f, 0f, true);
        }

        public static void PlaySilverMoonWave(Vector2 start, Vector2 direction, float length, float width, float duration = 0.24f, int sortingOrder = 1780)
        {
            direction = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;
            length *= SizeMultiplier;
            width *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            Vector2 center = start + direction * (length * 0.5f);
            GameObject root = new GameObject("VFX Selene Silver Moon Wave");
            root.transform.position = center;
            root.transform.right = direction;

            Color main = GetMainColor(CombatVFXKind.Frost);
            Color secondary = GetSecondaryColor(CombatVFXKind.Frost);
            SpriteRenderer trail = CreateRenderer(root.transform, "Silver Moon Trail", VFXSprites.Line, WithAlpha(main, 0.26f), sortingOrder);
            SpriteRenderer core = CreateRenderer(root.transform, "Silver Moon Core", VFXSprites.LineCore, WithAlpha(Color.white, 0.44f), sortingOrder + 1);
            SpriteRenderer moon = CreateRenderer(root.transform, "Silver Moon Crescent", VFXSprites.CrescentMoon, WithAlpha(secondary, 0.82f), sortingOrder + 2);
            SpriteRenderer wake = CreateRenderer(root.transform, "Silver Moon Wake", VFXSprites.MoonImpactShards, WithAlpha(main, 0.2f), sortingOrder + 3);

            trail.transform.localScale = new Vector3(length, width * 1.45f, 1f);
            core.transform.localScale = new Vector3(length * 0.86f, width * 0.36f, 1f);
            moon.transform.localPosition = Vector3.right * (length * 0.36f);
            moon.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            moon.transform.localScale = new Vector3(width * 2.7f, width * 2.7f, 1f);
            wake.transform.localScale = new Vector3(length * 0.52f, width * 2.2f, 1f);
            wake.transform.localPosition = Vector3.left * (length * 0.12f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 1.02f, 0.9f, 0f, true);
        }

        public static void PlayTeleportBurst(Vector2 position, float size = 0.85f, float duration = 0.24f, int sortingOrder = 1700)
        {
            PlayBurst(position, CombatVFXKind.ArcaneImpact, size, duration, sortingOrder);
            PlayExpandingRing(position, CombatVFXKind.ArcaneImpact, size * 0.35f, size * 1.35f, duration * 1.15f, sortingOrder - 1);
        }

        public static void PlayMoonMeteorWarning(Vector2 position, float radius, float duration = 0.55f, int sortingOrder = 760)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Moon Meteor Warning");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.MoonMeteor);
            Color secondary = GetSecondaryColor(CombatVFXKind.MoonMeteor);
            SpriteRenderer fill = CreateRenderer(root.transform, "Meteor Moonlight Fill", VFXSprites.SoftDisc, WithAlpha(main, 0.1f), sortingOrder);
            SpriteRenderer magicCircle = CreateRenderer(root.transform, "Meteor Moon Magic Circle", VFXSprites.MoonMagicCircle, WithAlpha(secondary, 0.72f), sortingOrder + 1);
            SpriteRenderer centerMoon = CreateRenderer(root.transform, "Meteor Center Moon", VFXSprites.FullMoon, WithAlpha(secondary, 0.62f), sortingOrder + 2);
            SpriteRenderer glimmer = CreateRenderer(root.transform, "Meteor Fine Stars", VFXSprites.MoonImpactShards, WithAlpha(main, 0.2f), sortingOrder + 3);

            float diameter = radius * 2f;
            fill.transform.localScale = Vector3.one * diameter * 1.02f;
            magicCircle.transform.localScale = Vector3.one * diameter * 1.2f;
            centerMoon.transform.localScale = Vector3.one * radius * 0.42f;
            centerMoon.transform.localPosition = Vector3.up * radius * 0.18f;
            glimmer.transform.localScale = Vector3.one * radius * 0.86f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.92f, 1.04f, -12f, true);
        }

        public static GameObject PlayMoonMeteorFall(Vector2 targetPosition, float radius, float duration = 0.55f, int sortingOrder = 1880)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            Vector2 startPosition = GetMeteorStartPosition(targetPosition, radius);
            GameObject root = new GameObject("VFX Moon Meteor Falling Body");
            root.transform.position = startPosition;

            Color main = GetMainColor(CombatVFXKind.MoonMeteor);
            Color secondary = GetSecondaryColor(CombatVFXKind.MoonMeteor);
            SpriteRenderer aura = CreateRenderer(root.transform, "Meteor Aura", VFXSprites.SoftDisc, WithAlpha(main, 0.36f), sortingOrder);
            SpriteRenderer core = CreateRenderer(root.transform, "Meteor Moon Core", VFXSprites.FullMoon, WithAlpha(secondary, 0.98f), sortingOrder + 2);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Meteor Sparks", VFXSprites.MoonImpactShards, WithAlpha(main, 0.5f), sortingOrder + 3);
            SpriteRenderer tail = CreateRenderer(root.transform, "Meteor Tail", VFXSprites.LineCore, WithAlpha(Color.white, 0.78f), sortingOrder - 1);
            SpriteRenderer tailGlow = CreateRenderer(root.transform, "Meteor Tail Glow", VFXSprites.Line, WithAlpha(secondary, 0.44f), sortingOrder - 2);

            aura.transform.localScale = Vector3.one * radius * 0.92f;
            core.transform.localScale = Vector3.one * radius * 0.58f;
            sparks.transform.localScale = Vector3.one * radius * 0.48f;

            Vector2 fallDirection = (targetPosition - startPosition).sqrMagnitude <= 0.001f
                ? Vector2.down
                : (targetPosition - startPosition).normalized;
            float angle = Mathf.Atan2(fallDirection.y, fallDirection.x) * Mathf.Rad2Deg;
            float tailLength = Mathf.Clamp(Vector2.Distance(startPosition, targetPosition) * 0.32f, radius * 1.8f, radius * 4.4f);
            Vector3 tailOffset = -(Vector3)fallDirection * tailLength * 0.5f;

            tail.transform.localPosition = tailOffset;
            tail.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            tail.transform.localScale = new Vector3(tailLength, radius * 0.1f, 1f);
            tailGlow.transform.localPosition = tailOffset;
            tailGlow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            tailGlow.transform.localScale = new Vector3(tailLength * 1.12f, radius * 0.3f, 1f);

            MoonMeteorFallEffect effect = root.AddComponent<MoonMeteorFallEffect>();
            effect.Play(startPosition, targetPosition, Mathf.Max(0.08f, duration), radius, aura, core, sparks, tail, tailGlow);
            return root;
        }

        public static void PlayMoonMeteorImpact(Vector2 position, float radius, float duration = 0.42f, int sortingOrder = 1820)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Moon Meteor Impact");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.MoonMeteor);
            Color secondary = GetSecondaryColor(CombatVFXKind.MoonMeteor);
            SpriteRenderer core = CreateRenderer(root.transform, "Meteor Impact Glow", VFXSprites.SoftDisc, WithAlpha(main, 0.48f), sortingOrder);
            SpriteRenderer magicCircle = CreateRenderer(root.transform, "Meteor Impact Magic Circle", VFXSprites.MoonMagicCircle, WithAlpha(secondary, 0.86f), sortingOrder + 1);
            SpriteRenderer shards = CreateRenderer(root.transform, "Meteor Impact Shards", VFXSprites.MoonImpactShards, WithAlpha(main, 0.76f), sortingOrder + 2);
            SpriteRenderer rays = CreateRenderer(root.transform, "Meteor Moon Rays", VFXSprites.Sparks, WithAlpha(Color.white, 0.34f), sortingOrder + 3);
            SpriteRenderer moon = CreateRenderer(root.transform, "Meteor Impact Moon", VFXSprites.FullMoon, WithAlpha(secondary, 0.98f), sortingOrder + 4);
            SpriteRenderer beam = CreateRenderer(root.transform, "Meteor Moon Beam", VFXSprites.Line, WithAlpha(secondary, 0.56f), sortingOrder + 5);
            SpriteRenderer beamCore = CreateRenderer(root.transform, "Meteor Moon Beam Core", VFXSprites.LineCore, WithAlpha(Color.white, 0.9f), sortingOrder + 6);

            core.transform.localScale = Vector3.one * radius * 1.62f;
            magicCircle.transform.localScale = Vector3.one * radius * 2.45f;
            shards.transform.localScale = Vector3.one * radius * 1.4f;
            rays.transform.localScale = Vector3.one * radius * 1.18f;
            moon.transform.localScale = Vector3.one * radius * 0.72f;
            moon.transform.localPosition = Vector3.up * radius * 0.56f;
            beam.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            beam.transform.localPosition = Vector3.up * radius * 1.18f;
            beam.transform.localScale = new Vector3(radius * 3.35f, radius * 0.32f, 1f);
            beamCore.transform.rotation = beam.transform.rotation;
            beamCore.transform.localPosition = beam.transform.localPosition;
            beamCore.transform.localScale = new Vector3(radius * 3f, radius * 0.09f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.72f, 1.12f, 28f, true);
        }

        private static Vector2 GetMeteorStartPosition(Vector2 targetPosition, float radius)
        {
            Camera camera = Camera.main;

            if (camera == null)
                return targetPosition + new Vector2(-radius * 1.1f, radius * 4.2f);

            float screenX = camera.WorldToViewportPoint(targetPosition).x;
            Vector3 topPoint = camera.ViewportToWorldPoint(new Vector3(screenX, 1.18f, Mathf.Abs(camera.transform.position.z)));
            Vector2 start = new Vector2(topPoint.x, topPoint.y);
            start.x -= radius * 1.15f;

            if (start.y < targetPosition.y + radius * 3.2f)
                start.y = targetPosition.y + radius * 3.2f;

            return start;
        }

        public static void PlayChainLightning(Vector2 from, Vector2 to, float duration = 0.22f, float width = 0.07f, int sortingOrder = 1900)
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

        public static void PlayChainLightningImpact(Vector2 position, float size = 0.22f, float duration = 0.12f, int sortingOrder = 1900)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            Color glowColor = WithAlpha(GetMainColor(CombatVFXKind.ChainLightning), 0.55f);
            Color coreColor = GetSecondaryColor(CombatVFXKind.ChainLightning);
            GameObject root = new GameObject("VFX Chain Lightning Spark");
            root.transform.position = position;

            LineRenderer[] lines = new LineRenderer[4];

            for (int i = 0; i < lines.Length; i++)
            {
                float angle = i * 90f + Random.Range(-18f, 18f);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                LineRenderer line = CreateLightningRenderer(root, $"Spark {i + 1}", 0.025f * size, i % 2 == 0 ? coreColor : glowColor, sortingOrder + i);
                line.positionCount = 2;
                line.SetPosition(0, (Vector2)position - direction * size * 0.36f);
                line.SetPosition(1, (Vector2)position + direction * size);
                lines[i] = line;
            }

            CombatVFXLineEffect effect = root.AddComponent<CombatVFXLineEffect>();
            effect.Play(duration, lines);
        }

        public static GameObject CreateZoneVisual(Transform parent, CombatVFXKind kind, float radius, Color tint, int sortingOrder = 620)
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

        public static GameObject CreateSeleneNebulaZoneVisual(Vector2 position, float radius, int sortingOrder = 615)
        {
            radius *= SizeMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Selene Nebula Zone");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.FrostZone);
            Color secondary = GetSecondaryColor(CombatVFXKind.FrostZone);
            SpriteRenderer fill = CreateRenderer(root.transform, "Nebula Fill", VFXSprites.ZoneFill, WithAlpha(main, 0.032f), sortingOrder);
            SpriteRenderer edge = CreateRenderer(root.transform, "Nebula Edge", VFXSprites.Ring, WithAlpha(secondary, 0.11f), sortingOrder + 1);
            SpriteRenderer detail = CreateRenderer(root.transform, "Nebula Pentagram", VFXSprites.Pentagram, WithAlpha(secondary, 0.105f), sortingOrder + 2);

            float diameter = radius * 2f;
            fill.transform.localScale = Vector3.one * diameter;
            edge.transform.localScale = Vector3.one * diameter;
            detail.transform.localScale = Vector3.one * diameter * 0.56f;

            CombatVFXLoop loop = root.AddComponent<CombatVFXLoop>();
            loop.Configure(detail.transform, edge, fill, 1.2f, 0.006f, 0.18f);
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
                case CombatVFXKind.MoonMeteor:
                    return new Color(0.78f, 0.62f, 1f, 0.84f);
                case CombatVFXKind.Burrow:
                    return new Color(0.38f, 0.25f, 0.16f, 0.7f);
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
                case CombatVFXKind.ConeImpact:
                case CombatVFXKind.TargetImpact:
                    return new Color(1f, 0.24f, 0.08f, 0.76f);
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
                case CombatVFXKind.MoonMeteor:
                    return new Color(0.98f, 0.94f, 1f, 0.96f);
                case CombatVFXKind.Burrow:
                    return new Color(0.95f, 0.58f, 0.28f, 0.82f);
                case CombatVFXKind.Vampirism:
                    return new Color(0.92f, 1f, 0.65f, 0.86f);
                case CombatVFXKind.ChainLightning:
                    return new Color(0.98f, 1f, 0.58f, 0.92f);
                case CombatVFXKind.WebZone:
                    return new Color(0.95f, 0.98f, 1f, 0.76f);
                case CombatVFXKind.ConeImpact:
                case CombatVFXKind.TargetImpact:
                    return new Color(1f, 0.82f, 0.2f, 0.9f);
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

        private static void AddMoonCircleMarkers(Transform parent, float radius, Color color, int sortingOrder, float alpha = 0.42f)
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = Mathf.PI * 0.5f * i + Mathf.PI * 0.25f;
                SpriteRenderer marker = CreateRenderer(parent, $"Moon Marker {i + 1}", VFXSprites.CrescentMoon, WithAlpha(color, alpha), sortingOrder + i);
                marker.transform.localPosition = Direction(angle) * radius * 0.92f;
                marker.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg - 90f);
                marker.transform.localScale = Vector3.one * radius * 0.24f;
            }
        }

        private static Vector2 Direction(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
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

    public class CombatVFXFollow : MonoBehaviour
    {
        private Transform target;
        private Vector2 offset;

        public void Configure(Transform followTarget)
        {
            Configure(followTarget, Vector2.zero);
        }

        public void Configure(Transform followTarget, Vector2 followOffset)
        {
            target = followTarget;
            offset = followOffset;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            transform.position = target.position + (Vector3)offset;
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

    public class MoonMeteorFallEffect : MonoBehaviour
    {
        private Vector2 startPosition;
        private Vector2 targetPosition;
        private float duration;
        private float elapsed;
        private float radius;
        private SpriteRenderer[] renderers;
        private Color[] initialColors;

        public void Play(
            Vector2 start,
            Vector2 target,
            float fallDuration,
            float effectRadius,
            params SpriteRenderer[] targetRenderers)
        {
            startPosition = start;
            targetPosition = target;
            duration = Mathf.Max(0.08f, fallDuration);
            radius = Mathf.Max(0.1f, effectRadius);
            renderers = targetRenderers;
            initialColors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
                initialColors[i] = renderers[i] == null ? Color.clear : renderers[i].color;
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            Vector2 arcOffset = Vector2.up * Mathf.Sin(progress * Mathf.PI) * radius * 0.28f;
            transform.position = Vector2.Lerp(startPosition, targetPosition, eased) + arcOffset;
            transform.Rotate(0f, 0f, 300f * Time.deltaTime);

            float charge = Mathf.Lerp(0.62f, 1.1f, progress);
            transform.localScale = Vector3.one * charge;
            float alpha = progress < 0.86f ? 1f : Mathf.Lerp(1f, 0f, (progress - 0.86f) / 0.14f);

            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = renderers[i];

                if (spriteRenderer == null)
                    continue;

                Color color = initialColors[i];
                color.a *= alpha;
                spriteRenderer.color = color;
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
        private float alphaPulseAmount = 1f;
        private Color edgeBaseColor;
        private Color fillBaseColor;

        public void Configure(Transform detailTransform, SpriteRenderer edgeRenderer, SpriteRenderer fillRenderer, float speed, float pulse)
        {
            Configure(detailTransform, edgeRenderer, fillRenderer, speed, pulse, 1f);
        }

        public void Configure(Transform detailTransform, SpriteRenderer edgeRenderer, SpriteRenderer fillRenderer, float speed, float pulse, float alphaPulse)
        {
            rotatingPart = detailTransform;
            edge = edgeRenderer;
            fill = fillRenderer;
            rotateSpeed = speed;
            pulseAmount = pulse;
            alphaPulseAmount = Mathf.Clamp01(alphaPulse);

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
            float edgePulse = Mathf.Lerp(1f, 0.82f + Mathf.Sin(Time.time * 3f) * 0.14f, alphaPulseAmount);
            float fillPulse = Mathf.Lerp(1f, 0.82f + Mathf.Sin(Time.time * 2f) * 0.1f, alphaPulseAmount);

            if (edge != null)
                edge.color = SetAlpha(edgeBaseColor, edgeBaseColor.a * edgePulse);

            if (fill != null)
                fill.color = SetAlpha(fillBaseColor, fillBaseColor.a * fillPulse);
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
        private static Sprite groundCracks;
        private static Sprite crescentMoon;
        private static Sprite pentagram;
        private static Sprite fullMoon;
        private static Sprite moonMagicCircle;
        private static Sprite moonImpactShards;

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
        public static Sprite GroundCracks => groundCracks ??= CreateGroundCrackSprite();
        public static Sprite CrescentMoon => crescentMoon ??= CreateCrescentMoonSprite();
        public static Sprite Pentagram => pentagram ??= CreatePentagramSprite();
        public static Sprite FullMoon => fullMoon ??= CreateFullMoonSprite();
        public static Sprite MoonMagicCircle => moonMagicCircle ??= CreateMoonMagicCircleSprite();
        public static Sprite MoonImpactShards => moonImpactShards ??= CreateMoonImpactShardsSprite();

        public static Sprite GetBurstSprite(CombatVFXKind kind)
        {
            switch (kind)
            {
                case CombatVFXKind.Explosion:
                case CombatVFXKind.TargetImpact:
                case CombatVFXKind.MoonMeteor:
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

        private static Sprite CreateCrescentMoonSprite()
        {
            Texture2D texture = CreateTexture(96, 96);
            Vector2 center = new Vector2(47.5f, 47.5f);
            Vector2 cutoutCenter = center + new Vector2(11f, 4f);

            for (int y = 0; y < 96; y++)
            {
                for (int x = 0; x < 96; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float outer = Vector2.Distance(point, center);
                    float cutout = Vector2.Distance(point, cutoutCenter);
                    float alpha = Mathf.Clamp01((30f - outer) / 2.2f) * Mathf.Clamp01((cutout - 21f) / 2.2f);

                    if (alpha <= 0f)
                        continue;

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            DrawDot(texture, center + new Vector2(17f, 15f), 2);
            return ToSprite(texture);
        }

        private static Sprite CreateFullMoonSprite()
        {
            Texture2D texture = CreateTexture(128, 128);
            Vector2 center = new Vector2(63.5f, 63.5f);

            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distance = Vector2.Distance(point, center);
                    float normalized = distance / 42f;

                    if (normalized > 1.08f)
                        continue;

                    float edge = Mathf.Clamp01((1.08f - normalized) / 0.08f);
                    float glow = normalized <= 1f ? Mathf.Lerp(0.98f, 0.7f, normalized) : edge * 0.45f;
                    float craterA = Mathf.Clamp01(1f - Vector2.Distance(point, center + new Vector2(-10f, 9f)) / 13f) * 0.16f;
                    float craterB = Mathf.Clamp01(1f - Vector2.Distance(point, center + new Vector2(12f, -7f)) / 10f) * 0.13f;
                    float craterC = Mathf.Clamp01(1f - Vector2.Distance(point, center + new Vector2(8f, 16f)) / 7f) * 0.1f;
                    float alpha = Mathf.Clamp01(glow - craterA - craterB - craterC);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            DrawRing(texture, center, 43f, 1, 1f);
            return ToSprite(texture);
        }

        private static Sprite CreateMoonMagicCircleSprite()
        {
            Texture2D texture = CreateTexture(192, 192);
            Vector2 center = new Vector2(95.5f, 95.5f);

            DrawRing(texture, center, 84f, 1, 1f);
            DrawRing(texture, center, 78f, 1, 0.78f);
            DrawRing(texture, center, 49f, 1, 1f);
            DrawRing(texture, center, 42f, 1, 0.64f);

            for (int i = 0; i < 12; i++)
            {
                float angle = Mathf.PI * 2f * i / 12f;
                float length = i % 3 == 0 ? 10f : 6f;
                Vector2 outer = center + Direction(angle) * 84f;
                DrawLine(texture, outer - Direction(angle) * length, outer + Direction(angle) * 2f, 1);
            }

            for (int i = 0; i < 4; i++)
            {
                float angle = Mathf.PI * 0.5f * i + Mathf.PI * 0.25f;
                Vector2 position = center + Direction(angle) * 67f;
                DrawCrescent(texture, position, angle - Mathf.PI * 0.5f, 9f);
            }

            DrawDot(texture, center + Vector2.up * 70f, 2);
            DrawDot(texture, center + Vector2.down * 70f, 2);
            DrawDot(texture, center + Vector2.left * 70f, 2);
            DrawDot(texture, center + Vector2.right * 70f, 2);
            return ToSprite(texture);
        }

        private static Sprite CreateMoonImpactShardsSprite()
        {
            Texture2D texture = CreateTexture(160, 160);
            Vector2 center = new Vector2(79.5f, 79.5f);

            for (int i = 0; i < 18; i++)
            {
                float angle = Mathf.PI * 2f * i / 18f + (i % 2) * 0.08f;
                float start = 16f + (i * 11 % 15);
                float end = 42f + (i * 17 % 34);
                int width = i % 3 == 0 ? 2 : 1;
                DrawLine(texture, center + Direction(angle) * start, center + Direction(angle) * end, width);
            }

            for (int i = 0; i < 10; i++)
            {
                float angle = Mathf.PI * 2f * i / 10f + 0.18f;
                DrawDot(texture, center + Direction(angle) * (50f + i % 3 * 8f), 1);
            }

            return ToSprite(texture);
        }

        private static Sprite CreatePentagramSprite()
        {
            Texture2D texture = CreateTexture(96, 96);
            Vector2 center = new Vector2(47.5f, 47.5f);
            Vector2[] points = new Vector2[5];

            for (int i = 0; i < points.Length; i++)
            {
                float angle = -Mathf.PI * 0.5f + Mathf.PI * 2f * i / points.Length;
                points[i] = center + Direction(angle) * 31f;
            }

            int[] order = { 0, 2, 4, 1, 3, 0 };
            for (int i = 0; i < order.Length - 1; i++)
                DrawLine(texture, points[order[i]], points[order[i + 1]], 1);

            DrawRing(texture, center, 34f, 1, 1f);
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

        private static Sprite CreateGroundCrackSprite()
        {
            Texture2D texture = CreateTexture(128, 128);
            Vector2 center = new Vector2(63.5f, 63.5f);
            float[] angles = { 0.12f, 0.9f, 1.7f, 2.55f, 3.35f, 4.15f, 5.02f, 5.76f };

            for (int i = 0; i < angles.Length; i++)
            {
                float length = 28f + (i % 3) * 8f;
                Vector2 start = center + Direction(angles[i]) * 8f;
                Vector2 end = center + Direction(angles[i]) * length;
                DrawJaggedLine(texture, start, end, i);
            }

            DrawRing(texture, center, 18f, 1);
            DrawRing(texture, center, 35f, 1, 0.55f);
            return ToSprite(texture);
        }

        private static void DrawJaggedLine(Texture2D texture, Vector2 from, Vector2 to, int seed)
        {
            Vector2 previous = from;
            int segments = 4;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 point = Vector2.Lerp(from, to, t);
                Vector2 normal = Direction(Mathf.Atan2(to.y - from.y, to.x - from.x) + Mathf.PI * 0.5f);
                point += normal * (((seed + i) % 2 == 0) ? 3f : -3f);
                DrawLine(texture, previous, point, i == segments ? 1 : 2);
                previous = point;
            }
        }

        private static void DrawRing(Texture2D texture, Vector2 center, float radius, int width, float arcRatio = 1f)
        {
            int steps = Mathf.RoundToInt(160f * Mathf.Clamp01(arcRatio));

            for (int i = 0; i < steps; i++)
            {
                float t = i / 160f;
                float angle = t * Mathf.PI * 2f;
                DrawDot(texture, center + Direction(angle) * radius, width);
            }
        }

        private static void DrawCrescent(Texture2D texture, Vector2 center, float rotation, float radius)
        {
            Vector2 cutoutOffset = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * radius * 0.42f;

            for (int y = Mathf.FloorToInt(center.y - radius * 1.2f); y <= Mathf.CeilToInt(center.y + radius * 1.2f); y++)
            {
                for (int x = Mathf.FloorToInt(center.x - radius * 1.2f); x <= Mathf.CeilToInt(center.x + radius * 1.2f); x++)
                {
                    if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                        continue;

                    Vector2 point = new Vector2(x, y);
                    float outer = Vector2.Distance(point, center);
                    float cutout = Vector2.Distance(point, center + cutoutOffset);
                    float alpha = Mathf.Clamp01((radius - outer) / 1.2f) * Mathf.Clamp01((cutout - radius * 0.68f) / 1.2f);

                    if (alpha <= 0f)
                        continue;

                    Color current = texture.GetPixel(x, y);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Max(current.a, alpha)));
                }
            }
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
