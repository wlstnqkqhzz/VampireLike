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

        [Header("First Experience")]
        [SerializeField]
        private int simpleChoiceUntilLevel = 3;

        [Header("Character Exclusive Choices")]
        [SerializeField]
        [Range(0f, 1f)]
        private float characterExclusiveUpgradeChance = 0.3f;

        [SerializeField]
        private int maxCharacterExclusiveChoices = 1;

        // 강화 타입별 현재 레벨을 런타임에 기록한다.
        private readonly Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
        private PlayerAutoAttack autoAttack;
        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private PlayerSpecialUpgradeController specialUpgradeController;
        private global::PlayerController playerController;
        private static readonly HashSet<UpgradeType> SimpleEarlyUpgradeTypes = new HashSet<UpgradeType>
        {
            UpgradeType.ProjectileDamage,
            UpgradeType.AttackInterval,
            UpgradeType.MoveSpeed,
            UpgradeType.MaxHealth,
            UpgradeType.PickupRadius
        };

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
            simpleChoiceUntilLevel = Mathf.Max(0, simpleChoiceUntilLevel);
            characterExclusiveUpgradeChance = Mathf.Clamp01(characterExclusiveUpgradeChance);
            maxCharacterExclusiveChoices = Mathf.Max(0, maxCharacterExclusiveChoices);

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

        public List<UpgradeChoice> GetRandomChoices(int count, int playerLevel = 0)
        {
            return GetRandomChoices(count, playerLevel, false);
        }

        public List<UpgradeChoice> GetRandomChoices(int count, int playerLevel, bool bossRewardOnly)
        {
            // 최대 레벨에 도달하지 않은 강화 중에서 중복 없이 랜덤 선택한다.
            List<UpgradeDefinition> availableDefinitions = GetAvailableDefinitions(playerLevel, bossRewardOnly);
            List<UpgradeChoice> choices = new List<UpgradeChoice>();
            List<UpgradeDefinition> normalDefinitions = new List<UpgradeDefinition>();
            List<UpgradeDefinition> specialDefinitions = new List<UpgradeDefinition>();
            List<UpgradeDefinition> characterDefinitions = new List<UpgradeDefinition>();

            for (int i = 0; i < availableDefinitions.Count; i++)
            {
                UpgradeDefinition definition = availableDefinitions[i];

                if (definition.IsCharacterExclusiveUpgrade)
                    characterDefinitions.Add(definition);
                else if (definition.IsSpecialUpgrade)
                    specialDefinitions.Add(definition);
                else
                    normalDefinitions.Add(definition);
            }

            bool hasNormalChoice = bossRewardOnly;
            int specialChoiceCount = 0;
            int characterChoiceCount = 0;

            while (choices.Count < count && (normalDefinitions.Count > 0 || specialDefinitions.Count > 0 || characterDefinitions.Count > 0))
            {
                bool chooseCharacter = bossRewardOnly || ShouldChooseCharacterExclusiveChoice(
                    choices.Count,
                    count,
                    hasNormalChoice,
                    characterChoiceCount,
                    normalDefinitions.Count,
                    characterDefinitions.Count);

                bool chooseSpecial = !chooseCharacter && (bossRewardOnly || ShouldChooseSpecialChoice(
                    choices.Count,
                    count,
                    hasNormalChoice,
                    specialChoiceCount,
                    normalDefinitions.Count,
                    specialDefinitions.Count));

                UpgradeDefinition definition = null;

                if (chooseCharacter)
                    definition = TakeRandomDefinition(characterDefinitions);
                else if (chooseSpecial)
                    definition = TakeRandomDefinition(specialDefinitions);
                else
                    definition = TakeRandomDefinition(normalDefinitions);

                if (!bossRewardOnly && definition == null)
                    definition = TakeRandomDefinition(normalDefinitions);

                if (definition == null)
                    definition = TakeRandomDefinition(specialDefinitions);

                if (definition == null)
                    definition = TakeRandomDefinition(characterDefinitions);

                if (definition == null)
                    break;

                if (!CanApply(definition))
                    continue;

                if (definition.IsCharacterExclusiveUpgrade)
                    characterChoiceCount++;
                else if (definition.IsSpecialUpgrade)
                    specialChoiceCount++;
                else
                    hasNormalChoice = true;

                choices.Add(new UpgradeChoice(definition, GetCurrentLevel(definition), GetMaxLevel(definition)));
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

            if (currentSpecialChoiceCount >= maxSpecialChoices)
                return false;

            if (normalCount <= 0)
                return true;

            bool isLastChoice = currentChoiceCount >= targetChoiceCount - 1;

            if (forceAtLeastOneNormalChoice && isLastChoice && !hasNormalChoice)
                return false;

            return Random.value < specialUpgradeChance;
        }

        private bool ShouldChooseCharacterExclusiveChoice(
            int currentChoiceCount,
            int targetChoiceCount,
            bool hasNormalChoice,
            int currentCharacterChoiceCount,
            int normalCount,
            int characterCount)
        {
            if (characterCount <= 0)
                return false;

            if (currentCharacterChoiceCount >= maxCharacterExclusiveChoices)
                return false;

            if (normalCount <= 0)
                return true;

            bool isLastChoice = currentChoiceCount >= targetChoiceCount - 1;

            if (forceAtLeastOneNormalChoice && isLastChoice && !hasNormalChoice)
                return false;

            return Random.value < characterExclusiveUpgradeChance;
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
            if (!TryReserveUpgradeLevel(definition))
                return;

            CacheComponents();

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
                    {
                        if (definition.Multiplier > 1f)
                            playerHealth.MultiplyMaxHealth(definition.Multiplier);
                        else
                            playerHealth.IncreaseMaxHealth(definition.FlatAmount);
                    }
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
                case UpgradeType.SequentialShot:
                    if (autoAttack != null)
                        autoAttack.AddProjectileCount(definition.FlatAmount);
                    break;
                case UpgradeType.KaelBlackSwordWave:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddKaelBlackSwordWaveLevel();
                    break;
                case UpgradeType.KaelGuardianResolve:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddKaelGuardianResolveLevel();
                    break;
                case UpgradeType.KaelManaSlash:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddKaelManaSlashLevel();
                    break;
                case UpgradeType.KaelBlackIronBarrier:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddKaelBlackIronBarrierLevel();
                    break;
                case UpgradeType.KaelExecutionBlade:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddKaelExecutionBladeLevel();
                    break;
                case UpgradeType.SeleneMoonShadowClone:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddSeleneMoonShadowCloneLevel();
                    break;
                case UpgradeType.SeleneShadowStep:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddSeleneShadowStepLevel();
                    break;
                case UpgradeType.SeleneTwinMoonFlurry:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddSeleneTwinMoonFlurryLevel();
                    break;
                case UpgradeType.SeleneMoonlightMark:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddSeleneMoonlightMarkLevel();
                    break;
                case UpgradeType.SeleneSilentBlade:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddSeleneSilentBladeLevel();
                    break;
                case UpgradeType.HanSeorinBloodMark:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddHanSeorinBloodMarkLevel();
                    break;
                case UpgradeType.HanSeorinShadowDagger:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddHanSeorinShadowDaggerLevel();
                    break;
                case UpgradeType.HanSeorinReturningBlade:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddHanSeorinReturningBladeLevel();
                    break;
                case UpgradeType.HanSeorinKillingIntent:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddHanSeorinKillingIntentLevel();
                    break;
                case UpgradeType.HanSeorinRedExecution:
                    if (specialUpgradeController != null)
                        specialUpgradeController.AddHanSeorinRedExecutionLevel();
                    break;
            }

            GameSessionStats.RecordUpgrade(definition.DisplayName);
            Debug.Log($"Upgrade Selected: {definition.DisplayName}");
        }

        public List<string> GetUpgradeStatusLines()
        {
            List<string> lines = new List<string>();

            if (upgradeDefinitions == null)
                return lines;

            HashSet<UpgradeType> addedTypes = new HashSet<UpgradeType>();

            foreach (UpgradeDefinition definition in upgradeDefinitions)
            {
                if (definition == null || definition.Unlimited)
                    continue;

                UpgradeType upgradeType = definition.UpgradeType == UpgradeType.SequentialShot
                    ? UpgradeType.ProjectileCount
                    : definition.UpgradeType;

                if (!addedTypes.Add(upgradeType))
                    continue;

                int level = GetCurrentLevel(definition);

                if (level <= 0)
                    continue;

                string valueText = GetUpgradeValueText(definition, level);
                lines.Add(string.IsNullOrEmpty(valueText)
                    ? $"{definition.DisplayName} Lv.{level}"
                    : $"{definition.DisplayName} Lv.{level} ({valueText})");
            }

            return lines;
        }

        public Dictionary<string, List<string>> GetUpgradeStatusLinesByCategory()
        {
            Dictionary<string, List<string>> linesByCategory = new Dictionary<string, List<string>>
            {
                { "일반 강화", new List<string>() },
                { "특수 강화", new List<string>() },
                { "전용 강화", new List<string>() }
            };

            if (upgradeDefinitions == null)
                return linesByCategory;

            HashSet<UpgradeType> addedTypes = new HashSet<UpgradeType>();

            foreach (UpgradeDefinition definition in upgradeDefinitions)
            {
                if (definition == null || definition.Unlimited)
                    continue;

                UpgradeType upgradeType = definition.UpgradeType == UpgradeType.SequentialShot
                    ? UpgradeType.ProjectileCount
                    : definition.UpgradeType;

                if (!addedTypes.Add(upgradeType))
                    continue;

                int level = GetCurrentLevel(definition);

                if (level <= 0)
                    continue;

                string category = definition.IsCharacterExclusiveUpgrade
                    ? "전용 강화"
                    : definition.IsSpecialUpgrade ? "특수 강화" : "일반 강화";
                linesByCategory[category].Add(GetUpgradeStatusLine(definition, level));
            }

            return linesByCategory;
        }

        private static string GetUpgradeValueText(UpgradeDefinition definition, int level)
        {
            if (definition == null)
                return string.Empty;

            if (definition.FlatAmount > 0)
                return $"+{definition.FlatAmount * level}";

            if (!Mathf.Approximately(definition.Multiplier, 1f) && definition.Multiplier > 0f)
            {
                float totalMultiplier = Mathf.Pow(definition.Multiplier, level);
                float percent = (totalMultiplier - 1f) * 100f;
                return $"{percent:+0;-0;0}%";
            }

            return string.Empty;
        }

        private static string GetUpgradeStatusLine(UpgradeDefinition definition, int level)
        {
            string valueText = GetUpgradeValueText(definition, level);
            return string.IsNullOrEmpty(valueText)
                ? $"{definition.DisplayName} Lv.{level}"
                : $"{definition.DisplayName} Lv.{level} ({valueText})";
        }

        private List<UpgradeDefinition> GetAvailableDefinitions(int playerLevel)
        {
            return GetAvailableDefinitions(playerLevel, false);
        }

        private List<UpgradeDefinition> GetAvailableDefinitions(int playerLevel, bool bossRewardOnly)
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

                if (!CanAppearForActiveCharacter(definition))
                    continue;

                if (definition.UpgradeType == UpgradeType.SequentialShot || definition.UpgradeType == UpgradeType.Vampirism)
                    continue;

                if (bossRewardOnly && !definition.IsSpecialUpgrade && !definition.IsCharacterExclusiveUpgrade)
                    continue;

                if (!definition.Unlimited && !addedLimitedTypes.Add(definition.UpgradeType))
                    continue;

                availableDefinitions.Add(definition);
            }

            if (!bossRewardOnly && ShouldUseSimpleEarlyChoices(playerLevel))
            {
                List<UpgradeDefinition> simpleDefinitions = new List<UpgradeDefinition>();

                foreach (UpgradeDefinition definition in availableDefinitions)
                {
                    if (definition == null)
                        continue;

                    if (definition.IsSpecialUpgrade || definition.IsCharacterExclusiveUpgrade)
                        continue;

                    if (SimpleEarlyUpgradeTypes.Contains(definition.UpgradeType))
                        simpleDefinitions.Add(definition);
                }

                if (simpleDefinitions.Count >= 3)
                    return simpleDefinitions;
            }

            return availableDefinitions;
        }

        private bool ShouldUseSimpleEarlyChoices(int playerLevel)
        {
            return simpleChoiceUntilLevel > 0
                && playerLevel > 0
                && playerLevel <= simpleChoiceUntilLevel;
        }

        private bool CanApply(UpgradeDefinition definition)
        {
            if (definition == null)
                return false;

            if (!CanAppearForActiveCharacter(definition))
                return false;

            if (definition.Unlimited)
                return true;

            int maxLevel = GetMaxLevel(definition);

            if (maxLevel <= 0)
                return false;

            return GetCurrentLevel(definition) < maxLevel;
        }

        private bool TryReserveUpgradeLevel(UpgradeDefinition definition)
        {
            if (!CanApply(definition))
                return false;

            if (definition.Unlimited)
                return true;

            int maxLevel = GetMaxLevel(definition);
            upgradeLevels[definition.UpgradeType] = Mathf.Min(GetCurrentLevel(definition) + 1, maxLevel);
            return true;
        }

        private int GetCurrentLevel(UpgradeDefinition definition)
        {
            if (definition == null)
                return 0;

            int trackedLevel = GetLevel(definition.UpgradeType);

            if (definition.IsSpecialUpgrade || definition.IsCharacterExclusiveUpgrade)
            {
                CacheComponents();
                if (specialUpgradeController != null)
                    trackedLevel = Mathf.Max(trackedLevel, specialUpgradeController.GetAppliedUpgradeLevel(definition.UpgradeType));
            }

            return trackedLevel;
        }

        private int GetLevel(UpgradeType upgradeType)
        {
            return upgradeLevels.TryGetValue(upgradeType, out int level) ? level : 0;
        }

        private int GetMaxLevel(UpgradeDefinition definition)
        {
            return GetActiveCharacter().GetMaxLevel(definition);
        }

        private static CharacterDefinition GetActiveCharacter()
        {
            string activeCharacterId = GetActiveCharacterId();
            CharacterDefinition[] characters = CharacterSelection.Characters;

            for (int i = 0; i < characters.Length; i++)
            {
                if (string.Equals(characters[i].Id, activeCharacterId, System.StringComparison.OrdinalIgnoreCase))
                    return characters[i];
            }

            return CharacterSelection.SelectedCharacter;
        }

        private static string GetActiveCharacterId()
        {
            return CharacterSelection.SelectedCharacter.Id;
        }

        private static bool CanAppearForActiveCharacter(UpgradeDefinition definition)
        {
            return definition != null && definition.CanAppearForCharacter(GetActiveCharacterId());
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
