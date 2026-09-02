using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Enemies;
using VampireLike.Growth;
using VampireLike.Audio;
using VampireLike.VFX;

namespace VampireLike.Combat
{
    /// <summary>
    /// 폭발탄, 냉기탄, 흡혈, 충격파처럼 투사체 적중과 처치에 반응하는 특수 강화를 처리한다.
    /// </summary>
    public class PlayerSpecialUpgradeController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask enemyLayerMask = 1 << 7;

        [SerializeField]
        private float explosionRadius = 1.45f;

        [SerializeField]
        private float explosionDamageRatioPerLevel = 0.28f;

        [SerializeField]
        private float frostDuration = 2f;

        [SerializeField]
        private float frostSlowMultiplierPerLevel = 0.08f;

        [SerializeField]
        private float vampirismChancePerLevel = 0.05f;

        [SerializeField]
        private int vampirismHealAmount = 1;

        [SerializeField]
        private int shockwaveBaseHitInterval = 9;

        [SerializeField]
        private float shockwaveRadius = 1.8f;

        [SerializeField]
        private float shockwaveDamageMultiplier = 0.75f;

        [SerializeField]
        private Color explosionColor = new Color(1f, 0.35f, 0.1f, 0.65f);

        [SerializeField]
        private Color shockwaveColor = new Color(0.82f, 0.94f, 1f, 0.7f);

        [SerializeField]
        private float scatterAnglePerLevel = 10f;

        [SerializeField]
        private int scatterMaxSideCount = 2;

        [SerializeField]
        private float scatterProjectileDamageMultiplier = 0.7f;

        [SerializeField]
        private float chainRicochetRadius = 2.4f;

        [SerializeField]
        private float chainRicochetDamageRatio = 0.45f;

        [SerializeField]
        private float projectileReflectSearchRadius = 4.2f;

        [SerializeField]
        private float shieldRechargeTime = 22f;

        [SerializeField]
        private float shieldRechargeReductionPerLevel = 2f;

        [SerializeField]
        private float orbitRadius = 1.08f;

        [SerializeField]
        private float orbitDamageInterval = 0.55f;

        [SerializeField]
        private int orbitBladeDamage = 1;

        [SerializeField]
        private float eclipseAuraBaseRadius = 1.25f;

        [SerializeField]
        private float eclipseAuraRadiusPerLevel = 0.35f;

        [SerializeField]
        private int eclipseAuraBaseDamage = 1;

        [SerializeField]
        private int eclipseAuraDamagePerLevel = 1;

        [SerializeField]
        private float eclipseAuraDamageInterval = 1f;

        [SerializeField]
        private float eclipseAuraIntervalReductionPerLevel = 0.12f;

        [SerializeField]
        private Color eclipseAuraColor = new Color(0.38f, 0.12f, 0.72f, 0.5f);

        [SerializeField]
        private ShieldVFXController shieldVfxPrefab;

        [Header("Character Exclusive - Kael")]
        [SerializeField]
        private float kaelBlackSwordWaveRadius = 1f;

        [SerializeField]
        private float kaelBlackSwordWaveDamageRatio = 0.24f;

        [SerializeField]
        private float kaelGuardianResolveHealthThreshold = 0.55f;

        [SerializeField]
        private float kaelGuardianResolveBlockChancePerLevel = 0.14f;

        [SerializeField]
        private int kaelManaSlashBaseHitInterval = 5;

        [SerializeField]
        private float kaelManaSlashDamageRatio = 1.35f;

        [SerializeField]
        private float kaelManaSlashRadius = 1.65f;

        [SerializeField]
        private float kaelBlackIronBarrierCooldown = 16f;

        [SerializeField]
        private float kaelBlackIronBarrierCooldownReductionPerLevel = 3.5f;

        [SerializeField]
        private float kaelExecutionHealthThreshold = 0.38f;

        [SerializeField]
        private float kaelExecutionDamageRatioPerLevel = 0.32f;

        [SerializeField]
        private float kaelBlackIronRegenBaseInterval = 10f;

        [SerializeField]
        private float kaelBlackIronRegenLevelTwoInterval = 8f;

        [SerializeField]
        private float kaelBlackIronRegenLevelFourInterval = 6f;

        [SerializeField]
        private float kaelBlackIronRegenBaseRatio = 0.01f;

        [SerializeField]
        private float kaelBlackIronRegenLowHealthThreshold = 0.3f;

        [Header("Character Exclusive - Selene")]
        [SerializeField]
        private int seleneMoonlightMarkBaseRequiredStacks = 4;

        [SerializeField]
        private float seleneMoonlightMarkDamageRatio = 0.8f;

        [SerializeField]
        private float seleneMoonlightMarkExplosionRadius = 1.25f;

        [SerializeField]
        private int seleneSilverMoonWaveBaseInterval = 5;

        [SerializeField]
        private float seleneSilverMoonWaveRadius = 1.65f;

        [SerializeField]
        private float seleneSilverMoonWaveDamageRatio = 0.7f;

        [SerializeField]
        private float seleneSilverMoonWaveSlowMultiplier = 0.82f;

        [SerializeField]
        private float seleneNebulaZoneDuration = 2f;

        [SerializeField]
        private float seleneNebulaZoneRadius = 1.15f;

        [SerializeField]
        private float seleneNebulaZoneTickInterval = 0.5f;

        [SerializeField]
        private float seleneNebulaZoneDamageRatio = 0.22f;

        [SerializeField]
        private float seleneNebulaZoneLevelThreeDamageMultiplier = 1.25f;

        [SerializeField]
        private float seleneStarChainChanceLevelOne = 0.2f;

        [SerializeField]
        private float seleneStarChainChanceLevelTwo = 0.35f;

        [SerializeField]
        private float seleneStarChainDamageRatio = 0.45f;

        [SerializeField]
        private float seleneStarChainLevelThreeDamageRatio = 0.6f;

        [SerializeField]
        private float seleneStarChainRadius = 2.8f;

        [SerializeField]
        private float seleneFullMoonBaseCooldown = 12f;

        [SerializeField]
        private float seleneFullMoonLevelTwoCooldown = 9f;

        [SerializeField]
        private float seleneFullMoonRadius = 2.1f;

        [SerializeField]
        private float seleneFullMoonDamageRatio = 1.15f;

        [SerializeField]
        private float seleneFullMoonMeteorWarningDuration = 0.52f;

        [SerializeField]
        private string seleneShadowStepEnemyLayerName = "Enemy";

        [SerializeField]
        private int seleneEclipseResonanceBaseRequiredHits = 4;

        [SerializeField]
        private float seleneEclipseResonanceRadius = 1.15f;

        [SerializeField]
        private float seleneEclipseResonanceDamageRatio = 0.35f;

        [Header("Character Exclusive - Han Seorin")]
        [SerializeField]
        private int hanSeorinBloodMarkBaseRequiredStacks = 5;

        [SerializeField]
        private float hanSeorinBloodMarkDamageRatio = 0.8f;

        [SerializeField]
        private float hanSeorinBloodMarkLevelThreeDamageRatio = 1.2f;

        [SerializeField]
        private float hanSeorinBloodMarkSplashRadius = 1.15f;

        [SerializeField]
        private float hanSeorinBloodMarkSplashDamageRatio = 0.5f;

        [SerializeField]
        private float hanSeorinShadowDaggerChanceLevelOne = 0.15f;

        [SerializeField]
        private float hanSeorinShadowDaggerChanceLevelTwo = 0.25f;

        [SerializeField]
        private float hanSeorinShadowDaggerChanceLevelThree = 0.35f;

        [SerializeField]
        private float hanSeorinShadowDaggerAngle = 4f;

        [SerializeField]
        private float hanSeorinShadowDaggerDamageMultiplier = 0.78f;

        [SerializeField]
        private float hanSeorinReturningBladeLevelTwoDamageMultiplier = 1.3f;

        [SerializeField]
        private float hanSeorinKillingIntentBonusPerHit = 0.05f;

        [SerializeField]
        private float hanSeorinRedExecutionHealthThreshold = 0.3f;

        [SerializeField]
        private float hanSeorinRedExecutionBonusPerLevel = 0.2f;

        [SerializeField]
        private float hanSeorinRedExecutionInstantKillThreshold = 0.1f;

        [SerializeField]
        private float hanSeorinBloodFangDuration = 3f;

        [SerializeField]
        private float hanSeorinBloodFangLevelThreeDuration = 4f;

        [SerializeField]
        private float hanSeorinBloodFangTickInterval = 1f;

        [SerializeField]
        private float hanSeorinBloodFangLevelOneDamageRatio = 0.15f;

        [SerializeField]
        private float hanSeorinBloodFangLevelTwoDamageRatio = 0.22f;

        [SerializeField]
        private float hanSeorinBloodFangLevelFourDamageRatio = 0.3f;

        [SerializeField]
        private float hanSeorinBloodFangBleedingTargetBonus = 0.15f;

        private readonly Collider2D[] areaResults = new Collider2D[64];
        private readonly List<OrbitingBlade> orbitingBlades = new List<OrbitingBlade>();
        private readonly List<SeleneNebulaZone> seleneNebulaZones = new List<SeleneNebulaZone>();
        private readonly List<HanSeorinBleed> hanSeorinBleeds = new List<HanSeorinBleed>();
        private readonly Dictionary<EnemyHealth, int> moonlightMarkStacks = new Dictionary<EnemyHealth, int>();
        private readonly Dictionary<EnemyHealth, int> seleneEclipseResonanceStacks = new Dictionary<EnemyHealth, int>();
        private readonly Dictionary<EnemyHealth, int> hanSeorinBloodMarkStacks = new Dictionary<EnemyHealth, int>();
        private int explosiveShotLevel;
        private int frostShotLevel;
        private int vampirismLevel;
        private int shockwaveLevel;
        private int scatterShotLevel;
        private int shieldLevel;
        private int orbitingBladeLevel;
        private int chainRicochetLevel;
        private int eclipseAuraLevel;
        private int projectileReflectLevel;
        private int kaelBlackSwordWaveLevel;
        private int kaelGuardianResolveLevel;
        private int kaelManaSlashLevel;
        private int kaelBlackIronBarrierLevel;
        private int kaelExecutionBladeLevel;
        private int kaelBlackIronRegenLevel;
        private int seleneMoonShadowCloneLevel;
        private int seleneShadowStepLevel;
        private int seleneTwinMoonFlurryLevel;
        private int seleneMoonlightMarkLevel;
        private int seleneSilentBladeLevel;
        private int seleneEclipseResonanceLevel;
        private int hanSeorinBloodMarkLevel;
        private int hanSeorinShadowDaggerLevel;
        private int hanSeorinReturningBladeLevel;
        private int hanSeorinKillingIntentLevel;
        private int hanSeorinRedExecutionLevel;
        private int hanSeorinBloodFangLevel;
        private int projectileHitCount;
        private int kaelManaSlashHitCount;
        private int seleneSilverMoonWaveAttackCount;
        private EnemyHealth hanSeorinKillingIntentTarget;
        private int hanSeorinKillingIntentStacks;
        private bool seleneSilverMoonWavePending;
        private Vector2 seleneSilverMoonWaveDirection = Vector2.right;
        private float shieldTimer;
        private float eclipseAuraTimer;
        private float kaelBlackIronBarrierTimer;
        private float kaelBlackIronRegenTimer;
        private bool kaelBlackIronBarrierReadySoundPlayed;
        private float seleneFullMoonTimer;
        private bool shieldReady;
        private PlayerHealth playerHealth;
        private PlayerEffectAnchors effectAnchors;
        private ShieldVFXController shieldVfx;
        private EclipseAuraVFX eclipseAuraVfx;
        private int shadowStepPlayerLayer = -1;
        private int shadowStepEnemyLayer = -1;
        private bool shadowStepCollisionPhaseActive;
        private bool shadowStepStoredCollisionIgnored;
        private float shadowStepCollisionPhaseTimer;

        private sealed class SeleneNebulaZone
        {
            public Vector2 Position;
            public float RemainingTime;
            public float TickTimer;
            public float Radius;
            public float Damage;
            public GameObject Visual;
        }

        private sealed class HanSeorinBleed
        {
            public EnemyHealth Enemy;
            public float RemainingTime;
            public float TickTimer;
            public float DamagePerTick;
        }

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            effectAnchors = GetComponent<PlayerEffectAnchors>();

            if (effectAnchors == null)
                effectAnchors = gameObject.AddComponent<PlayerEffectAnchors>();
        }

        private void OnValidate()
        {
            explosionRadius = Mathf.Max(0.1f, explosionRadius);
            explosionDamageRatioPerLevel = Mathf.Max(0.05f, explosionDamageRatioPerLevel);
            frostDuration = Mathf.Max(0.1f, frostDuration);
            frostSlowMultiplierPerLevel = Mathf.Clamp(frostSlowMultiplierPerLevel, 0.01f, 0.25f);
            vampirismChancePerLevel = Mathf.Clamp01(vampirismChancePerLevel);
            vampirismHealAmount = Mathf.Max(1, vampirismHealAmount);
            shockwaveBaseHitInterval = Mathf.Max(1, shockwaveBaseHitInterval);
            shockwaveRadius = Mathf.Max(0.1f, shockwaveRadius);
            shockwaveDamageMultiplier = Mathf.Max(0.1f, shockwaveDamageMultiplier);
            scatterAnglePerLevel = Mathf.Clamp(scatterAnglePerLevel, 1f, 20f);
            scatterMaxSideCount = Mathf.Clamp(scatterMaxSideCount, 1, 3);
            scatterProjectileDamageMultiplier = Mathf.Clamp(scatterProjectileDamageMultiplier, 0.1f, 1f);
            chainRicochetRadius = Mathf.Max(0.1f, chainRicochetRadius);
            chainRicochetDamageRatio = Mathf.Max(0.1f, chainRicochetDamageRatio);
            projectileReflectSearchRadius = Mathf.Max(0.5f, projectileReflectSearchRadius);
            shieldRechargeTime = Mathf.Max(1f, shieldRechargeTime);
            shieldRechargeReductionPerLevel = Mathf.Max(0f, shieldRechargeReductionPerLevel);
            orbitRadius = Mathf.Max(0.2f, orbitRadius);
            orbitDamageInterval = Mathf.Max(0.05f, orbitDamageInterval);
            orbitBladeDamage = Mathf.Max(1, orbitBladeDamage);
            eclipseAuraBaseRadius = Mathf.Max(0.3f, eclipseAuraBaseRadius);
            eclipseAuraRadiusPerLevel = Mathf.Max(0f, eclipseAuraRadiusPerLevel);
            eclipseAuraBaseDamage = Mathf.Max(1, eclipseAuraBaseDamage);
            eclipseAuraDamagePerLevel = Mathf.Max(0, eclipseAuraDamagePerLevel);
            eclipseAuraDamageInterval = Mathf.Max(0.2f, eclipseAuraDamageInterval);
            eclipseAuraIntervalReductionPerLevel = Mathf.Max(0f, eclipseAuraIntervalReductionPerLevel);
            kaelBlackSwordWaveRadius = Mathf.Max(0.1f, kaelBlackSwordWaveRadius);
            kaelBlackSwordWaveDamageRatio = Mathf.Max(0.01f, kaelBlackSwordWaveDamageRatio);
            kaelGuardianResolveHealthThreshold = Mathf.Clamp01(kaelGuardianResolveHealthThreshold);
            kaelGuardianResolveBlockChancePerLevel = Mathf.Clamp01(kaelGuardianResolveBlockChancePerLevel);
            kaelManaSlashBaseHitInterval = Mathf.Max(1, kaelManaSlashBaseHitInterval);
            kaelManaSlashDamageRatio = Mathf.Max(0.1f, kaelManaSlashDamageRatio);
            kaelManaSlashRadius = Mathf.Max(0.1f, kaelManaSlashRadius);
            kaelBlackIronBarrierCooldown = Mathf.Max(1f, kaelBlackIronBarrierCooldown);
            kaelBlackIronBarrierCooldownReductionPerLevel = Mathf.Max(0f, kaelBlackIronBarrierCooldownReductionPerLevel);
            kaelExecutionHealthThreshold = Mathf.Clamp01(kaelExecutionHealthThreshold);
            kaelExecutionDamageRatioPerLevel = Mathf.Max(0.01f, kaelExecutionDamageRatioPerLevel);
            kaelBlackIronRegenBaseInterval = Mathf.Max(1f, kaelBlackIronRegenBaseInterval);
            kaelBlackIronRegenLevelTwoInterval = Mathf.Max(1f, kaelBlackIronRegenLevelTwoInterval);
            kaelBlackIronRegenLevelFourInterval = Mathf.Max(1f, kaelBlackIronRegenLevelFourInterval);
            kaelBlackIronRegenBaseRatio = Mathf.Clamp01(kaelBlackIronRegenBaseRatio);
            kaelBlackIronRegenLowHealthThreshold = Mathf.Clamp01(kaelBlackIronRegenLowHealthThreshold);
            seleneMoonlightMarkBaseRequiredStacks = Mathf.Max(2, seleneMoonlightMarkBaseRequiredStacks);
            seleneMoonlightMarkDamageRatio = Mathf.Max(0.1f, seleneMoonlightMarkDamageRatio);
            seleneMoonlightMarkExplosionRadius = Mathf.Max(0.2f, seleneMoonlightMarkExplosionRadius);
            seleneSilverMoonWaveBaseInterval = Mathf.Max(1, seleneSilverMoonWaveBaseInterval);
            seleneSilverMoonWaveRadius = Mathf.Max(0.2f, seleneSilverMoonWaveRadius);
            seleneSilverMoonWaveDamageRatio = Mathf.Max(0.1f, seleneSilverMoonWaveDamageRatio);
            seleneSilverMoonWaveSlowMultiplier = Mathf.Clamp(seleneSilverMoonWaveSlowMultiplier, 0.2f, 1f);
            seleneNebulaZoneDuration = Mathf.Max(0.2f, seleneNebulaZoneDuration);
            seleneNebulaZoneRadius = Mathf.Max(0.2f, seleneNebulaZoneRadius);
            seleneNebulaZoneTickInterval = Mathf.Max(0.1f, seleneNebulaZoneTickInterval);
            seleneNebulaZoneDamageRatio = Mathf.Max(0.01f, seleneNebulaZoneDamageRatio);
            seleneNebulaZoneLevelThreeDamageMultiplier = Mathf.Max(1f, seleneNebulaZoneLevelThreeDamageMultiplier);
            seleneStarChainChanceLevelOne = Mathf.Clamp01(seleneStarChainChanceLevelOne);
            seleneStarChainChanceLevelTwo = Mathf.Clamp01(seleneStarChainChanceLevelTwo);
            seleneStarChainDamageRatio = Mathf.Max(0.1f, seleneStarChainDamageRatio);
            seleneStarChainLevelThreeDamageRatio = Mathf.Max(seleneStarChainDamageRatio, seleneStarChainLevelThreeDamageRatio);
            seleneStarChainRadius = Mathf.Max(0.3f, seleneStarChainRadius);
            seleneFullMoonBaseCooldown = Mathf.Max(1f, seleneFullMoonBaseCooldown);
            seleneFullMoonLevelTwoCooldown = Mathf.Max(1f, seleneFullMoonLevelTwoCooldown);
            seleneFullMoonRadius = Mathf.Max(0.3f, seleneFullMoonRadius);
            seleneFullMoonDamageRatio = Mathf.Max(0.1f, seleneFullMoonDamageRatio);
            seleneFullMoonMeteorWarningDuration = Mathf.Max(0.05f, seleneFullMoonMeteorWarningDuration);
            seleneEclipseResonanceBaseRequiredHits = Mathf.Max(2, seleneEclipseResonanceBaseRequiredHits);
            seleneEclipseResonanceRadius = Mathf.Max(0.2f, seleneEclipseResonanceRadius);
            seleneEclipseResonanceDamageRatio = Mathf.Max(0.01f, seleneEclipseResonanceDamageRatio);
            hanSeorinBloodMarkBaseRequiredStacks = Mathf.Max(2, hanSeorinBloodMarkBaseRequiredStacks);
            hanSeorinBloodMarkDamageRatio = Mathf.Max(0.1f, hanSeorinBloodMarkDamageRatio);
            hanSeorinBloodMarkLevelThreeDamageRatio = Mathf.Max(hanSeorinBloodMarkDamageRatio, hanSeorinBloodMarkLevelThreeDamageRatio);
            hanSeorinBloodMarkSplashRadius = Mathf.Max(0.1f, hanSeorinBloodMarkSplashRadius);
            hanSeorinBloodMarkSplashDamageRatio = Mathf.Clamp01(hanSeorinBloodMarkSplashDamageRatio);
            hanSeorinShadowDaggerChanceLevelOne = Mathf.Clamp01(hanSeorinShadowDaggerChanceLevelOne);
            hanSeorinShadowDaggerChanceLevelTwo = Mathf.Clamp01(hanSeorinShadowDaggerChanceLevelTwo);
            hanSeorinShadowDaggerChanceLevelThree = Mathf.Clamp01(hanSeorinShadowDaggerChanceLevelThree);
            hanSeorinShadowDaggerAngle = Mathf.Clamp(hanSeorinShadowDaggerAngle, 0f, 15f);
            hanSeorinShadowDaggerDamageMultiplier = Mathf.Clamp(hanSeorinShadowDaggerDamageMultiplier, 0.1f, 1f);
            hanSeorinReturningBladeLevelTwoDamageMultiplier = Mathf.Max(1f, hanSeorinReturningBladeLevelTwoDamageMultiplier);
            hanSeorinKillingIntentBonusPerHit = Mathf.Clamp(hanSeorinKillingIntentBonusPerHit, 0.01f, 0.2f);
            hanSeorinRedExecutionHealthThreshold = Mathf.Clamp01(hanSeorinRedExecutionHealthThreshold);
            hanSeorinRedExecutionBonusPerLevel = Mathf.Max(0.01f, hanSeorinRedExecutionBonusPerLevel);
            hanSeorinRedExecutionInstantKillThreshold = Mathf.Clamp01(hanSeorinRedExecutionInstantKillThreshold);
            hanSeorinBloodFangDuration = Mathf.Max(0.2f, hanSeorinBloodFangDuration);
            hanSeorinBloodFangLevelThreeDuration = Mathf.Max(hanSeorinBloodFangDuration, hanSeorinBloodFangLevelThreeDuration);
            hanSeorinBloodFangTickInterval = Mathf.Max(0.1f, hanSeorinBloodFangTickInterval);
            hanSeorinBloodFangLevelOneDamageRatio = Mathf.Max(0.01f, hanSeorinBloodFangLevelOneDamageRatio);
            hanSeorinBloodFangLevelTwoDamageRatio = Mathf.Max(hanSeorinBloodFangLevelOneDamageRatio, hanSeorinBloodFangLevelTwoDamageRatio);
            hanSeorinBloodFangLevelFourDamageRatio = Mathf.Max(hanSeorinBloodFangLevelTwoDamageRatio, hanSeorinBloodFangLevelFourDamageRatio);
            hanSeorinBloodFangBleedingTargetBonus = Mathf.Max(0f, hanSeorinBloodFangBleedingTargetBonus);
        }

        private void Update()
        {
            UpdateShieldRecharge();
            UpdateShieldAura();
            UpdateEclipseAura();
            UpdateSeleneNebulaZones();
            UpdateSeleneFullMoon();
            UpdateHanSeorinBleeds();
            UpdateCharacterExclusiveTimers();
            UpdateShadowStepCollisionPhase();
        }

        private void OnDisable()
        {
            EndShadowStepCollisionPhase();
        }

        public void AddExplosiveShotLevel()
        {
            explosiveShotLevel++;
        }

        public void AddFrostShotLevel()
        {
            frostShotLevel++;
        }

        public void AddVampirismLevel()
        {
            vampirismLevel++;
        }

        public void AddShockwaveLevel()
        {
            shockwaveLevel++;
        }

        public void AddScatterShotLevel()
        {
            scatterShotLevel++;
        }

        public void AddShieldLevel()
        {
            shieldLevel++;

            if (!shieldReady)
                shieldTimer = Mathf.Min(shieldTimer, GetShieldRechargeDuration());
        }

        public void AddOrbitingBladeLevel()
        {
            orbitingBladeLevel++;
            RefreshOrbitingBlades();
        }

        public void AddChainRicochetLevel()
        {
            chainRicochetLevel++;
        }

        public void AddEclipseAuraLevel()
        {
            eclipseAuraLevel++;
            eclipseAuraTimer = 0f;
            RefreshEclipseAuraVfx();
        }

        public void AddProjectileReflectLevel()
        {
            projectileReflectLevel++;
        }

        public int GetProjectileSplitLevel()
        {
            return Mathf.Clamp(projectileReflectLevel, 0, 3);
        }

        public void AddKaelBlackSwordWaveLevel()
        {
            kaelBlackSwordWaveLevel++;
        }

        public void AddKaelGuardianResolveLevel()
        {
            kaelGuardianResolveLevel++;
        }

        public void AddKaelManaSlashLevel()
        {
            kaelManaSlashLevel++;
        }

        public void AddKaelBlackIronBarrierLevel()
        {
            kaelBlackIronBarrierLevel++;
            kaelBlackIronBarrierTimer = Mathf.Min(kaelBlackIronBarrierTimer, GetKaelBlackIronBarrierCooldown());

            if (kaelBlackIronBarrierTimer <= 0f && !kaelBlackIronBarrierReadySoundPlayed)
            {
                GameSfx.Play(SfxType.KaelBlackIronBarrierOn);
                kaelBlackIronBarrierReadySoundPlayed = true;
            }
        }

        public void AddKaelExecutionBladeLevel()
        {
            kaelExecutionBladeLevel++;
        }

        public void AddKaelBlackIronRegenLevel()
        {
            kaelBlackIronRegenLevel++;
            kaelBlackIronRegenTimer = Mathf.Min(kaelBlackIronRegenTimer, GetKaelBlackIronRegenInterval());
        }

        public void AddSeleneMoonShadowCloneLevel()
        {
            seleneMoonShadowCloneLevel++;
        }

        public void AddSeleneShadowStepLevel()
        {
            seleneShadowStepLevel++;
        }

        public void AddSeleneTwinMoonFlurryLevel()
        {
            seleneTwinMoonFlurryLevel++;
        }

        public void AddSeleneMoonlightMarkLevel()
        {
            seleneMoonlightMarkLevel++;
        }

        public void AddSeleneSilentBladeLevel()
        {
            seleneSilentBladeLevel++;
            seleneFullMoonTimer = Mathf.Min(seleneFullMoonTimer, 0.25f);
        }

        public void AddSeleneEclipseResonanceLevel()
        {
            seleneEclipseResonanceLevel++;
        }

        public void AddHanSeorinBloodMarkLevel()
        {
            hanSeorinBloodMarkLevel++;
        }

        public void AddHanSeorinShadowDaggerLevel()
        {
            hanSeorinShadowDaggerLevel++;
        }

        public void AddHanSeorinReturningBladeLevel()
        {
            hanSeorinReturningBladeLevel++;
        }

        public void AddHanSeorinKillingIntentLevel()
        {
            hanSeorinKillingIntentLevel++;
        }

        public void AddHanSeorinRedExecutionLevel()
        {
            hanSeorinRedExecutionLevel++;
        }

        public void AddHanSeorinBloodFangLevel()
        {
            hanSeorinBloodFangLevel++;
        }

        public int GetAppliedUpgradeLevel(UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradeType.ExplosiveShot:
                    return explosiveShotLevel;
                case UpgradeType.FrostShot:
                    return frostShotLevel;
                case UpgradeType.Vampirism:
                    return vampirismLevel;
                case UpgradeType.Shockwave:
                    return shockwaveLevel;
                case UpgradeType.ScatterShot:
                    return scatterShotLevel;
                case UpgradeType.Shield:
                    return shieldLevel;
                case UpgradeType.OrbitingBlade:
                    return orbitingBladeLevel;
                case UpgradeType.ChainRicochet:
                    return chainRicochetLevel;
                case UpgradeType.EclipseAura:
                    return eclipseAuraLevel;
                case UpgradeType.ProjectileReflect:
                    return projectileReflectLevel;
                case UpgradeType.KaelBlackSwordWave:
                    return kaelBlackSwordWaveLevel;
                case UpgradeType.KaelGuardianResolve:
                    return kaelGuardianResolveLevel;
                case UpgradeType.KaelManaSlash:
                    return kaelManaSlashLevel;
                case UpgradeType.KaelBlackIronBarrier:
                    return kaelBlackIronBarrierLevel;
                case UpgradeType.KaelExecutionBlade:
                    return kaelExecutionBladeLevel;
                case UpgradeType.KaelBlackIronRegen:
                    return kaelBlackIronRegenLevel;
                case UpgradeType.SeleneMoonShadowClone:
                    return seleneMoonShadowCloneLevel;
                case UpgradeType.SeleneShadowStep:
                    return seleneShadowStepLevel;
                case UpgradeType.SeleneTwinMoonFlurry:
                    return seleneTwinMoonFlurryLevel;
                case UpgradeType.SeleneMoonlightMark:
                    return seleneMoonlightMarkLevel;
                case UpgradeType.SeleneSilentBlade:
                    return seleneSilentBladeLevel;
                case UpgradeType.SeleneEclipseResonance:
                    return seleneEclipseResonanceLevel;
                case UpgradeType.HanSeorinBloodMark:
                    return hanSeorinBloodMarkLevel;
                case UpgradeType.HanSeorinShadowDagger:
                    return hanSeorinShadowDaggerLevel;
                case UpgradeType.HanSeorinReturningBlade:
                    return hanSeorinReturningBladeLevel;
                case UpgradeType.HanSeorinKillingIntent:
                    return hanSeorinKillingIntentLevel;
                case UpgradeType.HanSeorinRedExecution:
                    return hanSeorinRedExecutionLevel;
                case UpgradeType.HanSeorinBloodFang:
                    return hanSeorinBloodFangLevel;
                default:
                    return 0;
            }
        }

        public int GetProjectileReflectCount()
        {
            return Mathf.Clamp(projectileReflectLevel, 0, 6);
        }

        public Vector2[] GetProjectileDirections(Vector2 baseDirection)
        {
            List<Vector2> directions = new List<Vector2>();

            directions.Add(baseDirection);

            if (scatterShotLevel > 0)
            {
                int additionalCount = Mathf.Clamp(scatterShotLevel, 1, scatterMaxSideCount);

                for (int i = 0; i < additionalCount; i++)
                {
                    float side = i % 2 == 0 ? 1f : -1f;
                    float step = i / 2 + 1f;
                    float angle = side * step * scatterAnglePerLevel;
                    directions.Add(Rotate(baseDirection, angle).normalized);
                }
            }

            if (seleneTwinMoonFlurryLevel > 0)
                CountSeleneSilverMoonWave(baseDirection);

            if (hanSeorinShadowDaggerLevel > 0 && Random.value < GetHanSeorinShadowDaggerChance())
            {
                directions.Add(Rotate(baseDirection, Random.Range(-hanSeorinShadowDaggerAngle, hanSeorinShadowDaggerAngle)).normalized);
                GameSfx.Play(SfxType.HanSeorinShadowDagger);
                CombatVFX.PlayBurst(GetEffectCenterPosition(), CombatVFXKind.Vampirism, 0.3f, 0.12f);
            }

            return directions.ToArray();
        }

        public float GetProjectileDamageMultiplierForDirections(int directionCount)
        {
            return scatterShotLevel > 0 && directionCount > 1 ? scatterProjectileDamageMultiplier : 1f;
        }

        public bool HasHanSeorinReturningBlade()
        {
            return hanSeorinReturningBladeLevel > 0;
        }

        public int GetHanSeorinReturningBladeBonusPierce()
        {
            if (hanSeorinReturningBladeLevel >= 5)
                return 2;

            return hanSeorinReturningBladeLevel >= 3 ? 1 : 0;
        }

        public float GetHanSeorinReturningBladeDamageMultiplier()
        {
            return hanSeorinReturningBladeLevel >= 2
                ? hanSeorinReturningBladeLevelTwoDamageMultiplier + 0.15f * Mathf.Max(0, hanSeorinReturningBladeLevel - 2)
                : 1f;
        }

        public float GetProjectileDamageMultiplierForEnemy(EnemyHealth enemy)
        {
            float multiplier = 1f;

            if (enemy != null && hanSeorinKillingIntentLevel > 0 && enemy == hanSeorinKillingIntentTarget)
                multiplier *= 1f + Mathf.Min(GetHanSeorinKillingIntentMaxBonus(), hanSeorinKillingIntentStacks * hanSeorinKillingIntentBonusPerHit);

            if (enemy != null && hanSeorinRedExecutionLevel > 0 && enemy.HealthProgress <= hanSeorinRedExecutionHealthThreshold)
                multiplier *= 1f + hanSeorinRedExecutionBonusPerLevel * hanSeorinRedExecutionLevel;

            if (enemy != null && hanSeorinBloodFangLevel >= 5 && IsHanSeorinBleeding(enemy))
                multiplier *= 1f + hanSeorinBloodFangBleedingTargetBonus;

            return multiplier;
        }

        public bool TryBlockDamage()
        {
            return TryBlockDamage(Vector2.zero);
        }

        public bool TryBlockDamage(Vector2 hitDirection)
        {
            if (TryBlockWithKaelBlackIronBarrier(hitDirection))
                return true;

            if (TryBlockWithKaelGuardianResolve(hitDirection))
                return true;

            if (shieldLevel <= 0 || !shieldReady)
                return false;

            shieldReady = false;
            shieldTimer = GetShieldRechargeDuration();
            GameSfx.Play(SfxType.ShieldBlock);
            BreakShieldVfx(hitDirection);
            return true;
        }

        public float GetBonusInvincibleDuration()
        {
            return 0f;
        }

        public void NotifyPlayerDamaged()
        {
        }

        public void HandleProjectileHit(EnemyHealth enemy, float projectileDamage, Vector2 hitPosition)
        {
            if (enemy == null || projectileDamage <= 0)
                return;

            if (frostShotLevel > 0 && !enemy.IsDead)
                ApplyFrost(enemy);

            if (shockwaveLevel > 0)
                CountShockwaveHit(projectileDamage, hitPosition);

            if (chainRicochetLevel > 0)
                TriggerChainRicochet(enemy, projectileDamage, hitPosition);

            if (seleneSilverMoonWavePending)
                TriggerSeleneSilverMoonWave(projectileDamage);

            if (kaelBlackSwordWaveLevel > 0)
                TriggerKaelBlackSwordWave(enemy, projectileDamage, hitPosition);

            if (kaelManaSlashLevel > 0)
                CountKaelManaSlash(projectileDamage, hitPosition);

            if (kaelExecutionBladeLevel > 0 && !enemy.IsDead)
                TryKaelExecutionBlade(enemy, projectileDamage);

            if (seleneMoonlightMarkLevel > 0 && !enemy.IsDead)
                ApplySeleneMoonlightMark(enemy, projectileDamage);

            if (seleneShadowStepLevel > 0)
                CreateSeleneNebulaZone(hitPosition, projectileDamage);

            if (seleneMoonShadowCloneLevel > 0)
                TrySeleneStarChain(enemy, projectileDamage, hitPosition);

            if (hanSeorinBloodMarkLevel > 0 && !enemy.IsDead)
                ApplyHanSeorinBloodMark(enemy, projectileDamage);

            if (hanSeorinBloodFangLevel > 0 && !enemy.IsDead)
                ApplyHanSeorinBloodFang(enemy, projectileDamage);

            if (hanSeorinKillingIntentLevel > 0 && !enemy.IsDead)
                CountHanSeorinKillingIntent(enemy);

            if (hanSeorinRedExecutionLevel > 0 && !enemy.IsDead)
                TryHanSeorinRedExecution(enemy);
        }

        public void HandleProjectileKill(EnemyHealth killedEnemy, float projectileDamage, Vector2 killPosition)
        {
            if (projectileDamage <= 0)
                return;

            if (explosiveShotLevel > 0)
                TriggerExplosion(killedEnemy, projectileDamage, killPosition);

            if (vampirismLevel > 0)
                TryVampirismHeal();
        }

        private void ApplyFrost(EnemyHealth enemy)
        {
            EnemyStatusEffects statusEffects = enemy.GetComponent<EnemyStatusEffects>();

            if (statusEffects == null)
                statusEffects = enemy.gameObject.AddComponent<EnemyStatusEffects>();

            float multiplier = Mathf.Clamp(1f - frostSlowMultiplierPerLevel * frostShotLevel, 0.45f, 1f);
            statusEffects.ApplySlow(multiplier, frostDuration);
            GameSfx.Play(SfxType.SkillFrost);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Frost, 0.62f, 0.24f);
        }

        private void CountShockwaveHit(float projectileDamage, Vector2 hitPosition)
        {
            projectileHitCount++;

            if (projectileHitCount < GetShockwaveHitInterval())
                return;

            projectileHitCount = 0;
            float damage = Mathf.Max(0.1f, projectileDamage * shockwaveDamageMultiplier);
            ApplyAreaDamage(hitPosition, shockwaveRadius, damage, null);
            GameSfx.Play(SfxType.SkillShockwave);
            CombatVFX.PlayBurst(hitPosition, CombatVFXKind.Shockwave, shockwaveRadius, 0.36f);
        }

        private int GetShockwaveHitInterval()
        {
            return Mathf.Max(3, shockwaveBaseHitInterval - Mathf.Max(0, shockwaveLevel - 1));
        }

        private void TriggerExplosion(EnemyHealth killedEnemy, float projectileDamage, Vector2 killPosition)
        {
            float damage = Mathf.Max(0.1f, projectileDamage * explosionDamageRatioPerLevel * explosiveShotLevel);
            ApplyAreaDamage(killPosition, explosionRadius, damage, killedEnemy);
            GameSfx.Play(SfxType.SkillExplosion);
            CombatVFX.PlayBurst(killPosition, CombatVFXKind.Explosion, explosionRadius, 0.32f);
        }

        private void ApplyAreaDamage(Vector2 center, float radius, float damage, EnemyHealth excludedEnemy, bool allowSeleneResonance = true)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(center, radius, areaResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy == excludedEnemy || enemy.IsDead)
                    continue;

                enemy.TakeDamage(damage);

                if (allowSeleneResonance && seleneEclipseResonanceLevel > 0 && !enemy.IsDead)
                    CountSeleneEclipseResonance(enemy, damage);
            }
        }

        private void ApplyBoxDamage(Vector2 center, Vector2 size, float angle, float damage, EnemyHealth excludedEnemy, bool allowSeleneResonance = true)
        {
            int hitCount = Physics2D.OverlapBoxNonAlloc(center, size, angle, areaResults, enemyLayerMask);
            HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy == excludedEnemy || enemy.IsDead || !damagedEnemies.Add(enemy))
                    continue;

                enemy.TakeDamage(damage);

                if (allowSeleneResonance && seleneEclipseResonanceLevel > 0 && !enemy.IsDead)
                    CountSeleneEclipseResonance(enemy, damage);
            }
        }

        private void TriggerKaelBlackSwordWave(EnemyHealth hitEnemy, float projectileDamage, Vector2 hitPosition)
        {
            float radius = kaelBlackSwordWaveRadius + 0.12f * Mathf.Max(0, kaelBlackSwordWaveLevel - 1);
            float damage = projectileDamage * kaelBlackSwordWaveDamageRatio * kaelBlackSwordWaveLevel;
            ApplyAreaDamage(hitPosition, radius, damage, hitEnemy);
            GameSfx.Play(SfxType.KaelBlackWave);
            CombatVFX.PlayBurst(hitPosition, CombatVFXKind.ArcaneImpact, radius, 0.2f);
        }

        private void CountKaelManaSlash(float projectileDamage, Vector2 hitPosition)
        {
            kaelManaSlashHitCount++;

            if (kaelManaSlashHitCount < GetKaelManaSlashInterval())
                return;

            kaelManaSlashHitCount = 0;
            ApplyAreaDamage(hitPosition, kaelManaSlashRadius, projectileDamage * GetKaelManaSlashDamageRatio(), null);
            GameSfx.Play(SfxType.KaelMagicSlash);
            CombatVFX.PlayBurst(hitPosition, CombatVFXKind.Shockwave, kaelManaSlashRadius, 0.28f);
        }

        private int GetKaelManaSlashInterval()
        {
            return Mathf.Max(3, kaelManaSlashBaseHitInterval - Mathf.Max(0, kaelManaSlashLevel - 1));
        }

        private float GetKaelManaSlashDamageRatio()
        {
            return kaelManaSlashDamageRatio * (1f + 0.18f * Mathf.Max(0, kaelManaSlashLevel - 1));
        }

        private void TryKaelExecutionBlade(EnemyHealth enemy, float projectileDamage)
        {
            float threshold = Mathf.Clamp01(kaelExecutionHealthThreshold + 0.04f * Mathf.Max(0, kaelExecutionBladeLevel - 1));

            if (enemy.HealthProgress > threshold)
                return;

            float damage = projectileDamage * kaelExecutionDamageRatioPerLevel * kaelExecutionBladeLevel;
            enemy.TakeDamage(damage);
            GameSfx.Play(SfxType.KaelExecutionBlade);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Explosion, 0.36f, 0.14f);
        }

        private void ApplySeleneMoonlightMark(EnemyHealth enemy, float projectileDamage)
        {
            if (!moonlightMarkStacks.TryGetValue(enemy, out int stacks))
                stacks = 0;

            stacks++;

            if (stacks < GetSeleneMoonlightMarkRequiredStacks())
            {
                moonlightMarkStacks[enemy] = stacks;
                GameSfx.Play(SfxType.SeleneMoonmarkApply);
                CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Frost, 0.28f, 0.1f);
                return;
            }

            moonlightMarkStacks.Remove(enemy);
            float damage = projectileDamage * GetSeleneMoonlightMarkDamageRatio();
            ApplyAreaDamage(enemy.transform.position, GetSeleneMoonlightMarkExplosionRadius(), damage, null);

            if (seleneMoonlightMarkLevel >= 3)
                SpreadSeleneMoonlightMark(enemy.transform.position, enemy);

            GameSfx.Play(SfxType.SeleneMoonmarkBurst);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Frost, GetSeleneMoonlightMarkExplosionRadius(), 0.22f);
        }

        private void TrySeleneStarChain(EnemyHealth firstEnemy, float projectileDamage, Vector2 startPosition)
        {
            float chance = GetSeleneStarChainChance();

            if (Random.value > chance)
                return;

            float damage = projectileDamage * GetSeleneStarChainDamageRatio();
            int maxTargets = GetSeleneStarChainMaxTargets();
            int chainedTargets = ChainSeleneStarDamage(startPosition, firstEnemy, damage, maxTargets);

            if (chainedTargets <= 0)
                return;

            GameSfx.Play(SfxType.SeleneStarlightChain);
        }

        private void CountSeleneEclipseResonance(EnemyHealth enemy, float areaDamage)
        {
            if (enemy == null || enemy.IsDead)
                return;

            int stacks = seleneEclipseResonanceStacks.TryGetValue(enemy, out int currentStacks) ? currentStacks : 0;
            stacks++;

            if (stacks < GetSeleneEclipseResonanceRequiredHits())
            {
                seleneEclipseResonanceStacks[enemy] = stacks;
                return;
            }

            seleneEclipseResonanceStacks.Remove(enemy);
            float damage = Mathf.Max(0.1f, areaDamage * GetSeleneEclipseResonanceDamageRatio());
            float radius = GetSeleneEclipseResonanceRadius();
            ApplyAreaDamage(enemy.transform.position, radius, damage, enemy, false);
            enemy.TakeDamage(damage);
            GameSfx.Play(SfxType.SeleneEclipseResonance);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Frost, radius, 0.16f);
        }

        private int GetSeleneEclipseResonanceRequiredHits()
        {
            int reduction = seleneEclipseResonanceLevel >= 2 ? 1 : 0;

            if (seleneEclipseResonanceLevel >= 5)
                reduction++;

            return Mathf.Max(2, seleneEclipseResonanceBaseRequiredHits - reduction);
        }

        private float GetSeleneEclipseResonanceDamageRatio()
        {
            float bonus = 0.12f * Mathf.Max(0, seleneEclipseResonanceLevel - 1);
            return seleneEclipseResonanceDamageRatio + bonus;
        }

        private float GetSeleneEclipseResonanceRadius()
        {
            float bonus = seleneEclipseResonanceLevel >= 3 ? 0.28f : 0f;
            bonus += 0.08f * Mathf.Max(0, seleneEclipseResonanceLevel - 3);
            return seleneEclipseResonanceRadius + bonus;
        }

        private int GetSeleneMoonlightMarkRequiredStacks()
        {
            int reduction = seleneMoonlightMarkLevel >= 2 ? 1 : 0;

            if (seleneMoonlightMarkLevel >= 5)
                reduction++;

            return Mathf.Max(2, seleneMoonlightMarkBaseRequiredStacks - reduction);
        }

        private float GetSeleneMoonlightMarkDamageRatio()
        {
            return seleneMoonlightMarkDamageRatio * (1f + 0.12f * Mathf.Max(0, seleneMoonlightMarkLevel - 1));
        }

        private float GetSeleneMoonlightMarkExplosionRadius()
        {
            float bonusRadius = seleneMoonlightMarkLevel >= 3 ? 0.35f : 0f;
            bonusRadius += 0.12f * Mathf.Max(0, seleneMoonlightMarkLevel - 3);
            return seleneMoonlightMarkExplosionRadius + bonusRadius;
        }

        private void SpreadSeleneMoonlightMark(Vector2 center, EnemyHealth excludedEnemy)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(center, GetSeleneMoonlightMarkExplosionRadius(), areaResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth target = hit.GetComponentInParent<EnemyHealth>();

                if (target == null || target == excludedEnemy || target.IsDead)
                    continue;

                int currentStacks = moonlightMarkStacks.TryGetValue(target, out int stacks) ? stacks : 0;
                moonlightMarkStacks[target] = Mathf.Min(GetSeleneMoonlightMarkRequiredStacks() - 1, currentStacks + 1);
                CombatVFX.PlayBurst(target.transform.position, CombatVFXKind.Frost, 0.24f, 0.08f);
            }
        }

        private void CountSeleneSilverMoonWave(Vector2 baseDirection)
        {
            seleneSilverMoonWaveAttackCount++;

            if (seleneSilverMoonWaveAttackCount < GetSeleneSilverMoonWaveInterval())
                return;

            seleneSilverMoonWaveAttackCount = 0;
            seleneSilverMoonWavePending = true;
            seleneSilverMoonWaveDirection = baseDirection.sqrMagnitude <= 0.001f ? Vector2.right : baseDirection.normalized;
        }

        private void TriggerSeleneSilverMoonWave(float projectileDamage)
        {
            seleneSilverMoonWavePending = false;
            Vector2 direction = seleneSilverMoonWaveDirection.sqrMagnitude <= 0.001f ? Vector2.right : seleneSilverMoonWaveDirection.normalized;
            Vector2 start = (Vector2)GetEffectCenterPosition() + direction * 0.55f;
            float length = GetSeleneSilverMoonWaveLength();
            float width = GetSeleneSilverMoonWaveWidth();
            Vector2 center = start + direction * (length * 0.5f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float damage = Mathf.Max(0.1f, projectileDamage * GetSeleneSilverMoonWaveDamageRatio());
            ApplyBoxDamage(center, new Vector2(length, width), angle, damage, null);

            if (seleneTwinMoonFlurryLevel >= 3)
                ApplyBoxSlow(center, new Vector2(length, width), angle, seleneSilverMoonWaveSlowMultiplier, frostDuration);

            GameSfx.Play(SfxType.SeleneSilvermoonWave);
            CombatVFX.PlaySilverMoonWave(start, direction, length, width, 0.24f);
        }

        private int GetSeleneSilverMoonWaveInterval()
        {
            int reduction = seleneTwinMoonFlurryLevel >= 2 ? 1 : 0;

            if (seleneTwinMoonFlurryLevel >= 4)
                reduction++;

            return Mathf.Max(2, seleneSilverMoonWaveBaseInterval - reduction);
        }

        private float GetSeleneSilverMoonWaveRadius()
        {
            return seleneSilverMoonWaveRadius + 0.12f * Mathf.Max(0, seleneTwinMoonFlurryLevel - 1);
        }

        private float GetSeleneSilverMoonWaveLength()
        {
            return GetSeleneSilverMoonWaveRadius() * 3.6f;
        }

        private float GetSeleneSilverMoonWaveWidth()
        {
            return 0.48f + 0.04f * Mathf.Max(0, seleneTwinMoonFlurryLevel - 1);
        }

        private float GetSeleneSilverMoonWaveDamageRatio()
        {
            return seleneSilverMoonWaveDamageRatio * (1f + 0.12f * Mathf.Max(0, seleneTwinMoonFlurryLevel - 1));
        }

        private void CreateSeleneNebulaZone(Vector2 position, float projectileDamage, bool playSpawnSfx = true)
        {
            SeleneNebulaZone zone = new SeleneNebulaZone
            {
                Position = position,
                RemainingTime = GetSeleneNebulaZoneDuration(),
                TickTimer = 0f,
                Radius = GetSeleneNebulaZoneRadius(),
                Damage = Mathf.Max(0.1f, projectileDamage * GetSeleneNebulaZoneDamageRatio())
            };
            zone.Visual = CombatVFX.CreateSeleneNebulaZoneVisual(position, zone.Radius);

            seleneNebulaZones.Add(zone);

            if (playSpawnSfx)
                GameSfx.Play(SfxType.SeleneNebulaSpawn);

            while (seleneNebulaZones.Count > 8)
                RemoveSeleneNebulaZoneAt(0);
        }

        private float GetSeleneNebulaZoneDuration()
        {
            return seleneNebulaZoneDuration + 0.7f * Mathf.Max(0, seleneShadowStepLevel - 1);
        }

        private float GetSeleneNebulaZoneRadius()
        {
            return seleneNebulaZoneRadius + 0.22f * Mathf.Max(0, seleneShadowStepLevel - 1);
        }

        private float GetSeleneNebulaZoneDamageRatio()
        {
            return seleneShadowStepLevel >= 3
                ? seleneNebulaZoneDamageRatio * seleneNebulaZoneLevelThreeDamageMultiplier * (1f + 0.08f * Mathf.Max(0, seleneShadowStepLevel - 3))
                : seleneNebulaZoneDamageRatio;
        }

        private int ChainSeleneStarDamage(Vector2 startPosition, EnemyHealth firstEnemy, float damage, int maxTargets)
        {
            int chainedTargets = 0;
            Vector2 currentPosition = startPosition;
            EnemyHealth currentEnemy = firstEnemy;

            for (int i = 0; i < maxTargets; i++)
            {
                EnemyHealth target = FindClosestEnemyAround(currentPosition, seleneStarChainRadius, currentEnemy);

                if (target == null)
                    break;

                CombatVFX.PlayChainLightning(currentPosition, target.transform.position, 0.2f, 0.065f);
                CombatVFX.PlayChainLightningImpact(target.transform.position, 0.22f, 0.1f);
                target.TakeDamage(damage);
                currentPosition = target.transform.position;
                currentEnemy = target;
                chainedTargets++;
            }

            return chainedTargets;
        }

        private float GetSeleneStarChainDamageRatio()
        {
            return seleneMoonShadowCloneLevel >= 3
                ? seleneStarChainLevelThreeDamageRatio + 0.08f * Mathf.Max(0, seleneMoonShadowCloneLevel - 3)
                : seleneStarChainDamageRatio;
        }

        private float GetSeleneStarChainChance()
        {
            if (seleneMoonShadowCloneLevel >= 3)
                return Mathf.Clamp01(seleneStarChainChanceLevelTwo + 0.08f * Mathf.Max(0, seleneMoonShadowCloneLevel - 2));

            return seleneMoonShadowCloneLevel >= 2 ? seleneStarChainChanceLevelTwo : seleneStarChainChanceLevelOne;
        }

        private int GetSeleneStarChainMaxTargets()
        {
            if (seleneMoonShadowCloneLevel < 3)
                return 2;

            return Mathf.Min(6, 4 + Mathf.Max(0, seleneMoonShadowCloneLevel - 3));
        }

        private void ApplyHanSeorinBloodMark(EnemyHealth enemy, float projectileDamage)
        {
            if (!hanSeorinBloodMarkStacks.TryGetValue(enemy, out int stacks))
                stacks = 0;

            stacks++;

            if (stacks < GetHanSeorinBloodMarkRequiredStacks())
            {
                hanSeorinBloodMarkStacks[enemy] = stacks;
                SetHanSeorinBloodMarkVfx(enemy, stacks);
                GameSfx.Play(SfxType.HanSeorinBloodMarkApply);
                CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Vampirism, 0.18f + 0.03f * stacks, 0.08f);
                return;
            }

            hanSeorinBloodMarkStacks.Remove(enemy);
            ClearHanSeorinBloodMarkVfx(enemy);
            float damage = projectileDamage * GetHanSeorinBloodMarkDamageRatio();
            enemy.TakeDamage(damage);

            if (hanSeorinBloodMarkLevel >= 3)
                ApplyAreaDamage(enemy.transform.position, GetHanSeorinBloodMarkSplashRadius(), damage * GetHanSeorinBloodMarkSplashDamageRatio(), enemy);

            GameSfx.Play(SfxType.HanSeorinBloodMarkBurst);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Explosion, hanSeorinBloodMarkLevel >= 3 ? GetHanSeorinBloodMarkSplashRadius() : 0.42f, 0.18f);
        }

        private void CountHanSeorinKillingIntent(EnemyHealth enemy)
        {
            if (enemy == null)
                return;

            if (hanSeorinKillingIntentTarget == enemy)
            {
                hanSeorinKillingIntentStacks++;
            }
            else
            {
                ClearHanSeorinKillingIntentVfx(hanSeorinKillingIntentTarget);
                hanSeorinKillingIntentTarget = enemy;
                hanSeorinKillingIntentStacks = 1;
            }

            if (IsHanSeorinKillingIntentMaxed())
                GameSfx.Play(SfxType.HanSeorinKillingIntentMax);

            SetHanSeorinKillingIntentVfx(enemy);
        }

        private void TryHanSeorinRedExecution(EnemyHealth enemy)
        {
            if (enemy == null || enemy.IsBoss || hanSeorinRedExecutionLevel < 3)
                return;

            if (enemy.HealthProgress > hanSeorinRedExecutionInstantKillThreshold)
                return;

            enemy.TakeDamage(enemy.MaxHealth);
            GameSfx.Play(SfxType.HanSeorinRedExecution);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Vampirism, 0.52f, 0.16f);
        }

        private void ApplyHanSeorinBloodFang(EnemyHealth enemy, float projectileDamage)
        {
            if (enemy == null || enemy.IsDead)
                return;

            HanSeorinBleed bleed = GetHanSeorinBleed(enemy);
            float damagePerTick = Mathf.Max(0.1f, projectileDamage * GetHanSeorinBloodFangDamageRatio());

            if (bleed == null)
            {
                bleed = new HanSeorinBleed { Enemy = enemy };
                hanSeorinBleeds.Add(bleed);
            }

            bleed.RemainingTime = GetHanSeorinBloodFangDuration();
            bleed.TickTimer = Mathf.Min(bleed.TickTimer, hanSeorinBloodFangTickInterval);
            bleed.DamagePerTick = Mathf.Max(bleed.DamagePerTick, damagePerTick);
            GameSfx.Play(SfxType.HanSeorinBloodFangApply);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Vampirism, 0.16f, 0.06f);
        }

        private HanSeorinBleed GetHanSeorinBleed(EnemyHealth enemy)
        {
            for (int i = 0; i < hanSeorinBleeds.Count; i++)
            {
                HanSeorinBleed bleed = hanSeorinBleeds[i];

                if (bleed.Enemy == enemy)
                    return bleed;
            }

            return null;
        }

        private bool IsHanSeorinBleeding(EnemyHealth enemy)
        {
            HanSeorinBleed bleed = GetHanSeorinBleed(enemy);
            return bleed != null && bleed.RemainingTime > 0f;
        }

        private float GetHanSeorinBloodFangDuration()
        {
            return hanSeorinBloodFangLevel >= 3 ? hanSeorinBloodFangLevelThreeDuration : hanSeorinBloodFangDuration;
        }

        private float GetHanSeorinBloodFangDamageRatio()
        {
            if (hanSeorinBloodFangLevel >= 4)
                return hanSeorinBloodFangLevelFourDamageRatio + 0.04f * Mathf.Max(0, hanSeorinBloodFangLevel - 4);

            if (hanSeorinBloodFangLevel >= 2)
                return hanSeorinBloodFangLevelTwoDamageRatio;

            return hanSeorinBloodFangLevelOneDamageRatio;
        }

        private int GetHanSeorinBloodMarkRequiredStacks()
        {
            int reduction = hanSeorinBloodMarkLevel >= 2 ? 1 : 0;

            if (hanSeorinBloodMarkLevel >= 5)
                reduction++;

            return Mathf.Max(2, hanSeorinBloodMarkBaseRequiredStacks - reduction);
        }

        private float GetHanSeorinBloodMarkDamageRatio()
        {
            return hanSeorinBloodMarkLevel >= 3
                ? hanSeorinBloodMarkLevelThreeDamageRatio + 0.15f * Mathf.Max(0, hanSeorinBloodMarkLevel - 3)
                : hanSeorinBloodMarkDamageRatio;
        }

        private float GetHanSeorinBloodMarkSplashRadius()
        {
            return hanSeorinBloodMarkSplashRadius + 0.12f * Mathf.Max(0, hanSeorinBloodMarkLevel - 3);
        }

        private float GetHanSeorinBloodMarkSplashDamageRatio()
        {
            return Mathf.Clamp01(hanSeorinBloodMarkSplashDamageRatio + 0.08f * Mathf.Max(0, hanSeorinBloodMarkLevel - 3));
        }

        private float GetHanSeorinShadowDaggerChance()
        {
            if (hanSeorinShadowDaggerLevel >= 3)
                return Mathf.Clamp01(hanSeorinShadowDaggerChanceLevelThree + 0.08f * Mathf.Max(0, hanSeorinShadowDaggerLevel - 3));

            if (hanSeorinShadowDaggerLevel == 2)
                return hanSeorinShadowDaggerChanceLevelTwo;

            return hanSeorinShadowDaggerChanceLevelOne;
        }

        private float GetHanSeorinKillingIntentMaxBonus()
        {
            return 0.15f * Mathf.Clamp(hanSeorinKillingIntentLevel, 1, 5);
        }

        private bool IsHanSeorinKillingIntentMaxed()
        {
            if (hanSeorinKillingIntentLevel <= 0)
                return false;

            int requiredStacks = Mathf.CeilToInt(GetHanSeorinKillingIntentMaxBonus() / Mathf.Max(0.01f, hanSeorinKillingIntentBonusPerHit));
            return hanSeorinKillingIntentStacks == requiredStacks;
        }

        private void SetHanSeorinBloodMarkVfx(EnemyHealth enemy, int stacks)
        {
            HanSeorinMarkVFX marker = GetHanSeorinMarkVfx(enemy);

            if (marker != null)
                marker.SetBloodMark(stacks, GetHanSeorinBloodMarkRequiredStacks());
        }

        private void ClearHanSeorinBloodMarkVfx(EnemyHealth enemy)
        {
            HanSeorinMarkVFX marker = enemy == null ? null : enemy.GetComponent<HanSeorinMarkVFX>();

            if (marker != null)
                marker.ClearBloodMark();
        }

        private void SetHanSeorinKillingIntentVfx(EnemyHealth enemy)
        {
            HanSeorinMarkVFX marker = GetHanSeorinMarkVfx(enemy);

            if (marker == null)
                return;

            float currentBonus = hanSeorinKillingIntentStacks * hanSeorinKillingIntentBonusPerHit;
            float maxBonus = Mathf.Max(0.01f, GetHanSeorinKillingIntentMaxBonus());
            marker.SetKillingIntent(Mathf.Clamp01(currentBonus / maxBonus));
        }

        private static void ClearHanSeorinKillingIntentVfx(EnemyHealth enemy)
        {
            HanSeorinMarkVFX marker = enemy == null ? null : enemy.GetComponent<HanSeorinMarkVFX>();

            if (marker != null)
                marker.ClearKillingIntent();
        }

        private static HanSeorinMarkVFX GetHanSeorinMarkVfx(EnemyHealth enemy)
        {
            if (enemy == null || enemy.IsDead)
                return null;

            HanSeorinMarkVFX marker = enemy.GetComponent<HanSeorinMarkVFX>();

            if (marker == null)
                marker = enemy.gameObject.AddComponent<HanSeorinMarkVFX>();

            return marker;
        }

        private void TryVampirismHeal()
        {
            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth == null)
                return;

            float chance = Mathf.Clamp01(vampirismChancePerLevel * vampirismLevel);

            if (Random.value <= chance)
            {
                playerHealth.Heal(vampirismHealAmount);
                GameSfx.Play(SfxType.SkillVampirism);
                CombatVFX.PlayBurst(GetEffectCenterPosition(), CombatVFXKind.Vampirism, 0.62f, 0.3f);
            }
        }

        private void TriggerChainRicochet(EnemyHealth firstEnemy, float projectileDamage, Vector2 startPosition)
        {
            EnemyHealth currentEnemy = firstEnemy;
            Vector2 currentPosition = startPosition;
            float damage = Mathf.Max(0.1f, projectileDamage * chainRicochetDamageRatio);
            int maxChains = Mathf.Clamp(chainRicochetLevel, 1, 6);

            for (int i = 0; i < maxChains; i++)
            {
                EnemyHealth nextEnemy = FindClosestChainTarget(currentPosition, currentEnemy);

                if (nextEnemy == null)
                    return;

                GameSfx.Play(SfxType.SkillRicochet);
                CombatVFX.PlayChainLightning(currentPosition, nextEnemy.transform.position, 0.24f, 0.075f);
                CombatVFX.PlayChainLightningImpact(nextEnemy.transform.position, 0.24f, 0.12f);
                nextEnemy.TakeDamage(damage);

                currentEnemy = nextEnemy;
                currentPosition = nextEnemy.transform.position;
            }
        }

        private EnemyHealth FindClosestChainTarget(Vector2 position, EnemyHealth excludedEnemy)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(position, chainRicochetRadius, areaResults, enemyLayerMask);
            EnemyHealth closestEnemy = null;
            float closestSqrDistance = chainRicochetRadius * chainRicochetRadius;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy == excludedEnemy || enemy.IsDead)
                    continue;

                float sqrDistance = ((Vector2)enemy.transform.position - position).sqrMagnitude;

                if (sqrDistance > closestSqrDistance)
                    continue;

                closestEnemy = enemy;
                closestSqrDistance = sqrDistance;
            }

            return closestEnemy;
        }

        public bool TryGetProjectileReflectDirection(Vector2 position, IReadOnlyCollection<EnemyHealth> ignoredEnemies, out Vector2 direction)
        {
            direction = Vector2.zero;

            if (projectileReflectLevel <= 0)
                return false;

            EnemyHealth target = FindClosestProjectileReflectTarget(position, ignoredEnemies);

            if (target == null)
                return false;

            direction = ((Vector2)target.transform.position - position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                return false;

            GameSfx.Play(SfxType.SkillRicochet);
            CombatVFX.PlayLine(position, target.transform.position, CombatVFXKind.Ricochet, 0.14f, 0.08f);
            CombatVFX.PlayBurst(position, CombatVFXKind.Ricochet, 0.36f, 0.14f);
            return true;
        }

        private EnemyHealth FindClosestProjectileReflectTarget(Vector2 position, IReadOnlyCollection<EnemyHealth> ignoredEnemies)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(position, projectileReflectSearchRadius, areaResults, enemyLayerMask);
            EnemyHealth closestEnemy = null;
            float closestSqrDistance = projectileReflectSearchRadius * projectileReflectSearchRadius;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy.IsDead)
                    continue;

                if (IsIgnoredProjectileReflectEnemy(enemy, ignoredEnemies))
                    continue;

                float sqrDistance = ((Vector2)enemy.transform.position - position).sqrMagnitude;

                if (sqrDistance > closestSqrDistance)
                    continue;

                closestEnemy = enemy;
                closestSqrDistance = sqrDistance;
            }

            return closestEnemy;
        }

        private EnemyHealth FindClosestEnemyAround(Vector2 position, float radius, EnemyHealth excludedEnemy)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(position, radius, areaResults, enemyLayerMask);
            EnemyHealth closestEnemy = null;
            float closestSqrDistance = radius * radius;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy == excludedEnemy || enemy.IsDead)
                    continue;

                float sqrDistance = ((Vector2)enemy.transform.position - position).sqrMagnitude;

                if (sqrDistance > closestSqrDistance)
                    continue;

                closestEnemy = enemy;
                closestSqrDistance = sqrDistance;
            }

            return closestEnemy;
        }

        private bool TryBlockWithKaelBlackIronBarrier(Vector2 hitDirection)
        {
            if (kaelBlackIronBarrierLevel <= 0 || kaelBlackIronBarrierTimer > 0f)
                return false;

            kaelBlackIronBarrierTimer = GetKaelBlackIronBarrierCooldown();
            kaelBlackIronBarrierReadySoundPlayed = false;
            GameSfx.Play(SfxType.KaelBlackIronBarrierBlock);
            CombatVFX.PlayBurst(GetShieldCenterPosition(), CombatVFXKind.Shockwave, 0.72f, 0.2f);
            return true;
        }

        private bool TryBlockWithKaelGuardianResolve(Vector2 hitDirection)
        {
            if (kaelGuardianResolveLevel <= 0)
                return false;

            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth == null || playerHealth.HealthProgress > kaelGuardianResolveHealthThreshold)
                return false;

            float chance = Mathf.Clamp01(kaelGuardianResolveBlockChancePerLevel * kaelGuardianResolveLevel);

            if (Random.value > chance)
                return false;

            GameSfx.Play(SfxType.KaelGuardiansResolve);
            CombatVFX.PlayBurst(GetShieldCenterPosition(), CombatVFXKind.Vampirism, 0.58f, 0.18f);
            return true;
        }

        private void UpdateSeleneNebulaZones()
        {
            if (seleneNebulaZones.Count == 0 || GameState.IsGameOver)
                return;

            if (Time.timeScale <= 0f)
                return;

            for (int i = seleneNebulaZones.Count - 1; i >= 0; i--)
            {
                SeleneNebulaZone zone = seleneNebulaZones[i];
                zone.RemainingTime -= Time.deltaTime;
                zone.TickTimer -= Time.deltaTime;

                if (zone.RemainingTime <= 0f)
                {
                    RemoveSeleneNebulaZoneAt(i);
                    continue;
                }

                if (zone.TickTimer > 0f)
                    continue;

                zone.TickTimer = seleneNebulaZoneTickInterval;
                ApplyAreaDamage(zone.Position, zone.Radius, zone.Damage, null);

                if (seleneShadowStepLevel >= 3)
                    ApplyAreaSlow(zone.Position, zone.Radius, 0.88f, seleneNebulaZoneTickInterval + 0.1f);
            }
        }

        private void RemoveSeleneNebulaZoneAt(int index)
        {
            if (index < 0 || index >= seleneNebulaZones.Count)
                return;

            GameObject visual = seleneNebulaZones[index].Visual;

            if (visual != null)
                Destroy(visual);

            seleneNebulaZones.RemoveAt(index);
        }

        private void UpdateSeleneFullMoon()
        {
            if (seleneSilentBladeLevel <= 0 || GameState.IsGameOver)
                return;

            if (Time.timeScale <= 0f)
                return;

            seleneFullMoonTimer -= Time.deltaTime;

            if (seleneFullMoonTimer > 0f)
                return;

            seleneFullMoonTimer = GetSeleneFullMoonCooldown();
            TriggerSeleneFullMoon();
        }

        private void TriggerSeleneFullMoon()
        {
            EnemyHealth target = FindClosestEnemyAround(GetEffectCenterPosition(), 24f, null);

            if (target == null)
                return;

            Vector2 position = target.transform.position;
            float damage = Mathf.Max(0.1f, seleneFullMoonDamageRatio * Mathf.Max(1, seleneSilentBladeLevel));
            float radius = GetSeleneFullMoonRadius();
            StartCoroutine(ResolveSeleneFullMoonMeteor(position, damage, radius));
        }

        private IEnumerator ResolveSeleneFullMoonMeteor(Vector2 position, float damage, float radius)
        {
            GameSfx.Play(SfxType.SeleneFullmoonWarning);
            CombatVFX.PlayMoonMeteorWarning(position, radius, seleneFullMoonMeteorWarningDuration);
            CombatVFX.PlayMoonMeteorFall(position, radius, seleneFullMoonMeteorWarningDuration);

            float fallSfxDelay = Mathf.Min(0.12f, seleneFullMoonMeteorWarningDuration * 0.35f);
            yield return new WaitForSeconds(fallSfxDelay);

            if (GameState.IsGameOver)
                yield break;

            GameSfx.Play(SfxType.SeleneFullmoonFall);

            yield return new WaitForSeconds(Mathf.Max(0f, seleneFullMoonMeteorWarningDuration - fallSfxDelay));

            if (GameState.IsGameOver)
                yield break;

            GameSfx.Play(SfxType.SeleneFullmoonImpact);
            ApplyAreaDamage(position, radius, damage, null);

            if (seleneSilentBladeLevel >= 3)
                CreateSeleneNebulaZone(position, damage, false);

            CombatVFX.PlayMoonMeteorImpact(position, radius);

            yield return new WaitForSeconds(0.05f);

            if (!GameState.IsGameOver)
                GameSfx.Play(SfxType.SeleneFullmoonExplosion);
        }

        private float GetSeleneFullMoonCooldown()
        {
            if (seleneSilentBladeLevel <= 1)
                return seleneFullMoonBaseCooldown;

            return Mathf.Max(7f, seleneFullMoonLevelTwoCooldown - 0.45f * Mathf.Max(0, seleneSilentBladeLevel - 2));
        }

        private float GetSeleneFullMoonRadius()
        {
            return seleneFullMoonRadius + 0.15f * Mathf.Max(0, seleneSilentBladeLevel - 1);
        }

        private void ApplyAreaSlow(Vector2 center, float radius, float moveSpeedMultiplier, float duration)
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(center, radius, areaResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyStatusEffects statusEffects = hit.GetComponentInParent<EnemyStatusEffects>();

                if (statusEffects == null)
                {
                    EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                    if (enemy == null)
                        continue;

                    statusEffects = enemy.gameObject.AddComponent<EnemyStatusEffects>();
                }

                statusEffects.ApplySlow(moveSpeedMultiplier, duration);
            }
        }

        private void ApplyBoxSlow(Vector2 center, Vector2 size, float angle, float moveSpeedMultiplier, float duration)
        {
            int hitCount = Physics2D.OverlapBoxNonAlloc(center, size, angle, areaResults, enemyLayerMask);
            HashSet<EnemyStatusEffects> slowedEnemies = new HashSet<EnemyStatusEffects>();

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = areaResults[i];

                if (hit == null)
                    continue;

                EnemyStatusEffects statusEffects = hit.GetComponentInParent<EnemyStatusEffects>();

                if (statusEffects == null)
                {
                    EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                    if (enemy == null)
                        continue;

                    statusEffects = enemy.gameObject.AddComponent<EnemyStatusEffects>();
                }

                if (!slowedEnemies.Add(statusEffects))
                    continue;

                statusEffects.ApplySlow(moveSpeedMultiplier, duration);
            }
        }

        private float GetKaelBlackIronBarrierCooldown()
        {
            return Mathf.Max(5f, kaelBlackIronBarrierCooldown - kaelBlackIronBarrierCooldownReductionPerLevel * Mathf.Max(0, kaelBlackIronBarrierLevel - 1));
        }

        private void UpdateCharacterExclusiveTimers()
        {
            if (GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (kaelBlackIronBarrierTimer > 0f)
            {
                kaelBlackIronBarrierTimer -= Time.deltaTime;

                if (kaelBlackIronBarrierTimer <= 0f && kaelBlackIronBarrierLevel > 0 && !kaelBlackIronBarrierReadySoundPlayed)
                {
                    GameSfx.Play(SfxType.KaelBlackIronBarrierOn);
                    kaelBlackIronBarrierReadySoundPlayed = true;
                }
            }

            UpdateKaelBlackIronRegen();
        }

        private void UpdateKaelBlackIronRegen()
        {
            if (kaelBlackIronRegenLevel <= 0)
                return;

            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth == null || playerHealth.IsDead || playerHealth.CurrentHealth >= playerHealth.MaxHealth)
                return;

            kaelBlackIronRegenTimer -= Time.deltaTime;

            if (kaelBlackIronRegenTimer > 0f)
                return;

            kaelBlackIronRegenTimer = GetKaelBlackIronRegenInterval();
            int healAmount = Mathf.Max(1, Mathf.CeilToInt(playerHealth.MaxHealth * GetKaelBlackIronRegenRatio()));

            if (kaelBlackIronRegenLevel >= 5 && playerHealth.HealthProgress <= kaelBlackIronRegenLowHealthThreshold)
                healAmount *= 2;

            playerHealth.Heal(healAmount);
            GameSfx.Play(SfxType.KaelBlackIronRegen);
            CombatVFX.PlayBurst(GetEffectCenterPosition(), CombatVFXKind.Vampirism, 0.46f, 0.16f);
        }

        private float GetKaelBlackIronRegenInterval()
        {
            if (kaelBlackIronRegenLevel >= 4)
                return kaelBlackIronRegenLevelFourInterval;

            if (kaelBlackIronRegenLevel >= 2)
                return kaelBlackIronRegenLevelTwoInterval;

            return kaelBlackIronRegenBaseInterval;
        }

        private float GetKaelBlackIronRegenRatio()
        {
            if (kaelBlackIronRegenLevel >= 5)
                return kaelBlackIronRegenBaseRatio * 3f;

            if (kaelBlackIronRegenLevel >= 3)
                return kaelBlackIronRegenBaseRatio * 2f;

            return kaelBlackIronRegenBaseRatio;
        }

        private void UpdateHanSeorinBleeds()
        {
            if (hanSeorinBleeds.Count == 0 || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            for (int i = hanSeorinBleeds.Count - 1; i >= 0; i--)
            {
                HanSeorinBleed bleed = hanSeorinBleeds[i];

                if (bleed.Enemy == null || bleed.Enemy.IsDead)
                {
                    hanSeorinBleeds.RemoveAt(i);
                    continue;
                }

                bleed.RemainingTime -= Time.deltaTime;
                bleed.TickTimer -= Time.deltaTime;

                if (bleed.RemainingTime <= 0f)
                {
                    hanSeorinBleeds.RemoveAt(i);
                    continue;
                }

                if (bleed.TickTimer > 0f)
                    continue;

                bleed.TickTimer = hanSeorinBloodFangTickInterval;
                bleed.Enemy.TakeDamage(bleed.DamagePerTick);
                CombatVFX.PlayBurst(bleed.Enemy.transform.position, CombatVFXKind.Vampirism, 0.18f, 0.08f);
            }
        }

        private void StartShadowStepCollisionPhase(float duration)
        {
            if (duration <= 0f)
                return;

            int playerLayer = gameObject.layer;
            int enemyLayer = LayerMask.NameToLayer(seleneShadowStepEnemyLayerName);

            if (enemyLayer < 0)
                return;

            if (!shadowStepCollisionPhaseActive)
            {
                shadowStepPlayerLayer = playerLayer;
                shadowStepEnemyLayer = enemyLayer;
                shadowStepStoredCollisionIgnored = Physics2D.GetIgnoreLayerCollision(playerLayer, enemyLayer);
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
                shadowStepCollisionPhaseActive = true;
            }

            shadowStepCollisionPhaseTimer = Mathf.Max(shadowStepCollisionPhaseTimer, duration);
        }

        private void UpdateShadowStepCollisionPhase()
        {
            if (!shadowStepCollisionPhaseActive)
                return;

            if (GameState.IsGameOver)
            {
                EndShadowStepCollisionPhase();
                return;
            }

            if (Time.timeScale <= 0f)
                return;

            shadowStepCollisionPhaseTimer -= Time.deltaTime;

            if (shadowStepCollisionPhaseTimer <= 0f)
                EndShadowStepCollisionPhase();
        }

        private void EndShadowStepCollisionPhase()
        {
            if (!shadowStepCollisionPhaseActive)
                return;

            if (shadowStepPlayerLayer >= 0 && shadowStepEnemyLayer >= 0)
                Physics2D.IgnoreLayerCollision(shadowStepPlayerLayer, shadowStepEnemyLayer, shadowStepStoredCollisionIgnored);

            shadowStepCollisionPhaseActive = false;
            shadowStepCollisionPhaseTimer = 0f;
            shadowStepPlayerLayer = -1;
            shadowStepEnemyLayer = -1;
        }

        private static bool IsIgnoredProjectileReflectEnemy(EnemyHealth enemy, IReadOnlyCollection<EnemyHealth> ignoredEnemies)
        {
            if (ignoredEnemies == null)
                return false;

            foreach (EnemyHealth ignoredEnemy in ignoredEnemies)
            {
                if (ignoredEnemy == enemy)
                    return true;
            }

            return false;
        }

        private void UpdateShieldRecharge()
        {
            if (shieldLevel <= 0 || shieldReady || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            shieldTimer -= Time.deltaTime;

            if (shieldTimer > 0f)
                return;

            shieldReady = true;
            GameSfx.Play(SfxType.ShieldReady);
            CreateShieldVfx();
        }

        private float GetShieldRechargeDuration()
        {
            return Mathf.Max(5f, shieldRechargeTime - shieldRechargeReductionPerLevel * Mathf.Max(0, shieldLevel - 1));
        }

        private void UpdateEclipseAura()
        {
            if (eclipseAuraLevel <= 0 || GameState.IsGameOver)
            {
                RemoveEclipseAuraVfx();
                return;
            }

            RefreshEclipseAuraVfx();

            if (Time.timeScale <= 0f)
                return;

            eclipseAuraTimer -= Time.deltaTime;

            if (eclipseAuraTimer > 0f)
                return;

            eclipseAuraTimer = GetEclipseAuraDamageInterval();
            Vector2 center = GetEffectCenterPosition();
            ApplyAreaDamage(center, GetEclipseAuraRadius(), GetEclipseAuraDamage(), null);

            if (eclipseAuraVfx != null)
                eclipseAuraVfx.PlayDamagePulse();
        }

        private float GetEclipseAuraRadius()
        {
            return eclipseAuraBaseRadius + eclipseAuraRadiusPerLevel * Mathf.Max(0, eclipseAuraLevel - 1);
        }

        private int GetEclipseAuraDamage()
        {
            return eclipseAuraBaseDamage + eclipseAuraDamagePerLevel * Mathf.Max(0, eclipseAuraLevel - 1);
        }

        private float GetEclipseAuraDamageInterval()
        {
            return Mathf.Max(0.4f, eclipseAuraDamageInterval - eclipseAuraIntervalReductionPerLevel * Mathf.Max(0, eclipseAuraLevel - 1));
        }

        private void RefreshEclipseAuraVfx()
        {
            if (eclipseAuraVfx == null)
            {
                GameObject auraObject = new GameObject("Eclipse Aura VFX");
                eclipseAuraVfx = auraObject.AddComponent<EclipseAuraVFX>();
                eclipseAuraVfx.Initialize(GetEffectCenter(), GetEclipseAuraRadius(), eclipseAuraColor);
                return;
            }

            eclipseAuraVfx.SetRadius(GetEclipseAuraRadius());
        }

        private void RefreshOrbitingBlades()
        {
            int desiredCount = Mathf.Clamp(orbitingBladeLevel, 1, 6);

            while (orbitingBlades.Count < desiredCount)
            {
                GameObject bladeObject = new GameObject("Orbiting Blade");
                bladeObject.transform.SetParent(transform);
                OrbitingBlade blade = bladeObject.AddComponent<OrbitingBlade>();
                orbitingBlades.Add(blade);
            }

            for (int i = 0; i < orbitingBlades.Count; i++)
            {
                if (orbitingBlades[i] == null)
                    continue;

                orbitingBlades[i].Configure(
                    GetOrbitCenter(),
                    orbitRadius,
                    120f + orbitingBladeLevel * 35f,
                    i * (360f / orbitingBlades.Count),
                    orbitBladeDamage + Mathf.Max(0, orbitingBladeLevel - 1),
                    orbitDamageInterval,
                    enemyLayerMask);
            }
        }

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        private void UpdateShieldAura()
        {
            if (shieldLevel <= 0 || !shieldReady || GameState.IsGameOver)
            {
                RemoveShieldVfx();
                return;
            }

            if (shieldVfx == null)
                CreateShieldVfx();

            if (shieldVfx != null)
                shieldVfx.SetShieldRatio(1f);
        }

        private void CreateShieldVfx()
        {
            if (shieldVfx != null)
                return;

            ShieldVFXController prefab = shieldVfxPrefab;

            if (prefab == null)
                prefab = Resources.Load<ShieldVFXController>("VFX/ShieldVFX");

            if (prefab != null)
                shieldVfx = Instantiate(prefab, GetShieldCenterPosition(), Quaternion.identity);
            else
                shieldVfx = new GameObject("ShieldVFX").AddComponent<ShieldVFXController>();

            shieldVfx.Initialize(GetShieldCenter());
            shieldVfx.SetShieldRatio(1f);
        }

        private Transform GetEffectCenter()
        {
            if (effectAnchors == null)
                effectAnchors = GetComponent<PlayerEffectAnchors>();

            return effectAnchors == null || effectAnchors.EffectCenter == null ? transform : effectAnchors.EffectCenter;
        }

        private Transform GetShieldCenter()
        {
            if (effectAnchors == null)
                effectAnchors = GetComponent<PlayerEffectAnchors>();

            return effectAnchors == null || effectAnchors.ShieldCenter == null ? transform : effectAnchors.ShieldCenter;
        }

        private Transform GetOrbitCenter()
        {
            if (effectAnchors == null)
                effectAnchors = GetComponent<PlayerEffectAnchors>();

            return effectAnchors == null || effectAnchors.OrbitCenter == null ? transform : effectAnchors.OrbitCenter;
        }

        private Vector3 GetEffectCenterPosition()
        {
            return effectAnchors == null ? transform.position : effectAnchors.EffectCenterPosition;
        }

        private Vector3 GetShieldCenterPosition()
        {
            return effectAnchors == null ? transform.position : effectAnchors.ShieldCenterPosition;
        }

        private void RemoveShieldVfx()
        {
            if (shieldVfx == null)
                return;

            GameSfx.Play(SfxType.ShieldBreak);
            shieldVfx.PlayBreak();
            shieldVfx = null;
        }

        private void RemoveEclipseAuraVfx()
        {
            if (eclipseAuraVfx == null)
                return;

            eclipseAuraVfx.StopAura();
            eclipseAuraVfx = null;
        }

        private void BreakShieldVfx(Vector2 hitDirection)
        {
            if (shieldVfx == null)
                CreateShieldVfx();

            if (shieldVfx != null)
            {
                shieldVfx.PlayHit(hitDirection);
                GameSfx.Play(SfxType.ShieldBreak);
                shieldVfx.PlayBreak();
            }

            shieldVfx = null;
        }

        private static void CreatePulseEffect(Vector2 position, float radius, Color color, float duration)
        {
            CreatePulseEffect(position, radius, color, duration, SpecialUpgradePulse.GetCircleSprite(), 0f);
        }

        private static void CreatePulseEffect(Vector2 position, float radius, Color color, float duration, Sprite sprite, float rotateSpeed)
        {
            GameObject effect = new GameObject("Special Upgrade Pulse");
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * radius * 2f;

            SpriteRenderer renderer = effect.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 14;

            SpecialUpgradePulse pulse = effect.AddComponent<SpecialUpgradePulse>();
            pulse.Play(duration, rotateSpeed);
        }

        private static void CreateLineEffect(Vector2 from, Vector2 to, Color color, float duration, float width)
        {
            Vector2 middle = (from + to) * 0.5f;
            Vector2 direction = to - from;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            GameObject effect = new GameObject("Chain Ricochet Line");
            effect.transform.position = middle;
            effect.transform.right = direction.normalized;
            effect.transform.localScale = new Vector3(direction.magnitude, width, 1f);

            SpriteRenderer renderer = effect.AddComponent<SpriteRenderer>();
            renderer.sprite = SpecialUpgradePulse.GetSquareSprite();
            renderer.color = color;
            renderer.sortingOrder = 15;

            SpecialUpgradePulse pulse = effect.AddComponent<SpecialUpgradePulse>();
            pulse.Play(duration);
        }
    }
}
