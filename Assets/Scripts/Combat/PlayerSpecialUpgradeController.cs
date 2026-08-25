using System.Collections.Generic;
using UnityEngine;
using VampireLike.Enemies;
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

        [Header("Character Exclusive - Selene")]
        [SerializeField]
        private float seleneMoonShadowCloneChancePerLevel = 0.16f;

        [SerializeField]
        private float seleneShadowStepBonusInvinciblePerLevel = 0.24f;

        [SerializeField]
        private string seleneShadowStepEnemyLayerName = "Enemy";

        [SerializeField]
        private int seleneTwinMoonFlurryBaseInterval = 7;

        [SerializeField]
        private float seleneTwinMoonFlurryAngle = 10f;

        [SerializeField]
        private int seleneMoonlightMarkRequiredStacks = 3;

        [SerializeField]
        private float seleneMoonlightMarkDamageRatioPerLevel = 0.38f;

        [SerializeField]
        private float seleneSilentBladeChancePerLevel = 0.14f;

        [SerializeField]
        private float seleneSilentBladeRadius = 2.8f;

        [SerializeField]
        private float seleneSilentBladeDamageRatio = 0.48f;

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

        private readonly Collider2D[] areaResults = new Collider2D[64];
        private readonly List<OrbitingBlade> orbitingBlades = new List<OrbitingBlade>();
        private readonly Dictionary<EnemyHealth, int> moonlightMarkStacks = new Dictionary<EnemyHealth, int>();
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
        private int seleneMoonShadowCloneLevel;
        private int seleneShadowStepLevel;
        private int seleneTwinMoonFlurryLevel;
        private int seleneMoonlightMarkLevel;
        private int seleneSilentBladeLevel;
        private int hanSeorinBloodMarkLevel;
        private int hanSeorinShadowDaggerLevel;
        private int hanSeorinReturningBladeLevel;
        private int hanSeorinKillingIntentLevel;
        private int hanSeorinRedExecutionLevel;
        private int projectileHitCount;
        private int kaelManaSlashHitCount;
        private int seleneTwinMoonFlurryAttackCount;
        private EnemyHealth hanSeorinKillingIntentTarget;
        private int hanSeorinKillingIntentStacks;
        private float shieldTimer;
        private float eclipseAuraTimer;
        private float kaelBlackIronBarrierTimer;
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
            seleneMoonShadowCloneChancePerLevel = Mathf.Clamp01(seleneMoonShadowCloneChancePerLevel);
            seleneShadowStepBonusInvinciblePerLevel = Mathf.Max(0f, seleneShadowStepBonusInvinciblePerLevel);
            seleneTwinMoonFlurryBaseInterval = Mathf.Max(1, seleneTwinMoonFlurryBaseInterval);
            seleneTwinMoonFlurryAngle = Mathf.Clamp(seleneTwinMoonFlurryAngle, 1f, 35f);
            seleneMoonlightMarkRequiredStacks = Mathf.Max(2, seleneMoonlightMarkRequiredStacks);
            seleneMoonlightMarkDamageRatioPerLevel = Mathf.Max(0.01f, seleneMoonlightMarkDamageRatioPerLevel);
            seleneSilentBladeChancePerLevel = Mathf.Clamp01(seleneSilentBladeChancePerLevel);
            seleneSilentBladeRadius = Mathf.Max(0.3f, seleneSilentBladeRadius);
            seleneSilentBladeDamageRatio = Mathf.Max(0.1f, seleneSilentBladeDamageRatio);
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
        }

        private void Update()
        {
            UpdateShieldRecharge();
            UpdateShieldAura();
            UpdateEclipseAura();
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
        }

        public void AddKaelExecutionBladeLevel()
        {
            kaelExecutionBladeLevel++;
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

        public int GetProjectileReflectCount()
        {
            return 0;
        }

        public Vector2[] GetProjectileDirections(Vector2 baseDirection)
        {
            List<Vector2> directions = new List<Vector2>();

            if (scatterShotLevel <= 0)
            {
                directions.Add(baseDirection);
            }
            else
            {
                int sideCount = Mathf.Clamp(scatterShotLevel, 1, scatterMaxSideCount);

                for (int i = -sideCount; i <= sideCount; i++)
                {
                    float angle = i * scatterAnglePerLevel;
                    directions.Add(Rotate(baseDirection, angle).normalized);
                }
            }

            if (seleneTwinMoonFlurryLevel > 0 && ShouldTriggerSeleneTwinMoonFlurry())
            {
                directions.Add(Rotate(baseDirection, -seleneTwinMoonFlurryAngle).normalized);
                directions.Add(Rotate(baseDirection, seleneTwinMoonFlurryAngle).normalized);
                GameSfx.Play(SfxType.SkillScatter);
                CombatVFX.PlayBurst(GetEffectCenterPosition(), CombatVFXKind.Ricochet, 0.42f, 0.16f);
            }

            if (seleneMoonShadowCloneLevel > 0 && Random.value < seleneMoonShadowCloneChancePerLevel * seleneMoonShadowCloneLevel)
            {
                directions.Add(Rotate(baseDirection, Random.Range(-5f, 5f)).normalized);
                CombatVFX.PlayChainLightningImpact(GetEffectCenterPosition(), 0.16f, 0.1f);
            }

            if (hanSeorinShadowDaggerLevel > 0 && Random.value < GetHanSeorinShadowDaggerChance())
            {
                directions.Add(Rotate(baseDirection, Random.Range(-hanSeorinShadowDaggerAngle, hanSeorinShadowDaggerAngle)).normalized);
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
            return hanSeorinReturningBladeLevel >= 3 ? 1 : 0;
        }

        public float GetHanSeorinReturningBladeDamageMultiplier()
        {
            return hanSeorinReturningBladeLevel >= 2 ? hanSeorinReturningBladeLevelTwoDamageMultiplier : 1f;
        }

        public float GetProjectileDamageMultiplierForEnemy(EnemyHealth enemy)
        {
            float multiplier = 1f;

            if (enemy != null && hanSeorinKillingIntentLevel > 0 && enemy == hanSeorinKillingIntentTarget)
                multiplier *= 1f + Mathf.Min(GetHanSeorinKillingIntentMaxBonus(), hanSeorinKillingIntentStacks * hanSeorinKillingIntentBonusPerHit);

            if (enemy != null && hanSeorinRedExecutionLevel > 0 && enemy.HealthProgress <= hanSeorinRedExecutionHealthThreshold)
                multiplier *= 1f + hanSeorinRedExecutionBonusPerLevel * hanSeorinRedExecutionLevel;

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
            return seleneShadowStepBonusInvinciblePerLevel * Mathf.Max(0, seleneShadowStepLevel);
        }

        public void NotifyPlayerDamaged()
        {
            if (seleneShadowStepLevel <= 0)
                return;

            CombatVFX.PlayBurst(GetEffectCenterPosition(), CombatVFXKind.Ricochet, 0.58f, 0.2f);
            StartShadowStepCollisionPhase(GetBonusInvincibleDuration());
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

            if (kaelBlackSwordWaveLevel > 0)
                TriggerKaelBlackSwordWave(enemy, projectileDamage, hitPosition);

            if (kaelManaSlashLevel > 0)
                CountKaelManaSlash(projectileDamage, hitPosition);

            if (kaelExecutionBladeLevel > 0 && !enemy.IsDead)
                TryKaelExecutionBlade(enemy, projectileDamage);

            if (seleneMoonlightMarkLevel > 0 && !enemy.IsDead)
                ApplySeleneMoonlightMark(enemy, projectileDamage);

            if (seleneSilentBladeLevel > 0)
                TrySeleneSilentBlade(enemy, projectileDamage, hitPosition);

            if (hanSeorinBloodMarkLevel > 0 && !enemy.IsDead)
                ApplyHanSeorinBloodMark(enemy, projectileDamage);

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

        private void ApplyAreaDamage(Vector2 center, float radius, float damage, EnemyHealth excludedEnemy)
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
            }
        }

        private void TriggerKaelBlackSwordWave(EnemyHealth hitEnemy, float projectileDamage, Vector2 hitPosition)
        {
            float radius = kaelBlackSwordWaveRadius + 0.12f * Mathf.Max(0, kaelBlackSwordWaveLevel - 1);
            float damage = projectileDamage * kaelBlackSwordWaveDamageRatio * kaelBlackSwordWaveLevel;
            ApplyAreaDamage(hitPosition, radius, damage, hitEnemy);
            CombatVFX.PlayBurst(hitPosition, CombatVFXKind.ArcaneImpact, radius, 0.2f);
        }

        private void CountKaelManaSlash(float projectileDamage, Vector2 hitPosition)
        {
            kaelManaSlashHitCount++;

            if (kaelManaSlashHitCount < GetKaelManaSlashInterval())
                return;

            kaelManaSlashHitCount = 0;
            ApplyAreaDamage(hitPosition, kaelManaSlashRadius, projectileDamage * kaelManaSlashDamageRatio, null);
            GameSfx.Play(SfxType.SkillShockwave);
            CombatVFX.PlayBurst(hitPosition, CombatVFXKind.Shockwave, kaelManaSlashRadius, 0.28f);
        }

        private int GetKaelManaSlashInterval()
        {
            return Mathf.Max(3, kaelManaSlashBaseHitInterval - Mathf.Max(0, kaelManaSlashLevel - 1));
        }

        private void TryKaelExecutionBlade(EnemyHealth enemy, float projectileDamage)
        {
            float threshold = Mathf.Clamp01(kaelExecutionHealthThreshold + 0.04f * Mathf.Max(0, kaelExecutionBladeLevel - 1));

            if (enemy.HealthProgress > threshold)
                return;

            float damage = projectileDamage * kaelExecutionDamageRatioPerLevel * kaelExecutionBladeLevel;
            enemy.TakeDamage(damage);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Explosion, 0.36f, 0.14f);
        }

        private void ApplySeleneMoonlightMark(EnemyHealth enemy, float projectileDamage)
        {
            if (!moonlightMarkStacks.TryGetValue(enemy, out int stacks))
                stacks = 0;

            stacks++;

            if (stacks < seleneMoonlightMarkRequiredStacks)
            {
                moonlightMarkStacks[enemy] = stacks;
                CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Ricochet, 0.28f, 0.1f);
                return;
            }

            moonlightMarkStacks.Remove(enemy);
            float damage = projectileDamage * seleneMoonlightMarkDamageRatioPerLevel * seleneMoonlightMarkLevel;
            enemy.TakeDamage(damage);
            GameSfx.Play(SfxType.SkillRicochet);
            CombatVFX.PlayChainLightningImpact(enemy.transform.position, 0.28f, 0.12f);
        }

        private void TrySeleneSilentBlade(EnemyHealth firstEnemy, float projectileDamage, Vector2 startPosition)
        {
            float chance = seleneSilentBladeChancePerLevel * seleneSilentBladeLevel;

            if (Random.value > chance)
                return;

            EnemyHealth target = FindClosestEnemyAround(GetEffectCenterPosition(), seleneSilentBladeRadius, firstEnemy);

            if (target == null)
                return;

            float damage = projectileDamage * seleneSilentBladeDamageRatio;
            target.TakeDamage(damage);
            GameSfx.Play(SfxType.SeleneDaggerThrow);
            CombatVFX.PlayLine(startPosition, target.transform.position, CombatVFXKind.Ricochet, 0.12f, 0.06f);
            CombatVFX.PlayBurst(target.transform.position, CombatVFXKind.Ricochet, 0.34f, 0.12f);
        }

        private void ApplyHanSeorinBloodMark(EnemyHealth enemy, float projectileDamage)
        {
            if (!hanSeorinBloodMarkStacks.TryGetValue(enemy, out int stacks))
                stacks = 0;

            stacks++;

            if (stacks < GetHanSeorinBloodMarkRequiredStacks())
            {
                hanSeorinBloodMarkStacks[enemy] = stacks;
                CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Vampirism, 0.22f, 0.08f);
                return;
            }

            hanSeorinBloodMarkStacks.Remove(enemy);
            float damage = projectileDamage * GetHanSeorinBloodMarkDamageRatio();
            enemy.TakeDamage(damage);

            if (hanSeorinBloodMarkLevel >= 3)
                ApplyAreaDamage(enemy.transform.position, hanSeorinBloodMarkSplashRadius, damage * hanSeorinBloodMarkSplashDamageRatio, enemy);

            GameSfx.Play(SfxType.SkillExplosion);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Explosion, hanSeorinBloodMarkLevel >= 3 ? hanSeorinBloodMarkSplashRadius : 0.42f, 0.18f);
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
                hanSeorinKillingIntentTarget = enemy;
                hanSeorinKillingIntentStacks = 1;
            }
        }

        private void TryHanSeorinRedExecution(EnemyHealth enemy)
        {
            if (enemy == null || enemy.IsBoss || hanSeorinRedExecutionLevel < 3)
                return;

            if (enemy.HealthProgress > hanSeorinRedExecutionInstantKillThreshold)
                return;

            enemy.TakeDamage(enemy.MaxHealth);
            GameSfx.Play(SfxType.SkillVampirism);
            CombatVFX.PlayBurst(enemy.transform.position, CombatVFXKind.Vampirism, 0.52f, 0.16f);
        }

        private int GetHanSeorinBloodMarkRequiredStacks()
        {
            return hanSeorinBloodMarkLevel >= 2
                ? Mathf.Max(2, hanSeorinBloodMarkBaseRequiredStacks - 1)
                : hanSeorinBloodMarkBaseRequiredStacks;
        }

        private float GetHanSeorinBloodMarkDamageRatio()
        {
            return hanSeorinBloodMarkLevel >= 3 ? hanSeorinBloodMarkLevelThreeDamageRatio : hanSeorinBloodMarkDamageRatio;
        }

        private float GetHanSeorinShadowDaggerChance()
        {
            if (hanSeorinShadowDaggerLevel >= 3)
                return hanSeorinShadowDaggerChanceLevelThree;

            if (hanSeorinShadowDaggerLevel == 2)
                return hanSeorinShadowDaggerChanceLevelTwo;

            return hanSeorinShadowDaggerChanceLevelOne;
        }

        private float GetHanSeorinKillingIntentMaxBonus()
        {
            return 0.15f * Mathf.Clamp(hanSeorinKillingIntentLevel, 1, 3);
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
            int maxChains = Mathf.Clamp(chainRicochetLevel, 1, 3);

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

        private bool ShouldTriggerSeleneTwinMoonFlurry()
        {
            seleneTwinMoonFlurryAttackCount++;
            int interval = Mathf.Max(3, seleneTwinMoonFlurryBaseInterval - Mathf.Max(0, seleneTwinMoonFlurryLevel - 1));

            if (seleneTwinMoonFlurryAttackCount < interval)
                return false;

            seleneTwinMoonFlurryAttackCount = 0;
            return true;
        }

        private bool TryBlockWithKaelBlackIronBarrier(Vector2 hitDirection)
        {
            if (kaelBlackIronBarrierLevel <= 0 || kaelBlackIronBarrierTimer > 0f)
                return false;

            kaelBlackIronBarrierTimer = GetKaelBlackIronBarrierCooldown();
            GameSfx.Play(SfxType.ShieldBlock);
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

            GameSfx.Play(SfxType.ShieldBlock);
            CombatVFX.PlayBurst(GetShieldCenterPosition(), CombatVFXKind.Vampirism, 0.58f, 0.18f);
            return true;
        }

        private float GetKaelBlackIronBarrierCooldown()
        {
            return Mathf.Max(5f, kaelBlackIronBarrierCooldown - kaelBlackIronBarrierCooldownReductionPerLevel * Mathf.Max(0, kaelBlackIronBarrierLevel - 1));
        }

        private void UpdateCharacterExclusiveTimers()
        {
            if (kaelBlackIronBarrierTimer <= 0f || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            kaelBlackIronBarrierTimer -= Time.deltaTime;
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
            int desiredCount = Mathf.Clamp(orbitingBladeLevel, 1, 3);

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
