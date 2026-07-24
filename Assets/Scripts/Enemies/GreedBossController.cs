using System;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 히든 보스 Greed Lord의 탐욕 게이지와 흡수 경험치 기반 능력치 성장을 관리한다.
    /// 체력 페이즈는 BossController가 담당하고, 탐욕 단계는 흡수한 경험치 총량으로 별도 계산한다.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class GreedBossController : MonoBehaviour
    {
        [SerializeField]
        private int[] greedLevelThresholds = { 0, 100, 300, 600 };

        [SerializeField]
        private float[] moveSpeedMultipliers = { 1f, 1.1f, 1.2f, 1.3f };

        [SerializeField]
        private float[] contactDamageMultipliers = { 1f, 1.1f, 1.2f, 1.3f };

        [SerializeField]
        private float[] patternCooldownMultipliers = { 1f, 0.95f, 0.9f, 0.8f };

        [SerializeField]
        private bool healOnAbsorb = true;

        [SerializeField]
        private float healPercentPerExperience = 0.0002f;

        [SerializeField]
        private float maxHealPercentPerAbsorptionAction = 0.05f;

        private EnemyHealth enemyHealth;
        private EnemyController enemyController;
        private EnemyContactDamage contactDamage;
        private BossController bossController;
        private GreedBossVisualController visualController;
        private float baseMoveSpeed;
        private int baseContactDamage;
        private int greedLevel = 1;

        public int TotalAbsorbedExperience { get; private set; }
        public int CurrentGreedGauge { get; private set; }
        public int GreedLevel => greedLevel;
        public event Action<int, int, int> GreedChanged;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
            enemyController = GetComponent<EnemyController>();
            contactDamage = GetComponent<EnemyContactDamage>();
            bossController = GetComponent<BossController>();
            visualController = GetComponent<GreedBossVisualController>();

            baseMoveSpeed = enemyController == null ? 0f : enemyController.MoveSpeed;
            baseContactDamage = contactDamage == null ? 1 : contactDamage.ContactDamage;
            ApplyGreedLevel();
        }

        private void OnValidate()
        {
            healPercentPerExperience = Mathf.Max(0f, healPercentPerExperience);
            maxHealPercentPerAbsorptionAction = Mathf.Clamp01(maxHealPercentPerAbsorptionAction);
        }

        public void AbsorbExperience(int amount)
        {
            if (enemyHealth == null || enemyHealth.IsDead || amount <= 0)
                return;

            TotalAbsorbedExperience += amount;
            CurrentGreedGauge += amount;

            if (healOnAbsorb)
                HealFromAbsorption(amount);

            int nextGreedLevel = CalculateGreedLevel(TotalAbsorbedExperience);

            if (nextGreedLevel != greedLevel)
            {
                greedLevel = nextGreedLevel;
                ApplyGreedLevel();
            }

            GreedChanged?.Invoke(TotalAbsorbedExperience, CurrentGreedGauge, greedLevel);
        }

        public bool TrySpendGauge(int cost)
        {
            if (cost <= 0)
                return true;

            if (CurrentGreedGauge < cost)
                return false;

            CurrentGreedGauge -= cost;
            GreedChanged?.Invoke(TotalAbsorbedExperience, CurrentGreedGauge, greedLevel);
            return true;
        }

        private int CalculateGreedLevel(int totalExperience)
        {
            int level = 1;

            if (greedLevelThresholds == null || greedLevelThresholds.Length == 0)
                return level;

            for (int i = 0; i < greedLevelThresholds.Length; i++)
            {
                if (totalExperience >= Mathf.Max(0, greedLevelThresholds[i]))
                    level = i + 1;
            }

            return Mathf.Clamp(level, 1, 4);
        }

        private void ApplyGreedLevel()
        {
            int index = Mathf.Clamp(greedLevel - 1, 0, 3);

            if (enemyController != null)
                enemyController.SetMoveSpeed(baseMoveSpeed * GetArrayValue(moveSpeedMultipliers, index, 1f));

            if (contactDamage != null)
                contactDamage.SetContactDamage(Mathf.RoundToInt(baseContactDamage * GetArrayValue(contactDamageMultipliers, index, 1f)));

            if (bossController != null)
                bossController.SetPatternCooldownMultiplier(GetArrayValue(patternCooldownMultipliers, index, 1f));

            visualController?.ApplyGreedLevel(greedLevel);
        }

        private void HealFromAbsorption(int amount)
        {
            if (enemyHealth == null)
                return;

            float healPercent = Mathf.Min(maxHealPercentPerAbsorptionAction, amount * healPercentPerExperience);
            int healAmount = Mathf.RoundToInt(enemyHealth.MaxHealth * healPercent);

            if (healAmount > 0)
                enemyHealth.Heal(healAmount);
        }

        private static float GetArrayValue(float[] values, int index, float fallback)
        {
            if (values == null || values.Length == 0)
                return fallback;

            return values[Mathf.Clamp(index, 0, values.Length - 1)];
        }
    }
}
