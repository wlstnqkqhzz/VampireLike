using UnityEngine;

namespace VampireLike.Enemies
{
    public static class BossStagePatternLoadout
    {
        public static void Apply(BossController boss)
        {
            if (boss == null)
                return;

            switch (boss.BossStage)
            {
                case 4:
                    ApplyFlameCaster(boss);
                    break;
                case 7:
                    ApplyBurrowWarlock(boss);
                    break;
                case 9:
                    ApplyFlameDragon(boss);
                    break;
            }
        }

        private static void ApplyFlameCaster(BossController boss)
        {
            GameObject gameObject = boss.gameObject;
            TripleFlameBurstPattern tripleFlame = gameObject.GetComponent<TripleFlameBurstPattern>();

            if (tripleFlame == null)
                tripleFlame = gameObject.AddComponent<TripleFlameBurstPattern>();

            SetOnlyEnabled(gameObject, gameObject.GetComponent<RadialProjectilePattern>(), gameObject.GetComponent<RotatingProjectilePattern>(), tripleFlame);

            tripleFlame.ConfigurePatternTiming(4.35f, 1.15f, 8, 1, 3, false);
            tripleFlame.ConfigureTripleFlame(0.4f, 3, 0.22f, 2.75f, 34f, 2, 0.16f, 0.2f);

            RadialProjectilePattern radial = gameObject.GetComponent<RadialProjectilePattern>();
            if (radial != null)
            {
                radial.ConfigurePatternTiming(4.8f, 2.3f, 6, 1, 3, false);
                radial.ConfigureRadial(7, 1, 3.35f, 1, 5.5f, 0f, 0.38f);
            }

            RotatingProjectilePattern rotating = gameObject.GetComponent<RotatingProjectilePattern>();
            if (rotating != null)
                rotating.ConfigurePatternTiming(6.2f, 4.5f, 7, 2, 3, false);
        }

        private static void ApplyBurrowWarlock(BossController boss)
        {
            GameObject gameObject = boss.gameObject;
            BurrowPattern burrow = gameObject.GetComponent<BurrowPattern>();

            if (burrow == null)
                burrow = gameObject.AddComponent<BurrowPattern>();

            SetOnlyEnabled(gameObject, burrow, gameObject.GetComponent<TargetAreaPattern>(), gameObject.GetComponent<HomingOrbPattern>());

            BossDistanceMovement movement = gameObject.GetComponent<BossDistanceMovement>();
            if (movement != null)
                movement.ConfigureMovement(1.42f, 2.25f, 3.7f, 0.32f);

            burrow.ConfigurePatternTiming(4.05f, 1.1f, 10, 1, 3, false);
            burrow.ConfigureBurrow(0.32f, 0.68f, 0.12f, 1, 1, 0.24f, 0.82f, 1.05f);

            TargetAreaPattern targetArea = gameObject.GetComponent<TargetAreaPattern>();
            if (targetArea != null)
            {
                targetArea.ConfigurePatternTiming(5.4f, 2.1f, 7, 1, 3, false);
                targetArea.ConfigureTargetArea(0.82f, 0.82f, 1, 1, 0.16f, 0.78f, 2, 0.32f, new Color(0.58f, 0.28f, 0.95f, 0.34f));
            }

            HomingOrbPattern homingOrb = gameObject.GetComponent<HomingOrbPattern>();
            if (homingOrb != null)
            {
                homingOrb.ConfigurePatternTiming(6.2f, 3.3f, 4, 2, 3, false);
                homingOrb.ConfigureHomingOrb(1, 1, 0.22f, 2.45f, 1, 4.8f, 1.9f, 105f, 0.32f);
            }
        }

        private static void ApplyFlameDragon(BossController boss)
        {
            GameObject gameObject = boss.gameObject;
            SetOnlyEnabled(
                gameObject,
                gameObject.GetComponent<BreathPattern>(),
                gameObject.GetComponent<TripleFlameBurstPattern>(),
                gameObject.GetComponent<RadialProjectilePattern>(),
                gameObject.GetComponent<AreaZonePattern>());

            BreathPattern breath = gameObject.GetComponent<BreathPattern>();
            if (breath != null)
            {
                breath.ConfigurePatternTiming(4.65f, 1.1f, 10, 1, 3, false);
                breath.ConfigureBreath(0.72f, 1.22f, 3.55f, 42f, 2, 0.34f, true, 1.25f, true, 78f);
            }

            TripleFlameBurstPattern tripleFlame = gameObject.GetComponent<TripleFlameBurstPattern>();
            if (tripleFlame != null)
            {
                tripleFlame.ConfigurePatternTiming(3.8f, 2.2f, 9, 1, 3, false);
                tripleFlame.ConfigureTripleFlame(0.38f, 3, 0.2f, 3.45f, 34f, 2, 0.15f, 0.21f);
            }

            AreaZonePattern areaZone = gameObject.GetComponent<AreaZonePattern>();
            if (areaZone != null)
            {
                areaZone.ConfigurePatternTiming(6.6f, 3.6f, 6, 2, 3, true);
                areaZone.ConfigureAreaZone(0.72f, 3.2f, 1.8f, 2, 0, 4, 1, 0.9f, 1, 0.75f, 0.82f, new Color(1f, 0.36f, 0.1f, 0.36f));
            }

            RadialProjectilePattern radial = gameObject.GetComponent<RadialProjectilePattern>();
            if (radial != null)
            {
                radial.ConfigurePatternTiming(5.8f, 4.1f, 5, 3, 3, false);
                radial.ConfigureRadial(12, 0, 3.15f, 1, 5f, 15f, 0.42f);
            }
        }

        private static void SetOnlyEnabled(GameObject gameObject, params BossPattern[] enabledPatterns)
        {
            BossPattern[] patterns = gameObject.GetComponents<BossPattern>();

            foreach (BossPattern pattern in patterns)
            {
                if (pattern == null)
                    continue;

                pattern.enabled = Contains(enabledPatterns, pattern);
            }
        }

        private static bool Contains(BossPattern[] patterns, BossPattern target)
        {
            foreach (BossPattern pattern in patterns)
            {
                if (pattern == target)
                    return true;
            }

            return false;
        }
    }
}
