using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;
using VampireLike.Menu;

namespace VampireLike.Growth
{
    /// <summary>
    /// 레벨업 강화 후보를 뽑고, 선택된 UpgradeDefinition의 효과를 실제 플레이어 능력치에 적용한다.
    /// </summary>
    public class PlayerUpgradeController : MonoBehaviour
    {
        // 레벨업 선택 후보로 사용할 강화 데이터 목록이다.
        [SerializeField]
        private UpgradeDefinition[] upgradeDefinitions;

        [Header("Choice Rarity")]
        [SerializeField]
        [Range(0f, 1f)]
        private float specialUpgradeChance = 0.28f;

        [SerializeField]
        private int maxSpecialChoices = 1;

        [SerializeField]
        private bool forceAtLeastOneNormalChoice = true;

        // 강화 타입별 현재 레벨을 런타임에 기록한다.
        private readonly Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
        private PlayerAutoAttack autoAttack;
        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private PlayerSpecialUpgradeController specialUpgradeController;
        private global::PlayerController playerController;

        public readonly struct UpgradeChoice
        {
            // UI가 표시할 강화 데이터와 현재 레벨을 함께 담는 선택지 구조체다.
            public UpgradeChoice(UpgradeDefinition definition, int currentLevel, int maxLevel)
            {
                Definition = definition;
                CurrentLevel = currentLevel;
                MaxLevel = maxLevel;
            }

            public UpgradeDefinition Definition { get; }
            public int CurrentLevel { get; }
            public int MaxLevel { get; }

            public string ButtonText
            {
                get
                {
                    if (Definition.Unlimited)
                        return $"[{Definition.GradeLabel}] {Definition.DisplayName}\n{Definition.Description}";

                    return $"[{Definition.GradeLabel}] {Definition.DisplayName} Lv.{CurrentLevel + 1}/{MaxLevel}\n{Definition.Description}";
                }
            }
        }

        private void Awake()
        {
            CacheComponents();
        }

        private void OnValidate()
        {
            specialUpgradeChance = Mathf.Clamp01(specialUpgradeChance);
            maxSpecialChoices = Mathf.Max(0, maxSpecialChoices);

            if (upgradeDefinitions == null)
                return;

            for (int i = 0; i < upgradeDefinitions.Length; i++)
            {
                for (int j = i + 1; j < upgradeDefinitions.Length; j++)
                {
                    if (upgradeDefinitions[i] != null && upgradeDefinitions[j] != null && upgradeDefinitions[i] == upgradeDefinitions[j])
                        upgradeDefinitions[j] = null;
                }
            }
        }

        public List<UpgradeChoice> GetRandomChoices(int count)
        {
            // 최대 레벨에 도달하지 않은 강화 중에서 중복 없이 랜덤 선택한다.
            List<UpgradeDefinition> availableDefinitions = GetAvailableDefinitions();
            List<UpgradeChoice> choices = new List<UpgradeChoice>();
            List<UpgradeDefinition> normalDefinitions = new List<UpgradeDefinition>();
            List<UpgradeDefinition> specialDefinitions = new List<UpgradeDefinition>();

            for (int i = 0; i < availableDefinitions.Count; i++)
            {
                UpgradeDefinition definition = availableDefinitions[i];

                if (definition.IsSpecialUpgrade)
                    specialDefinitions.Add(definition);
                else
                    normalDefinitions.Add(definition);
            }

            bool hasNormalChoice = false;
            int specialChoiceCount = 0;

            while (choices.Count < count && (normalDefinitions.Count > 0 || specialDefinitions.Count > 0))
            {
                bool chooseSpecial = ShouldChooseSpecialChoice(
                    choices.Count,
                    count,
                    hasNormalChoice,
                    specialChoiceCount,
                    normalDefinitions.Count,
                    specialDefinitions.Count);

                UpgradeDefinition definition = chooseSpecial
                    ? TakeRandomDefinition(specialDefinitions)
                    : TakeRandomDefinition(normalDefinitions);

                if (definition == null)
                    definition = TakeRandomDefinition(chooseSpecial ? normalDefinitions : specialDefinitions);

                if (definition == null)
                    break;

                if (definition.IsSpecialUpgrade)
                    specialChoiceCount++;
                else
                    hasNormalChoice = true;

                choices.Add(new UpgradeChoice(definition, GetLevel(definition.UpgradeType), GetMaxLevel(definition)));
            }

            return choices;
        }

        private bool ShouldChooseSpecialChoice(
            int currentChoiceCount,
            int targetChoiceCount,
            bool hasNormalChoice,
            int currentSpecialChoiceCount,
            int normalCount,
            int specialCount)
        {
            if (specialCount <= 0)
                return false;

            if (normalCount <= 0)
                return true;

            if (currentSpecialChoiceCount >= maxSpecialChoices)
                return false;

            bool isLastChoice = currentChoiceCount >= targetChoiceCount - 1;

            if (forceAtLeastOneNormalChoice && isLastChoice && !hasNormalChoice)
                return false;

            return Random.value < specialUpgradeChance;
        }

        private static UpgradeDefinition TakeRandomDefinition(List<UpgradeDefinition> definitions)
        {
            if (definitions == null || definitions.Count == 0)
                return null;

            int index = Random.Range(0, definitions.Count);
            UpgradeDefinition definition = definitions[index];
            definitions.RemoveAt(index);
            return definition;
        }

        public void ApplyUpgrade(UpgradeDefinition definition)
        {
            // 선택된 강화 데이터를 실제 담당 컴포넌트로 전달한다.
            if (definition == null || !CanApply(definition))
                return;

            CacheComponents();

            if (!definition.Unlimited)
                upgradeLevels[definition.UpgradeType] = Mathf.Min(GetLevel(definition.UpgradeType) + 1, GetMaxLevel(definition));

            switch (definition.UpgradeType)
            {
                case UpgradeType.ProjectileDamage:
                    if (autoAttack != null)
                        autoAttack.MultiplyProjectileDamage(definition.Multiplier);
                    break;
                case UpgradeType.AttackInterval:
                    if (autoAttack != null)
                        autoAttack.MultiplyAttackInterval(definition.Multiplier);
                    break;
                case UpgradeType.ProjectileCount:
                    if (autoAttack != null)
                        autoAttack.AddProjectileCount(definition.FlatAmount);
                    break;
                case UpgradeType.ProjectilePierce:
                    if (autoAttack != null)
                        autoAttack.AddProjectilePierceCount(definition.FlatAmount);
                    break;
                case UpgradeType.MoveSpeed:
                    if (playerController != null)
                        playerController.MultiplyMoveSpeed(definition.Multiplier);
                    break;
                case UpgradeType.MaxHealth:
                    if (playerHealth != null)
                        playerHealth.IncreaseMaxHealth(definition.FlatAmount);
                    break;
                case UpgradeType.Heal:
                    if (playerHealth != null)
                        playerHealth.Heal(definition.FlatAmount);
                    break;
                case UpgradeType.PickupRadius:
                    if (playerExperience != null)
                        playerExperience.MultiplyPickupRadius(definition.Multiplier);
                    break;
                case UpgradeType.ExplosiveShot:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddExplosiveShotLevel();
                    break;
                case UpgradeType.FrostShot:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddFrostShotLevel();
                    break;
                case UpgradeType.Vampirism:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddVampirismLevel();
                    break;
                case UpgradeType.Shockwave:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddShockwaveLevel();
                    break;
                case UpgradeType.ScatterShot:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddScatterShotLevel();
                    break;
                case UpgradeType.Shield:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddShieldLevel();
                    break;
                case UpgradeType.OrbitingBlade:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddOrbitingBladeLevel();
                    break;
                case UpgradeType.ChainRicochet:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddChainRicochetLevel();
                    break;
                case UpgradeType.EclipseAura:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddEclipseAuraLevel();
                    break;
                case UpgradeType.ProjectileReflect:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddProjectileReflectLevel();
                    break;
            }

            GameSessionStats.RecordUpgrade(definition.DisplayName);
            Debug.Log($"Upgrade Selected: {definition.DisplayName}");
        }

        private List<UpgradeDefinition> GetAvailableDefinitions()
        {
            // 제한 레벨이 남아 있거나 무제한 강화인 것만 후보로 사용한다.
            List<UpgradeDefinition> availableDefinitions = new List<UpgradeDefinition>();
            HashSet<UpgradeType> addedLimitedTypes = new HashSet<UpgradeType>();

            if (upgradeDefinitions == null)
                return availableDefinitions;

            foreach (UpgradeDefinition definition in upgradeDefinitions)
            {
                if (definition == null || !CanApply(definition))
                    continue;

                if (!definition.Unlimited && !addedLimitedTypes.Add(definition.UpgradeType))
                    continue;

                availableDefinitions.Add(definition);
            }

            return availableDefinitions;
        }

        private bool CanApply(UpgradeDefinition definition)
        {
            if (definition == null)
                return false;

            if (definition.Unlimited)
                return true;

            return GetLevel(definition.UpgradeType) < GetMaxLevel(definition);
        }

        private int GetLevel(UpgradeType upgradeType)
        {
            return upgradeLevels.TryGetValue(upgradeType, out int level) ? level : 0;
        }

        private int GetMaxLevel(UpgradeDefinition definition)
        {
            return CharacterSelection.SelectedCharacter.GetMaxLevel(definition);
        }

        private void CacheComponents()
        {
            // 강화 효과를 적용할 대상 컴포넌트들을 필요할 때 찾아 캐시한다.
            if (autoAttack == null)
                autoAttack = GetComponent<PlayerAutoAttack>();

            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (playerExperience == null)
                playerExperience = GetComponent<PlayerExperience>();

            if (specialUpgradeController == null)
                specialUpgradeController = GetComponent<PlayerSpecialUpgradeController>();

            if (specialUpgradeController == null)
                specialUpgradeController = gameObject.AddComponent<PlayerSpecialUpgradeController>();

            if (playerController == null)
                playerController = GetComponent<global::PlayerController>();
        }
    }
}
