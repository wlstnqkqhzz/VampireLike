using System.Collections.Generic;
using UnityEngine;
using VampireLike.Enemies;

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
        private float chainRicochetRadius = 2.4f;

        [SerializeField]
        private float chainRicochetDamageRatio = 0.45f;

        [SerializeField]
        private float shieldRechargeTime = 22f;

        [SerializeField]
        private float shieldRechargeReductionPerLevel = 2f;

        [SerializeField]
        private float orbitRadius = 0.95f;

        [SerializeField]
        private float orbitDamageInterval = 0.55f;

        [SerializeField]
        private int orbitBladeDamage = 1;

        private readonly Collider2D[] areaResults = new Collider2D[64];
        private readonly List<OrbitingBlade> orbitingBlades = new List<OrbitingBlade>();
        private int explosiveShotLevel;
        private int frostShotLevel;
        private int vampirismLevel;
        private int shockwaveLevel;
        private int scatterShotLevel;
        private int shieldLevel;
        private int orbitingBladeLevel;
        private int chainRicochetLevel;
        private int projectileHitCount;
        private float shieldTimer;
        private bool shieldReady;
        private PlayerHealth playerHealth;
        private SpecialUpgradeAura shieldAura;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
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
            chainRicochetRadius = Mathf.Max(0.1f, chainRicochetRadius);
            chainRicochetDamageRatio = Mathf.Max(0.1f, chainRicochetDamageRatio);
            shieldRechargeTime = Mathf.Max(1f, shieldRechargeTime);
            shieldRechargeReductionPerLevel = Mathf.Max(0f, shieldRechargeReductionPerLevel);
            orbitRadius = Mathf.Max(0.2f, orbitRadius);
            orbitDamageInterval = Mathf.Max(0.05f, orbitDamageInterval);
            orbitBladeDamage = Mathf.Max(1, orbitBladeDamage);
        }

        private void Update()
        {
            UpdateShieldRecharge();
            UpdateShieldAura();
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

        public Vector2[] GetProjectileDirections(Vector2 baseDirection)
        {
            if (scatterShotLevel <= 0)
                return new[] { baseDirection };

            int sideCount = Mathf.Clamp(scatterShotLevel, 1, 3);
            int directionCount = sideCount * 2 + 1;
            Vector2[] directions = new Vector2[directionCount];
            int index = 0;

            for (int i = -sideCount; i <= sideCount; i++)
            {
                float angle = i * scatterAnglePerLevel;
                directions[index] = Rotate(baseDirection, angle).normalized;
                index++;
            }

            return directions;
        }

        public bool TryBlockDamage()
        {
            if (shieldLevel <= 0 || !shieldReady)
                return false;

            shieldReady = false;
            shieldTimer = GetShieldRechargeDuration();
            RemoveShieldAura();
            CreatePulseEffect(transform.position, 0.85f, new Color(0.35f, 0.82f, 1f, 0.9f), 0.28f, SpecialUpgradePulse.GetDiamondSprite(), 180f);
            CreatePulseEffect(transform.position, 1.05f, new Color(0.45f, 0.95f, 1f, 0.55f), 0.34f, SpecialUpgradePulse.GetCircleSprite(), -120f);
            return true;
        }

        public void HandleProjectileHit(EnemyHealth enemy, int projectileDamage, Vector2 hitPosition)
        {
            if (enemy == null || projectileDamage <= 0)
                return;

            if (frostShotLevel > 0 && !enemy.IsDead)
                ApplyFrost(enemy);

            if (shockwaveLevel > 0)
                CountShockwaveHit(projectileDamage, hitPosition);

            if (chainRicochetLevel > 0)
                TriggerChainRicochet(enemy, projectileDamage, hitPosition);
        }

        public void HandleProjectileKill(EnemyHealth killedEnemy, int projectileDamage, Vector2 killPosition)
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
            CreatePulseEffect(enemy.transform.position, 0.42f, new Color(0.55f, 0.9f, 1f, 0.78f), 0.2f, SpecialUpgradePulse.GetDiamondSprite(), 220f);
        }

        private void CountShockwaveHit(int projectileDamage, Vector2 hitPosition)
        {
            projectileHitCount++;

            if (projectileHitCount < GetShockwaveHitInterval())
                return;

            projectileHitCount = 0;
            int damage = Mathf.Max(1, Mathf.RoundToInt(projectileDamage * shockwaveDamageMultiplier));
            ApplyAreaDamage(hitPosition, shockwaveRadius, damage, null);
            CreatePulseEffect(hitPosition, shockwaveRadius, shockwaveColor, 0.34f, SpecialUpgradePulse.GetCircleSprite(), 120f);
            CreatePulseEffect(hitPosition, shockwaveRadius * 0.58f, new Color(0.75f, 1f, 1f, 0.32f), 0.24f, SpecialUpgradePulse.GetFilledCircleSprite(), 0f);
        }

        private int GetShockwaveHitInterval()
        {
            return Mathf.Max(3, shockwaveBaseHitInterval - Mathf.Max(0, shockwaveLevel - 1));
        }

        private void TriggerExplosion(EnemyHealth killedEnemy, int projectileDamage, Vector2 killPosition)
        {
            int damage = Mathf.Max(1, Mathf.RoundToInt(projectileDamage * explosionDamageRatioPerLevel * explosiveShotLevel));
            ApplyAreaDamage(killPosition, explosionRadius, damage, killedEnemy);
            CreatePulseEffect(killPosition, explosionRadius, explosionColor, 0.26f, SpecialUpgradePulse.GetStarSprite(), 260f);
            CreatePulseEffect(killPosition, explosionRadius * 0.7f, new Color(1f, 0.85f, 0.18f, 0.45f), 0.2f, SpecialUpgradePulse.GetFilledCircleSprite(), 0f);
        }

        private void ApplyAreaDamage(Vector2 center, float radius, int damage, EnemyHealth excludedEnemy)
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
                CreatePulseEffect(transform.position, 0.5f, new Color(0.25f, 1f, 0.45f, 0.72f), 0.24f, SpecialUpgradePulse.GetDiamondSprite(), -180f);
            }
        }

        private void TriggerChainRicochet(EnemyHealth firstEnemy, int projectileDamage, Vector2 startPosition)
        {
            EnemyHealth currentEnemy = firstEnemy;
            Vector2 currentPosition = startPosition;
            int damage = Mathf.Max(1, Mathf.RoundToInt(projectileDamage * chainRicochetDamageRatio));
            int maxChains = Mathf.Clamp(chainRicochetLevel, 1, 3);

            for (int i = 0; i < maxChains; i++)
            {
                EnemyHealth nextEnemy = FindClosestChainTarget(currentPosition, currentEnemy);

                if (nextEnemy == null)
                    return;

                CreateLineEffect(currentPosition, nextEnemy.transform.position, new Color(0.65f, 0.9f, 1f, 0.92f), 0.16f, 0.1f);
                CreatePulseEffect(nextEnemy.transform.position, 0.36f, new Color(0.7f, 0.95f, 1f, 0.78f), 0.16f, SpecialUpgradePulse.GetStarSprite(), 240f);
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

        private void UpdateShieldRecharge()
        {
            if (shieldLevel <= 0 || shieldReady || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            shieldTimer -= Time.deltaTime;

            if (shieldTimer > 0f)
                return;

            shieldReady = true;
            CreatePulseEffect(transform.position, 0.75f, new Color(0.35f, 0.75f, 1f, 0.65f), 0.24f, SpecialUpgradePulse.GetCircleSprite(), 150f);
            CreateShieldAura();
        }

        private float GetShieldRechargeDuration()
        {
            return Mathf.Max(5f, shieldRechargeTime - shieldRechargeReductionPerLevel * Mathf.Max(0, shieldLevel - 1));
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
                    transform,
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
                RemoveShieldAura();
                return;
            }

            if (shieldAura == null)
                CreateShieldAura();
        }

        private void CreateShieldAura()
        {
            if (shieldAura != null)
                return;

            GameObject auraObject = new GameObject("Shield Aura");
            auraObject.transform.position = transform.position;
            shieldAura = auraObject.AddComponent<SpecialUpgradeAura>();
            shieldAura.Initialize(transform, SpecialUpgradePulse.GetCircleSprite(), new Color(0.38f, 0.82f, 1f, 0.62f), 1.35f, 120f, 5.5f, 0.08f);
        }

        private void RemoveShieldAura()
        {
            if (shieldAura == null)
                return;

            Destroy(shieldAura.gameObject);
            shieldAura = null;
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
