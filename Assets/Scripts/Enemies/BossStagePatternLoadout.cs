using UnityEngine;
using VampireLike.Audio;

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
                case 1:
                    ApplyBrute(boss);
                    break;
                case 2:
                    ApplyNecromancer(boss);
                    break;
                case 3:
                    ApplySpiderQueen(boss);
                    break;
                case 4:
                    ApplyFlameCaster(boss);
                    break;
                case 5:
                    ApplyFrostGolem(boss);
                    break;
                case 6:
                    ApplyShadowAssassin(boss);
                    break;
                case 7:
                    ApplyBurrowWarlock(boss);
                    break;
                case 9:
                    ApplyFlameDragon(boss);
                    break;
            }
        }

        private static void ApplyBrute(BossController boss)
        {
            GameObject gameObject = boss.gameObject;

            DashPattern dash = gameObject.GetComponent<DashPattern>();
            if (dash != null)
                dash.ConfigureDashSfx(SfxType.Boss1DashPrepare, SfxType.Boss1Dash, SfxType.Boss1DashImpact);

            ShockwavePattern shockwave = gameObject.GetComponent<ShockwavePattern>();
            if (shockwave != null)
                shockwave.ConfigureShockwaveSfx(SfxType.Boss1Shockwave);
        }

        private static void ApplyNecromancer(BossController boss)
        {
            GameObject gameObject = boss.gameObject;

            SummonPattern summon = gameObject.GetComponent<SummonPattern>();
            if (summon != null)
                summon.ConfigureSummonSfx(SfxType.Boss2SummonPrepare, SfxType.Boss2Summon);

            TargetAreaPattern targetArea = gameObject.GetComponent<TargetAreaPattern>();
            if (targetArea != null)
                targetArea.ConfigureTargetAreaSfx(SfxType.Boss2AreaWarning, SfxType.Boss2AreaExplosion);
        }

        private static void ApplySpiderQueen(BossController boss)
        {
            GameObject gameObject = boss.gameObject;

            AreaZonePattern areaZone = gameObject.GetComponent<AreaZonePattern>();
            if (areaZone != null)
                areaZone.ConfigureAreaZoneSfx(SfxType.Boss3WebWarning, SfxType.Boss3WebSpawn);

            DashPattern dash = gameObject.GetComponent<DashPattern>();
            if (dash != null)
                dash.ConfigureDashSfx(SfxType.Boss3Dash);

            SummonPattern summon = gameObject.GetComponent<SummonPattern>();
            if (summon != null)
                summon.ConfigureSummonSfx(SfxType.Boss3Summon);
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
            tripleFlame.ConfigureFlameSfx(SfxType.Boss4TripleFlame);

            RadialProjectilePattern radial = gameObject.GetComponent<RadialProjectilePattern>();
            if (radial != null)
            {
                radial.ConfigurePatternTiming(4.8f, 2.3f, 6, 1, 3, false);
                radial.ConfigureRadial(7, 1, 3.35f, 1, 5.5f, 0f, 0.38f);
                radial.ConfigureBarrageSfx(SfxType.Boss4RadialBarrage);
            }

            RotatingProjectilePattern rotating = gameObject.GetComponent<RotatingProjectilePattern>();
            if (rotating != null)
            {
                rotating.ConfigurePatternTiming(6.2f, 4.5f, 7, 2, 3, false);
                rotating.ConfigureBarrageSfx(SfxType.Boss4RotatingBarrage);
            }
        }

        private static void ApplyShadowAssassin(BossController boss)
        {
            GameObject gameObject = boss.gameObject;

            TeleportPattern teleport = gameObject.GetComponent<TeleportPattern>();
            if (teleport != null)
            {
                teleport.ConfigureShadowTeleportVisual(true);
                teleport.ConfigureTeleportSfx(SfxType.Boss6Teleport);
            }

            ConeAttackPattern coneAttack = gameObject.GetComponent<ConeAttackPattern>();
            if (coneAttack != null)
            {
                coneAttack.ConfigureShadowSlashVisual(true);
                coneAttack.ConfigureSlashSfx(SfxType.Boss6FrontSlash);
            }

            DashPattern dash = gameObject.GetComponent<DashPattern>();
            if (dash != null)
                dash.ConfigureDashSfx(SfxType.Boss6DashFirst, SfxType.Boss6DashSecond);
        }

        private static void ApplyFrostGolem(BossController boss)
        {
            GameObject gameObject = boss.gameObject;

            ShockwavePattern shockwave = gameObject.GetComponent<ShockwavePattern>();
            if (shockwave != null)
            {
                shockwave.SetAllowMovementDuringPattern(true);
                shockwave.ConfigureShockwaveSfx(SfxType.Boss5Shockwave);
            }

            TargetAreaPattern targetArea = gameObject.GetComponent<TargetAreaPattern>();
            if (targetArea != null)
            {
                targetArea.ConfigureTargetAreaSfx(SfxType.Boss5AreaWarning, SfxType.Boss5AreaExplosion);
                targetArea.ConfigureFrostDropVisual(true);
            }

            EnragePattern enrage = gameObject.GetComponent<EnragePattern>();
            if (enrage != null)
                enrage.ConfigureEnrageSfx(SfxType.Boss5Enrage);
        }

        private static void ApplyBurrowWarlock(BossController boss)
        {
            GameObject gameObject = boss.gameObject;
            BurrowPattern burrow = gameObject.GetComponent<BurrowPattern>();

            if (burrow == null)
                burrow = gameObject.AddComponent<BurrowPattern>();

            SummonPattern summon = gameObject.GetComponent<SummonPattern>();
            SetOnlyEnabled(gameObject, burrow, gameObject.GetComponent<TargetAreaPattern>(), gameObject.GetComponent<HomingOrbPattern>(), summon);

            BossDistanceMovement movement = gameObject.GetComponent<BossDistanceMovement>();
            if (movement != null)
                movement.ConfigureMovement(1.42f, 2.25f, 3.7f, 0.32f);

            burrow.ConfigurePatternTiming(4.05f, 1.1f, 10, 1, 3, false);
            burrow.ConfigureBurrow(0.32f, 0.68f, 0.12f, 1, 1, 0.24f, 0.82f, 1.05f);
            burrow.ConfigureWarlockTeleportVisual(true);
            burrow.ConfigureTeleportSfx(SfxType.Boss7Teleport);

            TargetAreaPattern targetArea = gameObject.GetComponent<TargetAreaPattern>();
            if (targetArea != null)
            {
                targetArea.ConfigurePatternTiming(5.4f, 2.1f, 7, 1, 3, false);
                targetArea.ConfigureTargetArea(0.82f, 0.82f, 1, 1, 0.16f, 0.78f, 2, 0.32f, new Color(0.58f, 0.28f, 0.95f, 0.34f));
                targetArea.ConfigureTargetAreaSfx(SfxType.Boss7CurseWarning, SfxType.Boss7CurseExplosion);
                targetArea.ConfigureWarlockCurseVisual(true);
            }

            HomingOrbPattern homingOrb = gameObject.GetComponent<HomingOrbPattern>();
            if (homingOrb != null)
            {
                homingOrb.ConfigurePatternTiming(6.2f, 3.3f, 4, 2, 3, false);
                homingOrb.ConfigureHomingOrb(1, 1, 0.22f, 2.45f, 1, 4.8f, 1.9f, 105f, 0.32f);
                homingOrb.ConfigureHomingOrbSfx(SfxType.Boss7HomingOrb);
            }

            if (summon != null)
            {
                summon.ConfigurePatternTiming(7.1f, 2.8f, 5, 2, 3, true);
                summon.ConfigureSummon(10, 10, 0, 10, 1.9f, 0.06f, 0.22f);
                summon.ConfigureSummonSfx(SfxType.Boss7Summon);
                summon.ConfigureWarlockSummonVisual(true);
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
