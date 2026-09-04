using System.Collections.Generic;
using UnityEngine;
using VampireLike.Settings;

namespace VampireLike.Audio
{
    public enum SfxType
    {
        PlayerShoot,
        EnemyHit,
        EnemyDeath,
        PlayerHit,
        ExperiencePickup,
        LevelUp,
        UpgradeSelect,
        BossAppear,
        GameOver,
        Heal,
        ShieldBlock,
        KaelSwordWave,
        SeleneDaggerThrow,
        ShieldReady,
        ShieldBreak,
        SkillExplosion,
        SkillRicochet,
        SkillScatter,
        SkillOrbitBlade,
        SkillShockwave,
        SkillFrost,
        SkillVampirism,
        SkillMeteorImpact,
        BossDash,
        BossZone,
        BossProjectile,
        BossTeleport,
        BossDeath,
        KaelAttack,
        KaelHit1,
        KaelHit2,
        KaelDeath,
        SeleneAttack,
        SeleneHit1,
        SeleneHit2,
        SeleneDeath,
        HanSeorinAttack,
        HanSeorinHit1,
        HanSeorinHit2,
        HanSeorinHit3,
        HanSeorinDeath,
        SmallHealthPackPickup,
        LargeHealthPackPickup,
        KaelBlackWave,
        KaelGuardiansResolve,
        KaelMagicSlash,
        KaelBlackIronBarrierBlock,
        KaelBlackIronBarrierOn,
        KaelExecutionBlade,
        KaelBlackIronRegen,
        SeleneMoonmarkApply,
        SeleneMoonmarkBurst,
        SeleneStarlightChain,
        SeleneNebulaSpawn,
        SeleneSilvermoonWave,
        SeleneEclipseResonance,
        SeleneFullmoonWarning,
        SeleneFullmoonFall,
        SeleneFullmoonImpact,
        SeleneFullmoonExplosion,
        HanSeorinBloodMarkApply,
        HanSeorinBloodMarkBurst,
        HanSeorinShadowDagger,
        HanSeorinReturningBlade,
        HanSeorinKillingIntentMax,
        HanSeorinRedExecution,
        HanSeorinBloodFangApply,
        Boss1DashPrepare,
        Boss1Dash,
        Boss1DashImpact,
        Boss1Shockwave,
        Boss2SummonPrepare,
        Boss2Summon,
        Boss2AreaWarning,
        Boss2AreaExplosion,
        Boss3WebWarning,
        Boss3WebSpawn,
        Boss3Dash,
        Boss3Summon,
        Boss4RadialBarrage,
        Boss4TripleFlame,
        Boss4RotatingBarrage,
        Boss5Shockwave,
        Boss5AreaWarning,
        Boss5AreaExplosion,
        Boss5Enrage,
        Boss6Teleport,
        Boss6FrontSlash,
        Boss6DashFirst,
        Boss6DashSecond
    }

    /// <summary>
    /// 게임 전반에서 사용하는 짧은 효과음을 한 곳에서 재생합니다.
    /// 같은 효과음이 너무 빽빽하게 겹치지 않도록 최소 재생 간격도 함께 관리합니다.
    /// </summary>
    public class GameSfx : MonoBehaviour
    {
        private const string ResourceRoot = "Sounds/";
        private static readonly Dictionary<SfxType, string> ClipNames = new Dictionary<SfxType, string>
        {
            { SfxType.PlayerShoot, "player_shoot" },
            { SfxType.EnemyHit, "enemy_hit" },
            { SfxType.EnemyDeath, "enemy_death" },
            { SfxType.PlayerHit, "player_hit" },
            { SfxType.ExperiencePickup, "experience_pickup" },
            { SfxType.LevelUp, "level_up" },
            { SfxType.UpgradeSelect, "upgrade_select" },
            { SfxType.BossAppear, "boss_appear" },
            { SfxType.GameOver, "game_over" },
            { SfxType.Heal, "heal" },
            { SfxType.ShieldBlock, "shield_block_sfx" },
            { SfxType.KaelSwordWave, "kael_sword_wave" },
            { SfxType.SeleneDaggerThrow, "selene_dagger_throw" },
            { SfxType.ShieldReady, "shield_ready_sfx" },
            { SfxType.ShieldBreak, "shield_break_sfx" },
            { SfxType.KaelBlackWave, "kael_black_wave" },
            { SfxType.KaelGuardiansResolve, "kael_guardians_resolve" },
            { SfxType.KaelMagicSlash, "kael_magic_slash" },
            { SfxType.KaelBlackIronBarrierBlock, "kael_blackiron_barrier_block" },
            { SfxType.KaelBlackIronBarrierOn, "kael_blackiron_barrier_on" },
            { SfxType.KaelExecutionBlade, "kael_execution_blade" },
            { SfxType.KaelBlackIronRegen, "kael_blackiron_regen" },
            { SfxType.SeleneMoonmarkApply, "selene_moonmark_apply" },
            { SfxType.SeleneMoonmarkBurst, "selene_moonmark_burst" },
            { SfxType.SeleneStarlightChain, "selene_starlight_chain" },
            { SfxType.SeleneNebulaSpawn, "selene_nebula_spawn" },
            { SfxType.SeleneSilvermoonWave, "selene_silvermoon_wave" },
            { SfxType.SeleneEclipseResonance, "selene_eclipse_resonance" },
            { SfxType.SeleneFullmoonWarning, "selene_fullmoon_warning" },
            { SfxType.SeleneFullmoonFall, "selene_fullmoon_fall" },
            { SfxType.SeleneFullmoonImpact, "selene_fullmoon_impact" },
            { SfxType.SeleneFullmoonExplosion, "selene_fullmoon_explosion" },
            { SfxType.HanSeorinBloodMarkApply, "hanseorin_blood_mark_apply" },
            { SfxType.HanSeorinBloodMarkBurst, "hanseorin_blood_mark_burst" },
            { SfxType.HanSeorinShadowDagger, "hanseorin_shadow_dagger" },
            { SfxType.HanSeorinReturningBlade, "hanseorin_returning_blade" },
            { SfxType.HanSeorinKillingIntentMax, "hanseorin_killing_intent_max" },
            { SfxType.HanSeorinRedExecution, "hanseorin_red_execution" },
            { SfxType.HanSeorinBloodFangApply, "hanseorin_bloodfang_apply" },
            { SfxType.Boss1DashPrepare, "boss1_dash_prepare" },
            { SfxType.Boss1Dash, "boss1_dash" },
            { SfxType.Boss1DashImpact, "boss1_dash_impact" },
            { SfxType.Boss1Shockwave, "boss1_shockwave" },
            { SfxType.Boss2SummonPrepare, "boss2_summon_prepare" },
            { SfxType.Boss2Summon, "boss2_summon" },
            { SfxType.Boss2AreaWarning, "boss2_area_warning" },
            { SfxType.Boss2AreaExplosion, "boss2_area_explosion" },
            { SfxType.Boss3WebWarning, "boss3_web_warning" },
            { SfxType.Boss3WebSpawn, "boss3_web_spawn" },
            { SfxType.Boss3Dash, "boss3_dash" },
            { SfxType.Boss3Summon, "boss3_summon" },
            { SfxType.Boss4RadialBarrage, "boss4_radial_barrage" },
            { SfxType.Boss4TripleFlame, "boss4_triple_flame" },
            { SfxType.Boss4RotatingBarrage, "boss4_rotating_barrage" },
            { SfxType.Boss5Shockwave, "boss5_shockwave" },
            { SfxType.Boss5AreaWarning, "boss5_area_warning" },
            { SfxType.Boss5AreaExplosion, "boss5_area_explosion" },
            { SfxType.Boss5Enrage, "boss5_enrage" },
            { SfxType.Boss6Teleport, "boss6_teleport" },
            { SfxType.Boss6FrontSlash, "boss6_front_slash" },
            { SfxType.Boss6DashFirst, "boss6_dash_01" },
            { SfxType.Boss6DashSecond, "boss6_dash_02" },
            { SfxType.SkillExplosion, "skill_explosion" },
            { SfxType.SkillRicochet, "skill_ricochet" },
            { SfxType.SkillScatter, "skill_scatter" },
            { SfxType.SkillOrbitBlade, "skill_orbit_blade" },
            { SfxType.SkillShockwave, "skill_shockwave" },
            { SfxType.SkillFrost, "skill_frost" },
            { SfxType.SkillVampirism, "skill_vampirism" },
            { SfxType.SkillMeteorImpact, "skill_meteor_impact" },
            { SfxType.BossDash, "boss_dash" },
            { SfxType.BossZone, "boss_zone" },
            { SfxType.BossProjectile, "boss_projectile" },
            { SfxType.BossTeleport, "boss_teleport" },
            { SfxType.BossDeath, "boss_death" },
            { SfxType.KaelAttack, "kael_attack" },
            { SfxType.KaelHit1, "kael_hit_1" },
            { SfxType.KaelHit2, "kael_hit_2" },
            { SfxType.KaelDeath, "kael_death" },
            { SfxType.SeleneAttack, "selene_attack" },
            { SfxType.SeleneHit1, "selene_hit_1" },
            { SfxType.SeleneHit2, "selene_hit_2" },
            { SfxType.SeleneDeath, "selene_death" },
            { SfxType.HanSeorinAttack, "hanseorin_attack" },
            { SfxType.HanSeorinHit1, "hanseorin_hit_1" },
            { SfxType.HanSeorinHit2, "hanseorin_hit_2" },
            { SfxType.HanSeorinHit3, "hanseorin_hit_3" },
            { SfxType.HanSeorinDeath, "hanseorin_death" },
            { SfxType.SmallHealthPackPickup, "health_pack_small_pickup" },
            { SfxType.LargeHealthPackPickup, "health_pack_large_pickup" }
        };

        private static readonly Dictionary<SfxType, float> MinIntervals = new Dictionary<SfxType, float>
        {
            { SfxType.PlayerShoot, 0.06f },
            { SfxType.EnemyHit, 0.035f },
            { SfxType.EnemyDeath, 0.05f },
            { SfxType.PlayerHit, 0.12f },
            { SfxType.ExperiencePickup, 0.025f },
            { SfxType.LevelUp, 0.25f },
            { SfxType.UpgradeSelect, 0.08f },
            { SfxType.BossAppear, 0.5f },
            { SfxType.GameOver, 0.5f },
            { SfxType.Heal, 0.1f },
            { SfxType.ShieldBlock, 0.1f },
            { SfxType.KaelSwordWave, 0.05f },
            { SfxType.SeleneDaggerThrow, 0.05f },
            { SfxType.ShieldReady, 0.35f },
            { SfxType.ShieldBreak, 0.18f },
            { SfxType.KaelBlackWave, 0.15f },
            { SfxType.KaelGuardiansResolve, 0.2f },
            { SfxType.KaelMagicSlash, 0.15f },
            { SfxType.KaelBlackIronBarrierBlock, 0.2f },
            { SfxType.KaelBlackIronBarrierOn, 0f },
            { SfxType.KaelExecutionBlade, 0.12f },
            { SfxType.KaelBlackIronRegen, 0f },
            { SfxType.SeleneMoonmarkApply, 0.1f },
            { SfxType.SeleneMoonmarkBurst, 0.15f },
            { SfxType.SeleneStarlightChain, 0.1f },
            { SfxType.SeleneNebulaSpawn, 0.15f },
            { SfxType.SeleneSilvermoonWave, 0.15f },
            { SfxType.SeleneEclipseResonance, 0.2f },
            { SfxType.SeleneFullmoonWarning, 0f },
            { SfxType.SeleneFullmoonFall, 0f },
            { SfxType.SeleneFullmoonImpact, 0f },
            { SfxType.SeleneFullmoonExplosion, 0f },
            { SfxType.HanSeorinBloodMarkApply, 0.07f },
            { SfxType.HanSeorinBloodMarkBurst, 0.15f },
            { SfxType.HanSeorinShadowDagger, 0.08f },
            { SfxType.HanSeorinReturningBlade, 0.1f },
            { SfxType.HanSeorinKillingIntentMax, 0.2f },
            { SfxType.HanSeorinRedExecution, 0.12f },
            { SfxType.HanSeorinBloodFangApply, 0.15f },
            { SfxType.Boss1DashPrepare, 0f },
            { SfxType.Boss1Dash, 0.15f },
            { SfxType.Boss1DashImpact, 0.15f },
            { SfxType.Boss1Shockwave, 0.2f },
            { SfxType.Boss2SummonPrepare, 0f },
            { SfxType.Boss2Summon, 0.15f },
            { SfxType.Boss2AreaWarning, 0.2f },
            { SfxType.Boss2AreaExplosion, 0.2f },
            { SfxType.Boss3WebWarning, 0.2f },
            { SfxType.Boss3WebSpawn, 0.15f },
            { SfxType.Boss3Dash, 0.12f },
            { SfxType.Boss3Summon, 0.2f },
            { SfxType.Boss4RadialBarrage, 0.15f },
            { SfxType.Boss4TripleFlame, 0f },
            { SfxType.Boss4RotatingBarrage, 0.12f },
            { SfxType.Boss5Shockwave, 0.2f },
            { SfxType.Boss5AreaWarning, 0.2f },
            { SfxType.Boss5AreaExplosion, 0.2f },
            { SfxType.Boss5Enrage, 0f },
            { SfxType.Boss6Teleport, 0.12f },
            { SfxType.Boss6FrontSlash, 0.1f },
            { SfxType.Boss6DashFirst, 0.1f },
            { SfxType.Boss6DashSecond, 0.1f },
            { SfxType.SkillExplosion, 0.12f },
            { SfxType.SkillRicochet, 0.08f },
            { SfxType.SkillScatter, 0.08f },
            { SfxType.SkillOrbitBlade, 0.045f },
            { SfxType.SkillShockwave, 0.16f },
            { SfxType.SkillFrost, 0.12f },
            { SfxType.SkillVampirism, 0.18f },
            { SfxType.SkillMeteorImpact, 0.18f },
            { SfxType.BossDash, 0.18f },
            { SfxType.BossZone, 0.2f },
            { SfxType.BossProjectile, 0.12f },
            { SfxType.BossTeleport, 0.2f },
            { SfxType.BossDeath, 0.5f },
            { SfxType.KaelAttack, 0f },
            { SfxType.KaelHit1, 0.35f },
            { SfxType.KaelHit2, 0.35f },
            { SfxType.KaelDeath, 0f },
            { SfxType.SeleneAttack, 0f },
            { SfxType.SeleneHit1, 0.35f },
            { SfxType.SeleneHit2, 0.35f },
            { SfxType.SeleneDeath, 0f },
            { SfxType.HanSeorinAttack, 0.04f },
            { SfxType.HanSeorinHit1, 0.3f },
            { SfxType.HanSeorinHit2, 0.3f },
            { SfxType.HanSeorinHit3, 0.3f },
            { SfxType.HanSeorinDeath, 0f },
            { SfxType.SmallHealthPackPickup, 0.08f },
            { SfxType.LargeHealthPackPickup, 0.12f }
        };

        private static readonly Dictionary<SfxType, float> Volumes = new Dictionary<SfxType, float>
        {
            { SfxType.PlayerShoot, 0.45f },
            { SfxType.EnemyHit, 0.34f },
            { SfxType.EnemyDeath, 0.62f },
            { SfxType.PlayerHit, 0.55f },
            { SfxType.ExperiencePickup, 0.32f },
            { SfxType.LevelUp, 0.62f },
            { SfxType.UpgradeSelect, 0.62f },
            { SfxType.BossAppear, 0.72f },
            { SfxType.GameOver, 0.72f },
            { SfxType.Heal, 0.48f },
            { SfxType.ShieldBlock, 0.58f },
            { SfxType.KaelSwordWave, 0.85f },
            { SfxType.SeleneDaggerThrow, 0.7f },
            { SfxType.ShieldReady, 0.48f },
            { SfxType.ShieldBreak, 0.52f },
            { SfxType.KaelBlackWave, 0.75f },
            { SfxType.KaelGuardiansResolve, 0.95f },
            { SfxType.KaelMagicSlash, 1.05f },
            { SfxType.KaelBlackIronBarrierBlock, 0.95f },
            { SfxType.KaelBlackIronBarrierOn, 0.7f },
            { SfxType.KaelExecutionBlade, 1.05f },
            { SfxType.KaelBlackIronRegen, 0.4f },
            { SfxType.SeleneMoonmarkApply, 0.45f },
            { SfxType.SeleneMoonmarkBurst, 0.95f },
            { SfxType.SeleneStarlightChain, 0.5f },
            { SfxType.SeleneNebulaSpawn, 0.6f },
            { SfxType.SeleneSilvermoonWave, 0.75f },
            { SfxType.SeleneEclipseResonance, 0.65f },
            { SfxType.SeleneFullmoonWarning, 0.7f },
            { SfxType.SeleneFullmoonFall, 0.85f },
            { SfxType.SeleneFullmoonImpact, 1.05f },
            { SfxType.SeleneFullmoonExplosion, 0.95f },
            { SfxType.HanSeorinBloodMarkApply, 0.72f },
            { SfxType.HanSeorinBloodMarkBurst, 1f },
            { SfxType.HanSeorinShadowDagger, 0.68f },
            { SfxType.HanSeorinReturningBlade, 0.78f },
            { SfxType.HanSeorinKillingIntentMax, 0.58f },
            { SfxType.HanSeorinRedExecution, 1.03f },
            { SfxType.HanSeorinBloodFangApply, 0.55f },
            { SfxType.Boss1DashPrepare, 0.8f },
            { SfxType.Boss1Dash, 0.9f },
            { SfxType.Boss1DashImpact, 1f },
            { SfxType.Boss1Shockwave, 0.95f },
            { SfxType.Boss2SummonPrepare, 0.75f },
            { SfxType.Boss2Summon, 0.8f },
            { SfxType.Boss2AreaWarning, 0.7f },
            { SfxType.Boss2AreaExplosion, 1f },
            { SfxType.Boss3WebWarning, 0.65f },
            { SfxType.Boss3WebSpawn, 0.8f },
            { SfxType.Boss3Dash, 0.85f },
            { SfxType.Boss3Summon, 0.75f },
            { SfxType.Boss4RadialBarrage, 0.8f },
            { SfxType.Boss4TripleFlame, 0.8f },
            { SfxType.Boss4RotatingBarrage, 0.75f },
            { SfxType.Boss5Shockwave, 0.95f },
            { SfxType.Boss5AreaWarning, 0.65f },
            { SfxType.Boss5AreaExplosion, 1f },
            { SfxType.Boss5Enrage, 0.85f },
            { SfxType.Boss6Teleport, 1.08f },
            { SfxType.Boss6FrontSlash, 1.18f },
            { SfxType.Boss6DashFirst, 1.10f },
            { SfxType.Boss6DashSecond, 1.20f },
            { SfxType.SkillExplosion, 0.5f },
            { SfxType.SkillRicochet, 0.38f },
            { SfxType.SkillScatter, 0.42f },
            { SfxType.SkillOrbitBlade, 0.58f },
            { SfxType.SkillShockwave, 0.5f },
            { SfxType.SkillFrost, 0.4f },
            { SfxType.SkillVampirism, 0.44f },
            { SfxType.SkillMeteorImpact, 0.58f },
            { SfxType.BossDash, 0.52f },
            { SfxType.BossZone, 0.5f },
            { SfxType.BossProjectile, 0.42f },
            { SfxType.BossTeleport, 0.48f },
            { SfxType.BossDeath, 0.68f },
            { SfxType.KaelAttack, 1f },
            { SfxType.KaelHit1, 0.88f },
            { SfxType.KaelHit2, 0.88f },
            { SfxType.KaelDeath, 1f },
            { SfxType.SeleneAttack, 1f },
            { SfxType.SeleneHit1, 0.83f },
            { SfxType.SeleneHit2, 0.83f },
            { SfxType.SeleneDeath, 1f },
            { SfxType.HanSeorinAttack, 0.62f },
            { SfxType.HanSeorinHit1, 0.83f },
            { SfxType.HanSeorinHit2, 0.83f },
            { SfxType.HanSeorinHit3, 0.83f },
            { SfxType.HanSeorinDeath, 1f },
            { SfxType.SmallHealthPackPickup, 0.5f },
            { SfxType.LargeHealthPackPickup, 0.62f }
        };

        private static readonly Dictionary<SfxType, Vector2> PitchRanges = new Dictionary<SfxType, Vector2>
        {
            { SfxType.KaelAttack, new Vector2(0.97f, 1.03f) },
            { SfxType.KaelSwordWave, new Vector2(0.97f, 1.03f) },
            { SfxType.KaelBlackWave, new Vector2(0.97f, 1.03f) },
            { SfxType.KaelGuardiansResolve, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelMagicSlash, new Vector2(0.99f, 1.01f) },
            { SfxType.KaelBlackIronBarrierOn, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelBlackIronBarrierBlock, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelExecutionBlade, new Vector2(0.99f, 1.01f) },
            { SfxType.KaelBlackIronRegen, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelHit1, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelHit2, new Vector2(0.98f, 1.02f) },
            { SfxType.KaelDeath, new Vector2(1f, 1f) },
            { SfxType.SeleneAttack, new Vector2(0.97f, 1.03f) },
            { SfxType.SeleneDaggerThrow, new Vector2(0.97f, 1.04f) },
            { SfxType.SeleneStarlightChain, new Vector2(0.96f, 1.06f) },
            { SfxType.SeleneNebulaSpawn, new Vector2(0.98f, 1.03f) },
            { SfxType.SeleneSilvermoonWave, new Vector2(0.97f, 1.03f) },
            { SfxType.SeleneMoonmarkApply, new Vector2(0.97f, 1.05f) },
            { SfxType.SeleneMoonmarkBurst, new Vector2(0.99f, 1.02f) },
            { SfxType.SeleneEclipseResonance, new Vector2(0.98f, 1.03f) },
            { SfxType.SeleneFullmoonWarning, new Vector2(1f, 1f) },
            { SfxType.SeleneFullmoonFall, new Vector2(0.98f, 1.02f) },
            { SfxType.SeleneFullmoonImpact, new Vector2(0.99f, 1.01f) },
            { SfxType.SeleneFullmoonExplosion, new Vector2(0.99f, 1.01f) },
            { SfxType.SeleneHit1, new Vector2(0.98f, 1.02f) },
            { SfxType.SeleneHit2, new Vector2(0.98f, 1.02f) },
            { SfxType.SeleneDeath, new Vector2(1f, 1f) },
            { SfxType.HanSeorinAttack, new Vector2(0.96f, 1.04f) },
            { SfxType.HanSeorinBloodMarkApply, new Vector2(0.96f, 1.04f) },
            { SfxType.HanSeorinBloodMarkBurst, new Vector2(0.98f, 1.02f) },
            { SfxType.HanSeorinShadowDagger, new Vector2(0.96f, 1.04f) },
            { SfxType.HanSeorinReturningBlade, new Vector2(0.96f, 1.04f) },
            { SfxType.HanSeorinKillingIntentMax, new Vector2(0.99f, 1.01f) },
            { SfxType.HanSeorinRedExecution, new Vector2(0.99f, 1.01f) },
            { SfxType.HanSeorinBloodFangApply, new Vector2(0.97f, 1.03f) },
            { SfxType.Boss1DashPrepare, new Vector2(0.94f, 0.98f) },
            { SfxType.Boss1Dash, new Vector2(0.91f, 0.96f) },
            { SfxType.Boss1DashImpact, new Vector2(0.94f, 0.98f) },
            { SfxType.Boss1Shockwave, new Vector2(0.93f, 0.98f) },
            { SfxType.Boss2SummonPrepare, new Vector2(0.96f, 1f) },
            { SfxType.Boss2Summon, new Vector2(0.96f, 1.03f) },
            { SfxType.Boss2AreaWarning, new Vector2(0.97f, 1.01f) },
            { SfxType.Boss2AreaExplosion, new Vector2(0.96f, 1.01f) },
            { SfxType.Boss3WebWarning, new Vector2(1.02f, 1.08f) },
            { SfxType.Boss3WebSpawn, new Vector2(1f, 1.05f) },
            { SfxType.Boss3Dash, new Vector2(1.03f, 1.08f) },
            { SfxType.Boss3Summon, new Vector2(0.98f, 1.04f) },
            { SfxType.Boss4RadialBarrage, new Vector2(0.98f, 1.03f) },
            { SfxType.Boss4TripleFlame, new Vector2(0.98f, 1.04f) },
            { SfxType.Boss4RotatingBarrage, new Vector2(0.97f, 1.03f) },
            { SfxType.Boss5Shockwave, new Vector2(0.93f, 0.98f) },
            { SfxType.Boss5AreaWarning, new Vector2(0.98f, 1.03f) },
            { SfxType.Boss5AreaExplosion, new Vector2(0.96f, 1.01f) },
            { SfxType.Boss5Enrage, new Vector2(0.94f, 0.99f) },
            { SfxType.Boss6Teleport, new Vector2(0.98f, 1.05f) },
            { SfxType.Boss6FrontSlash, new Vector2(1f, 1.05f) },
            { SfxType.Boss6DashFirst, new Vector2(1.02f, 1.06f) },
            { SfxType.Boss6DashSecond, new Vector2(1.04f, 1.08f) },
            { SfxType.HanSeorinHit1, new Vector2(0.98f, 1.02f) },
            { SfxType.HanSeorinHit2, new Vector2(0.98f, 1.02f) },
            { SfxType.HanSeorinHit3, new Vector2(0.98f, 1.02f) },
            { SfxType.HanSeorinDeath, new Vector2(1f, 1f) }
        };

        private static readonly Dictionary<SfxType, AudioClip> LoadedClips = new Dictionary<SfxType, AudioClip>();
        private static readonly Dictionary<SfxType, float> LastPlayTimes = new Dictionary<SfxType, float>();
        private static GameSfx instance;

        [SerializeField]
        private float masterVolume = 0.8f;

        [SerializeField]
        private int audioSourceCount = 8;

        private readonly List<AudioSource> audioSources = new List<AudioSource>();
        private int nextSourceIndex;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
            LoadedClips.Clear();
            LastPlayTimes.Clear();
        }

        public static void Play(SfxType type)
        {
            EnsureInstance();
            instance.PlayInternal(type);
        }

        public static void PlayRandom(IReadOnlyList<SfxType> types)
        {
            if (types == null || types.Count == 0)
                return;

            Play(types[Random.Range(0, types.Count)]);
        }

        private static void EnsureInstance()
        {
            if (instance != null)
                return;

            GameObject sfxObject = new GameObject("Game SFX");
            instance = sfxObject.AddComponent<GameSfx>();
            DontDestroyOnLoad(sfxObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSources();
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        private void OnValidate()
        {
            masterVolume = Mathf.Clamp01(masterVolume);
            audioSourceCount = Mathf.Clamp(audioSourceCount, 1, 24);
        }

        private void PlayInternal(SfxType type)
        {
            if (!CanPlay(type))
                return;

            AudioClip clip = LoadClip(type);

            if (clip == null)
                return;

            EnsureAudioSources();
            AudioSource source = GetNextSource();
            source.pitch = GetPitch(type);
            source.PlayOneShot(clip, GetVolume(type) * masterVolume * GameOptions.MasterVolume * GameOptions.SfxVolume);
            LastPlayTimes[type] = Time.unscaledTime;
        }

        private static bool CanPlay(SfxType type)
        {
            float minInterval = MinIntervals.TryGetValue(type, out float value) ? value : 0f;

            if (!LastPlayTimes.TryGetValue(type, out float lastPlayTime))
                return true;

            if (Time.unscaledTime < lastPlayTime)
            {
                LastPlayTimes.Remove(type);
                return true;
            }

            return Time.unscaledTime - lastPlayTime >= minInterval;
        }

        private static float GetVolume(SfxType type)
        {
            return Volumes.TryGetValue(type, out float volume) ? volume : 0.5f;
        }

        private static float GetPitch(SfxType type)
        {
            if (!PitchRanges.TryGetValue(type, out Vector2 range))
                range = new Vector2(0.96f, 1.04f);

            float min = Mathf.Min(range.x, range.y);
            float max = Mathf.Max(range.x, range.y);
            return Mathf.Approximately(min, max) ? min : Random.Range(min, max);
        }

        private static AudioClip LoadClip(SfxType type)
        {
            if (LoadedClips.TryGetValue(type, out AudioClip loadedClip))
                return loadedClip;

            if (!ClipNames.TryGetValue(type, out string clipName))
                return null;

            AudioClip clip = Resources.Load<AudioClip>(ResourceRoot + clipName);
            LoadedClips[type] = clip;
            return clip;
        }

        private void EnsureAudioSources()
        {
            while (audioSources.Count < audioSourceCount)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;
                audioSources.Add(source);
            }
        }

        private AudioSource GetNextSource()
        {
            AudioSource source = audioSources[nextSourceIndex];
            nextSourceIndex = (nextSourceIndex + 1) % audioSources.Count;
            return source;
        }
    }
}
