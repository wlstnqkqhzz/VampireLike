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

        public static void PlayShadowTeleportBurst(Vector2 position, float size = 0.85f, float duration = 0.24f, int sortingOrder = 1700)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Shadow Teleport Burst");
            root.transform.position = position;

            Color shadow = new Color(0.16f, 0.03f, 0.24f, 0.78f);
            Color edge = new Color(0.82f, 0.18f, 1f, 0.88f);
            SpriteRenderer smoke = CreateRenderer(root.transform, "Shadow Smoke", VFXSprites.SoftDisc, WithAlpha(shadow, 0.62f), sortingOrder);
            SpriteRenderer ring = CreateRenderer(root.transform, "Shadow Tear Ring", VFXSprites.WarningRing, WithAlpha(edge, 0.56f), sortingOrder + 1);
            SpriteRenderer slashA = CreateRenderer(root.transform, "Shadow Tear A", VFXSprites.LineCore, WithAlpha(edge, 0.82f), sortingOrder + 2);
            SpriteRenderer slashB = CreateRenderer(root.transform, "Shadow Tear B", VFXSprites.LineCore, WithAlpha(edge, 0.58f), sortingOrder + 3);

            smoke.transform.localScale = Vector3.one * size * 1.2f;
            ring.transform.localScale = Vector3.one * size * 0.9f;
            slashA.transform.localRotation = Quaternion.Euler(0f, 0f, 28f);
            slashB.transform.localRotation = Quaternion.Euler(0f, 0f, -38f);
            slashA.transform.localScale = new Vector3(size * 1.15f, size * 0.08f, 1f);
            slashB.transform.localScale = new Vector3(size * 0.92f, size * 0.06f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.72f, 1.22f, -180f, true);
        }

        public static GameObject PlayShadowTeleportArrivalWarning(Vector2 position, float size = 0.72f, float duration = 0.2f, int sortingOrder = 1450)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Shadow Teleport Arrival Warning");
            root.transform.position = position;

            Color shadow = new Color(0.1f, 0.02f, 0.16f, 0.62f);
            Color edge = new Color(0.74f, 0.1f, 1f, 0.7f);
            SpriteRenderer puddle = CreateRenderer(root.transform, "Arrival Shadow", VFXSprites.SoftDisc, WithAlpha(shadow, 0.48f), sortingOrder);
            SpriteRenderer mark = CreateRenderer(root.transform, "Arrival Slash Mark", VFXSprites.LineCore, WithAlpha(edge, 0.68f), sortingOrder + 1);

            puddle.transform.localScale = new Vector3(size * 1.2f, size * 0.55f, 1f);
            mark.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            mark.transform.localScale = new Vector3(size * 1.08f, size * 0.08f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.86f, 1.08f, 0f, true);
            return root;
        }

        public static void PlayWarlockTeleportBurst(Vector2 position, float size = 0.95f, float duration = 0.3f, int sortingOrder = 1700)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Warlock Teleport Burst");
            root.transform.position = position;

            Color voidColor = new Color(0.08f, 0.01f, 0.15f, 0.72f);
            Color violet = new Color(0.68f, 0.18f, 1f, 0.86f);
            Color pale = new Color(0.92f, 0.76f, 1f, 0.68f);

            SpriteRenderer smoke = CreateRenderer(root.transform, "Void Smoke", VFXSprites.SoftDisc, WithAlpha(voidColor, 0.62f), sortingOrder);
            SpriteRenderer glyph = CreateRenderer(root.transform, "Warlock Glyph", VFXSprites.Pentagram, WithAlpha(violet, 0.7f), sortingOrder + 1);
            SpriteRenderer ring = CreateRenderer(root.transform, "Rift Ring", VFXSprites.WarningRing, WithAlpha(pale, 0.5f), sortingOrder + 2);
            SpriteRenderer sparks = CreateRenderer(root.transform, "Curse Sparks", VFXSprites.Sparks, WithAlpha(violet, 0.44f), sortingOrder + 3);

            smoke.transform.localScale = Vector3.one * size * 1.2f;
            glyph.transform.localScale = Vector3.one * size * 0.74f;
            ring.transform.localScale = Vector3.one * size;
            sparks.transform.localScale = Vector3.one * size * 0.8f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.72f, 1.18f, -220f, true);
        }

        public static GameObject PlayWarlockTeleportWarning(Vector2 position, float size, float duration = 0.55f, int sortingOrder = 1450)
        {
            size *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Warlock Teleport Warning");
            root.transform.position = position;

            Color shadow = new Color(0.07f, 0.01f, 0.13f, 0.58f);
            Color violet = new Color(0.68f, 0.2f, 1f, 0.76f);
            SpriteRenderer puddle = CreateRenderer(root.transform, "Void Landing Mark", VFXSprites.SoftDisc, WithAlpha(shadow, 0.42f), sortingOrder);
            SpriteRenderer glyph = CreateRenderer(root.transform, "Landing Curse Glyph", VFXSprites.Pentagram, WithAlpha(violet, 0.42f), sortingOrder + 1);
            SpriteRenderer ring = CreateRenderer(root.transform, "Thin Rift Ring", VFXSprites.WarningRing, WithAlpha(violet, 0.3f), sortingOrder + 2);

            puddle.transform.localScale = Vector3.one * size * 1.1f;
            glyph.transform.localScale = Vector3.one * size * 0.72f;
            ring.transform.localScale = Vector3.one * size * 0.9f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.88f, 1.08f, 74f, true);
            return root;
        }

        public static GameObject PlayWarlockCurseWarning(Vector2 position, float radius, float duration = 0.9f, int sortingOrder = 760)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Warlock Curse Warning");
            root.transform.position = position;

            Color shadow = new Color(0.09f, 0f, 0.15f, 0.54f);
            Color violet = new Color(0.58f, 0.18f, 0.92f, 0.78f);
            Color spirit = new Color(0.9f, 0.72f, 1f, 0.88f);

            SpriteRenderer fill = CreateRenderer(root.transform, "Curse Pool", VFXSprites.ZoneFill, WithAlpha(shadow, 0.18f), sortingOrder);
            SpriteRenderer glyph = CreateRenderer(root.transform, "Curse Pentagram", VFXSprites.Pentagram, WithAlpha(violet, 0.34f), sortingOrder + 1);
            SpriteRenderer ring = CreateRenderer(root.transform, "Curse Boundary", VFXSprites.WarningRing, WithAlpha(violet, 0.24f), sortingOrder + 2);

            float diameter = radius * 2f;
            fill.transform.localScale = Vector3.one * diameter * 1.08f;
            glyph.transform.localScale = Vector3.one * diameter * 0.84f;
            ring.transform.localScale = Vector3.one * diameter;

            for (int i = 0; i < 7; i++)
            {
                float angle = Mathf.PI * 2f * i / 7f + Random.Range(-0.18f, 0.18f);
                SpriteRenderer skull = CreateRenderer(root.transform, $"Curse Skull Aura {i + 1}", VFXSprites.Skull, WithAlpha(spirit, 0.72f), sortingOrder + 6 + i);
                skull.transform.localPosition = Direction(angle) * radius * Random.Range(0.36f, 0.84f);
                skull.transform.localScale = Vector3.one * radius * Random.Range(0.38f, 0.5f);
            }

            WarlockCurseAuraEffect effect = root.AddComponent<WarlockCurseAuraEffect>();
            effect.Play(duration, 18f);
            return root;
        }

        public static void PlayWarlockCurseImpact(Vector2 position, float radius, float duration = 0.35f, int sortingOrder = 1820)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Warlock Curse Impact");
            root.transform.position = position;

            Color flash = new Color(0.78f, 0.44f, 1f, 0.72f);
            Color shadow = new Color(0.09f, 0f, 0.14f, 0.66f);
            SpriteRenderer burst = CreateRenderer(root.transform, "Curse Flash", VFXSprites.Sparks, WithAlpha(flash, 0.72f), sortingOrder + 2);
            SpriteRenderer ring = CreateRenderer(root.transform, "Curse Shock Ring", VFXSprites.WarningRing, WithAlpha(flash, 0.58f), sortingOrder + 1);
            SpriteRenderer smoke = CreateRenderer(root.transform, "Curse Smoke", VFXSprites.SoftDisc, WithAlpha(shadow, 0.38f), sortingOrder);

            float diameter = radius * 2f;
            smoke.transform.localScale = Vector3.one * diameter * 1.15f;
            ring.transform.localScale = Vector3.one * diameter * 0.75f;
            burst.transform.localScale = Vector3.one * diameter * 0.9f;

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.78f, 1.24f, 130f, true);
        }

        public static void PlayNinjaTeleportAfterimage(SpriteRenderer source, bool appearing, float duration = 0.24f, int sortingOrder = 1700)
        {
            if (source == null || source.sprite == null)
                return;

            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject(appearing ? "VFX Ninja Teleport Appearing" : "VFX Ninja Teleport Vanishing");
            root.transform.position = source.transform.position;
            root.transform.rotation = source.transform.rotation;
            root.transform.localScale = source.transform.lossyScale;

            Color bodyColor = appearing
                ? new Color(0.36f, 0.88f, 1f, 0.42f)
                : new Color(0.78f, 0.18f, 1f, 0.4f);
            Color lineColor = appearing
                ? new Color(0.72f, 1f, 1f, 0.88f)
                : new Color(0.98f, 0.28f, 1f, 0.82f);

            SpriteRenderer body = CreateRenderer(root.transform, "Afterimage Body", source.sprite, WithAlpha(bodyColor, bodyColor.a), sortingOrder);
            body.flipX = source.flipX;
            body.flipY = source.flipY;

            int lineCount = 14;
            SpriteRenderer[] lines = new SpriteRenderer[lineCount];
            float height = Mathf.Max(0.08f, source.sprite.bounds.size.y);
            float width = Mathf.Max(0.08f, source.sprite.bounds.size.x);
            float startY = -height * 0.42f;
            float step = height * 0.84f / Mathf.Max(1, lineCount - 1);

            for (int i = 0; i < lineCount; i++)
            {
                SpriteRenderer line = CreateRenderer(root.transform, $"Afterimage Scanline {i + 1}", VFXSprites.LineCore, WithAlpha(lineColor, lineColor.a), sortingOrder + 1 + i);
                float y = startY + step * i;
                float jitter = Mathf.Sin(i * 12.17f) * width * 0.04f;
                line.transform.localPosition = new Vector3(jitter, y, 0f);
                line.transform.localScale = new Vector3(width * (0.78f + (i % 3) * 0.08f), 0.018f, 1f);
                lines[i] = line;
            }

            NinjaTeleportAfterimageEffect effect = root.AddComponent<NinjaTeleportAfterimageEffect>();
            effect.Play(duration, appearing, body, lines);
        }

        public static GameObject PlayShadowSlashWarning(Vector2 origin, Vector2 direction, float range, float angle, float duration, int sortingOrder = 1480)
        {
            return PlayShadowSlash(origin, direction, range, angle, duration, false, sortingOrder);
        }

        public static GameObject PlayShadowSlashImpact(Vector2 origin, Vector2 direction, float range, float angle, float duration, int sortingOrder = 1650)
        {
            return PlayShadowSlash(origin, direction, range, angle, duration, true, sortingOrder);
        }

        private static GameObject PlayShadowSlash(Vector2 origin, Vector2 direction, float range, float angle, float duration, bool impact, int sortingOrder)
        {
            range *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;
            direction = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;

            float slashAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector2 center = origin + direction * range * 0.52f;
            GameObject root = new GameObject(impact ? "VFX Shadow Slash Impact" : "VFX Shadow Slash Warning");
            root.transform.position = center;
            root.transform.rotation = Quaternion.Euler(0f, 0f, slashAngle);

            Color dark = new Color(0.18f, 0.01f, 0.08f, impact ? 0.58f : 0.24f);
            Color red = new Color(1f, 0.04f, 0.13f, impact ? 0.92f : 0.46f);
            Color violet = new Color(0.74f, 0.18f, 1f, impact ? 0.72f : 0.34f);
            SpriteRenderer shadow = CreateRenderer(root.transform, "Slash Shadow", VFXSprites.SoftDisc, WithAlpha(dark, dark.a), sortingOrder);
            SpriteRenderer arc = CreateRenderer(root.transform, "Slash Arc", VFXSprites.SlashArc, WithAlpha(red, red.a), sortingOrder + 1);
            SpriteRenderer core = CreateRenderer(root.transform, "Slash Core", VFXSprites.LineCore, WithAlpha(violet, violet.a), sortingOrder + 2);
            SpriteRenderer trail = CreateRenderer(root.transform, "Slash Trail", VFXSprites.Line, WithAlpha(red, impact ? 0.42f : 0.2f), sortingOrder);

            shadow.transform.localPosition = Vector3.left * range * 0.06f;
            shadow.transform.localScale = new Vector3(range * 0.82f, range * 0.44f, 1f);
            arc.transform.localScale = Vector3.one * range * (impact ? 1.02f : 0.92f);
            core.transform.localPosition = Vector3.right * range * 0.08f;
            core.transform.localRotation = Quaternion.Euler(0f, 0f, impact ? -8f : -5f);
            core.transform.localScale = new Vector3(range * 0.72f, range * 0.035f, 1f);
            trail.transform.localPosition = Vector3.left * range * 0.02f;
            trail.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(angle * 0.22f, 8f, 18f));
            trail.transform.localScale = new Vector3(range * 0.52f, range * 0.18f, 1f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, impact ? 0.72f : 0.92f, impact ? 1.18f : 1.02f, impact ? 36f : 6f, true);
            return root;
        }

        public static void PlayMoonMeteorWarning(Vector2 position, float radius, float duration = 0.55f, int sortingOrder = 760)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Moon Meteor Warning");
            root.transform.position = position;

            SpriteRenderer spellImage = CreateRenderer(root.transform, "Meteor Spell Image Preview", VFXSprites.FullMoonDescent, WithAlpha(Color.white, 0.34f), sortingOrder);
            ScaleSpriteToDiameter(spellImage, radius * 2.85f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.94f, 1.02f, 0f, true);
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

            SpriteRenderer spellImage = CreateRenderer(root.transform, "Meteor Full Moon Descent Image", VFXSprites.FullMoonDescent, WithAlpha(Color.white, 0.96f), sortingOrder);
            ScaleSpriteToDiameter(spellImage, radius * 3.05f);

            CombatVFXEffect effect = root.AddComponent<CombatVFXEffect>();
            effect.Play(duration, 0.8f, 1.08f, 0f, true);
        }

        public static GameObject PlayFrostDropWarning(Vector2 position, float radius, float duration = 0.9f, int sortingOrder = 760)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Frost Drop Warning");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.FrostZone);
            Color secondary = GetSecondaryColor(CombatVFXKind.FrostZone);
            SpriteRenderer shadow = CreateRenderer(root.transform, "Frost Landing Shadow", VFXSprites.ZoneFill, WithAlpha(main, 0.16f), sortingOrder);
            SpriteRenderer cracks = CreateRenderer(root.transform, "Frost Crack Preview", VFXSprites.GroundCracks, WithAlpha(secondary, 0.38f), sortingOrder + 1);
            SpriteRenderer shard = CreateRenderer(root.transform, "Falling Ice Preview", VFXSprites.IceShard, WithAlpha(secondary, 0.48f), sortingOrder + 2);

            shadow.transform.localScale = Vector3.one * radius * 1.9f;
            cracks.transform.localScale = Vector3.one * radius * 1.45f;
            shard.transform.localPosition = new Vector3(0f, radius * 1.65f, 0f);
            shard.transform.localRotation = Quaternion.Euler(0f, 0f, -10f);
            shard.transform.localScale = Vector3.one * radius * 0.78f;

            FrostDropWarningEffect effect = root.AddComponent<FrostDropWarningEffect>();
            effect.Play(duration, radius, shadow, cracks, shard);
            return root;
        }

        public static void PlayFrostDropImpact(Vector2 position, float radius, float duration = 0.35f, int sortingOrder = 1820)
        {
            radius *= SizeMultiplier;
            duration *= DurationMultiplier;
            sortingOrder += SortingOffset;

            GameObject root = new GameObject("VFX Frost Drop Impact");
            root.transform.position = position;

            Color main = GetMainColor(CombatVFXKind.Frost);
            Color secondary = GetSecondaryColor(CombatVFXKind.Frost);
            SpriteRenderer shock = CreateRenderer(root.transform, "Frost Impact Flash", VFXSprites.SoftDisc, WithAlpha(main, 0.34f), sortingOrder);
            SpriteRenderer cracks = CreateRenderer(root.transform, "Frost Impact Cracks", VFXSprites.GroundCracks, WithAlpha(secondary, 0.78f), sortingOrder + 1);
            SpriteRenderer burst = CreateRenderer(root.transform, "Frost Impact Shards", VFXSprites.MoonImpactShards, WithAlpha(secondary, 0.62f), sortingOrder + 2);
            SpriteRenderer shard = CreateRenderer(root.transform, "Falling Ice Shard", VFXSprites.IceShard, WithAlpha(secondary, 0.98f), sortingOrder + 3);
            SpriteRenderer trail = CreateRenderer(root.transform, "Falling Ice Trail", VFXSprites.LineCore, WithAlpha(Color.white, 0.62f), sortingOrder - 1);

            shock.transform.localScale = Vector3.one * radius * 1.75f;
            cracks.transform.localScale = Vector3.one * radius * 1.75f;
            burst.transform.localScale = Vector3.one * radius * 1.35f;
            shard.transform.localPosition = new Vector3(-radius * 0.18f, radius * 0.28f, 0f);
            shard.transform.localRotation = Quaternion.Euler(0f, 0f, -12f);
            shard.transform.localScale = Vector3.one * radius * 0.82f;
            trail.transform.localPosition = new Vector3(-radius * 0.18f, radius * 0.98f, 0f);
            trail.transform.localRotation = Quaternion.Euler(0f, 0f, -78f);
            trail.transform.localScale = new Vector3(radius * 1.85f, radius * 0.09f, 1f);

            FrostDropImpactEffect effect = root.AddComponent<FrostDropImpactEffect>();
            effect.Play(duration, radius, shock, cracks, burst, shard, trail);
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
            SpriteRenderer fill = CreateRenderer(root.transform, "Nebula Fill", VFXSprites.ZoneFill, WithAlpha(main, 0.026f), sortingOrder);
            SpriteRenderer detail = CreateRenderer(root.transform, "Nebula Pentagram", VFXSprites.Pentagram, WithAlpha(secondary, 0.13f), sortingOrder + 1);

            float diameter = radius * 2f;
            fill.transform.localScale = Vector3.one * diameter;
            detail.transform.localScale = Vector3.one * diameter * 0.94f;

            CombatVFXLoop loop = root.AddComponent<CombatVFXLoop>();
            loop.Configure(detail.transform, null, fill, 0.5f, 0.004f, 0.12f);
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

        private static void ScaleSpriteToDiameter(SpriteRenderer renderer, float diameter)
        {
            if (renderer == null || renderer.sprite == null)
                return;

            float spriteDiameter = Mathf.Max(renderer.sprite.bounds.size.x, renderer.sprite.bounds.size.y);

            if (spriteDiameter <= 0.001f)
                return;

            renderer.transform.localScale = Vector3.one * (diameter / spriteDiameter);
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

    public class WarlockCurseAuraEffect : MonoBehaviour
    {
        private SpriteRenderer[] renderers;
        private Vector3[] startPositions;
        private Vector3[] startScales;
        private Color[] startColors;
        private float duration;
        private float rotateSpeed;
        private float elapsed;

        public void Play(float effectDuration, float rotationSpeed)
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
            startPositions = new Vector3[renderers.Length];
            startScales = new Vector3[renderers.Length];
            startColors = new Color[renderers.Length];
            duration = Mathf.Max(0.05f, effectDuration);
            rotateSpeed = rotationSpeed;
            elapsed = 0f;

            for (int i = 0; i < renderers.Length; i++)
            {
                Transform rendererTransform = renderers[i].transform;
                startPositions[i] = rendererTransform.localPosition;
                startScales[i] = rendererTransform.localScale;
                startColors[i] = renderers[i].color;
            }
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            float pulse = 0.92f + Mathf.Sin(progress * Mathf.PI * 5f) * 0.08f;

            for (int i = 0; i < renderers.Length; i++)
            {
                bool skull = renderers[i].name.Contains("Skull");
                Transform rendererTransform = renderers[i].transform;
                float skullPulse = 0.96f + Mathf.Sin((progress * Mathf.PI * 4f) + i) * 0.12f;
                rendererTransform.localScale = startScales[i] * (skull ? skullPulse : pulse);

                if (skull)
                    rendererTransform.localPosition = startPositions[i] + Vector3.up * Mathf.Lerp(0.05f, 0.42f, progress);

                Color color = startColors[i];
                float fade = skull ? Mathf.Lerp(0.92f, 0.38f, progress) : Mathf.Lerp(1f, 0.18f, progress);
                color.a *= fade;
                renderers[i].color = color;
            }

            if (progress >= 1f)
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

    public class FrostDropWarningEffect : MonoBehaviour
    {
        private SpriteRenderer shadow;
        private SpriteRenderer cracks;
        private SpriteRenderer shard;
        private Color shadowColor;
        private Color cracksColor;
        private Color shardColor;
        private float duration;
        private float radius;
        private float elapsed;

        public void Play(float duration, float radius, SpriteRenderer shadow, SpriteRenderer cracks, SpriteRenderer shard)
        {
            this.duration = Mathf.Max(0.05f, duration);
            this.radius = Mathf.Max(0.05f, radius);
            this.shadow = shadow;
            this.cracks = cracks;
            this.shard = shard;
            shadowColor = shadow == null ? Color.clear : shadow.color;
            cracksColor = cracks == null ? Color.clear : cracks.color;
            shardColor = shard == null ? Color.clear : shard.color;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float pulse = 0.88f + Mathf.Sin(Time.time * 10f) * 0.06f;

            if (shadow != null)
            {
                shadow.transform.localScale = Vector3.one * radius * Mathf.Lerp(1.35f, 1.95f, progress) * pulse;
                shadow.color = SetAlpha(shadowColor, Mathf.Lerp(0.06f, shadowColor.a, progress));
            }

            if (cracks != null)
            {
                cracks.transform.localScale = Vector3.one * radius * Mathf.Lerp(0.8f, 1.45f, progress);
                cracks.color = SetAlpha(cracksColor, cracksColor.a * Mathf.Lerp(0.25f, 1f, progress));
            }

            if (shard != null)
            {
                shard.transform.localPosition = new Vector3(0f, Mathf.Lerp(radius * 2.3f, radius * 1.25f, progress), 0f);
                shard.color = SetAlpha(shardColor, shardColor.a * Mathf.Lerp(0.24f, 0.7f, progress));
            }

            if (progress >= 1f)
                Destroy(gameObject);
        }

        private static Color SetAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }
    }

    public class NinjaTeleportAfterimageEffect : MonoBehaviour
    {
        private SpriteRenderer body;
        private SpriteRenderer[] lines;
        private Color bodyColor;
        private Color[] lineColors;
        private Vector3[] linePositions;
        private Vector3[] lineScales;
        private float duration;
        private float elapsed;
        private bool appearing;

        public void Play(float duration, bool appearing, SpriteRenderer body, SpriteRenderer[] lines)
        {
            this.duration = Mathf.Max(0.06f, duration);
            this.appearing = appearing;
            this.body = body;
            this.lines = lines ?? System.Array.Empty<SpriteRenderer>();
            bodyColor = body == null ? Color.clear : body.color;
            lineColors = new Color[this.lines.Length];
            linePositions = new Vector3[this.lines.Length];
            lineScales = new Vector3[this.lines.Length];

            for (int i = 0; i < this.lines.Length; i++)
            {
                SpriteRenderer line = this.lines[i];

                if (line == null)
                    continue;

                lineColors[i] = line.color;
                linePositions[i] = line.transform.localPosition;
                lineScales[i] = line.transform.localScale;
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float visibility = appearing ? progress : 1f - progress;
            float smear = appearing ? 1f - progress : progress;

            if (body != null)
            {
                body.color = SetAlpha(bodyColor, bodyColor.a * visibility * 0.85f);
                body.transform.localScale = Vector3.one * Mathf.Lerp(1.06f, 0.98f, visibility);
            }

            for (int i = 0; i < lines.Length; i++)
            {
                SpriteRenderer line = lines[i];

                if (line == null)
                    continue;

                float direction = i % 2 == 0 ? 1f : -1f;
                float offset = Mathf.Sin((i + 1) * 1.73f) * 0.05f + direction * smear * 0.18f;
                line.transform.localPosition = linePositions[i] + new Vector3(offset, 0f, 0f);
                line.transform.localScale = new Vector3(lineScales[i].x * Mathf.Lerp(0.72f, 1.28f, smear), lineScales[i].y, lineScales[i].z);
                line.color = SetAlpha(lineColors[i], lineColors[i].a * Mathf.Clamp01(visibility + 0.16f));
            }

            if (progress >= 1f)
                Destroy(gameObject);
        }

        private static Color SetAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }
    }

    public class FrostDropImpactEffect : MonoBehaviour
    {
        private SpriteRenderer[] renderers;
        private Color[] colors;
        private SpriteRenderer shard;
        private SpriteRenderer trail;
        private float duration;
        private float radius;
        private float elapsed;

        public void Play(float duration, float radius, SpriteRenderer shock, SpriteRenderer cracks, SpriteRenderer burst, SpriteRenderer shard, SpriteRenderer trail)
        {
            this.duration = Mathf.Max(0.06f, duration);
            this.radius = Mathf.Max(0.05f, radius);
            this.shard = shard;
            this.trail = trail;
            renderers = new[] { shock, cracks, burst, shard, trail };
            colors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
                colors[i] = renderers[i] == null ? Color.clear : renderers[i].color;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float fade = 1f - Mathf.Pow(progress, 1.7f);

            if (shard != null)
            {
                float drop = Mathf.Clamp01(progress / 0.42f);
                shard.transform.localPosition = new Vector3(-radius * 0.18f, Mathf.Lerp(radius * 2.1f, radius * 0.08f, drop), 0f);
                shard.transform.localScale = Vector3.one * radius * Mathf.Lerp(1.05f, 0.76f, progress);
            }

            if (trail != null)
            {
                float drop = Mathf.Clamp01(progress / 0.42f);
                trail.transform.localPosition = new Vector3(-radius * 0.18f, Mathf.Lerp(radius * 2.9f, radius * 0.86f, drop), 0f);
                trail.transform.localScale = new Vector3(radius * Mathf.Lerp(2.45f, 0.9f, progress), radius * 0.09f, 1f);
            }

            transform.localScale = Vector3.one * Mathf.Lerp(0.82f, 1.12f, progress);

            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = renderers[i];

                if (spriteRenderer == null)
                    continue;

                spriteRenderer.color = SetAlpha(colors[i], colors[i].a * fade);
            }

            if (progress >= 1f)
                Destroy(gameObject);
        }

        private static Color SetAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
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
        private static Sprite fullMoonDescent;
        private static Sprite moonMagicCircle;
        private static Sprite moonImpactShards;
        private static Sprite iceShard;
        private static Sprite slashArc;
        private static Sprite skull;

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
        public static Sprite FullMoonDescent => fullMoonDescent ??= LoadResourceSprite("Effects/selene_fullmoon_descent", MoonMagicCircle);
        public static Sprite MoonMagicCircle => moonMagicCircle ??= CreateMoonMagicCircleSprite();
        public static Sprite MoonImpactShards => moonImpactShards ??= CreateMoonImpactShardsSprite();
        public static Sprite IceShard => iceShard ??= CreateIceShardSprite();
        public static Sprite SlashArc => slashArc ??= CreateSlashArcSprite();
        public static Sprite Skull => skull ??= CreateSkullSprite();

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

        private static Sprite LoadResourceSprite(string path, Sprite fallback)
        {
            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite != null)
                return sprite;

            Sprite[] sprites = Resources.LoadAll<Sprite>(path);

            if (sprites != null && sprites.Length > 0)
                return sprites[0];

            return fallback;
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

        private static Sprite CreateIceShardSprite()
        {
            Texture2D texture = CreateTexture(96, 128);
            Vector2 center = new Vector2(47.5f, 63.5f);
            Vector2 top = center + new Vector2(0f, 53f);
            Vector2 right = center + new Vector2(22f, 8f);
            Vector2 bottom = center + new Vector2(0f, -54f);
            Vector2 left = center + new Vector2(-22f, 8f);

            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 96; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float upper = Mathf.Clamp01(1f - Mathf.Abs(SignedDistanceToSegment(point, top, right)) / 12f)
                        + Mathf.Clamp01(1f - Mathf.Abs(SignedDistanceToSegment(point, top, left)) / 12f);
                    float lower = Mathf.Clamp01(1f - Mathf.Abs(SignedDistanceToSegment(point, bottom, right)) / 15f)
                        + Mathf.Clamp01(1f - Mathf.Abs(SignedDistanceToSegment(point, bottom, left)) / 15f);
                    float centerGlow = Mathf.Clamp01(1f - Mathf.Abs(point.x - center.x) / 12f) * Mathf.Clamp01(1f - Mathf.Abs(point.y - center.y) / 58f);
                    float alpha = Mathf.Clamp01((upper + lower) * 0.22f + centerGlow * 0.58f);

                    if (alpha > 0.02f)
                        texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            DrawLine(texture, top, right, 2);
            DrawLine(texture, right, bottom, 2);
            DrawLine(texture, bottom, left, 2);
            DrawLine(texture, left, top, 2);
            DrawLine(texture, top, bottom, 1);
            DrawLine(texture, left, right, 1);
            return ToSprite(texture);
        }

        private static Sprite CreateSlashArcSprite()
        {
            Texture2D texture = CreateTexture(160, 112);
            Vector2 center = new Vector2(26f, 55.5f);
            float innerRadius = 42f;
            float outerRadius = 78f;
            float minAngle = -38f;
            float maxAngle = 38f;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Vector2 offset = new Vector2(x, y) - center;
                    float radius = offset.magnitude;
                    float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

                    if (radius < innerRadius || radius > outerRadius || angle < minAngle || angle > maxAngle)
                        continue;

                    float radiusProgress = Mathf.InverseLerp(innerRadius, outerRadius, radius);
                    float angleProgress = Mathf.InverseLerp(minAngle, maxAngle, angle);
                    float edgeFade = Mathf.Sin(angleProgress * Mathf.PI);
                    float body = Mathf.Sin(radiusProgress * Mathf.PI);
                    float alpha = Mathf.Clamp01(body * edgeFade * 1.35f);

                    if (radiusProgress > 0.72f)
                        alpha = Mathf.Max(alpha, edgeFade * 0.72f);

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            for (int i = 0; i < 9; i++)
            {
                float angle = Mathf.Lerp(minAngle + 6f, maxAngle - 6f, i / 8f) * Mathf.Deg2Rad;
                DrawLine(texture, center + Direction(angle) * 52f, center + Direction(angle) * 82f, i % 3 == 0 ? 2 : 1);
            }

            return ToSprite(texture, 96f);
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

        private static Sprite CreateSkullSprite()
        {
            Texture2D texture = CreateTexture(72, 72);
            Vector2 center = new Vector2(35.5f, 38f);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float head = Mathf.Clamp01(1f - Vector2.Distance(point, center + Vector2.up * 7f) / 22f);
                    float jaw = Mathf.Clamp01(1f - Mathf.Abs(point.x - center.x) / 16f) * Mathf.Clamp01(1f - Mathf.Abs(point.y - 25f) / 12f);
                    float alpha = Mathf.Max(Mathf.Pow(head, 0.7f), jaw * 0.82f);

                    if (alpha <= 0.03f)
                        continue;

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            ClearDot(texture, center + new Vector2(-8f, 10f), 4);
            ClearDot(texture, center + new Vector2(8f, 10f), 4);
            ClearDot(texture, center + new Vector2(0f, 1f), 2);

            for (int x = 26; x <= 45; x += 6)
                DrawLine(texture, new Vector2(x, 18f), new Vector2(x, 25f), 1);

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

        private static float SignedDistanceToSegment(Vector2 point, Vector2 from, Vector2 to)
        {
            Vector2 segment = to - from;
            float segmentLength = segment.sqrMagnitude;

            if (segmentLength <= 0.001f)
                return Vector2.Distance(point, from);

            float t = Mathf.Clamp01(Vector2.Dot(point - from, segment) / segmentLength);
            return Vector2.Distance(point, from + segment * t);
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

        private static void ClearDot(Texture2D texture, Vector2 position, int radius)
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

                    texture.SetPixel(px, py, Color.clear);
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
