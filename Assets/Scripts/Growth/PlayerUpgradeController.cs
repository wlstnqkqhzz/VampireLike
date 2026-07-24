using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

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

        // 강화 타입별 현재 레벨을 런타임에 기록한다.
        private readonly Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
        private PlayerAutoAttack autoAttack;
        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private global::PlayerController playerController;

        public readonly struct UpgradeChoice
        {
            // UI가 표시할 강화 데이터와 현재 레벨을 함께 담는 선택지 구조체다.
            public UpgradeChoice(UpgradeDefinition definition, int currentLevel)
            {
                Definition = definition;
                CurrentLevel = currentLevel;
            }

            public UpgradeDefinition Definition { get; }
            public int CurrentLevel { get; }

            public string ButtonText
            {
                get
                {
                    if (Definition.Unlimited)
                        return $"{Definition.DisplayName}\n{Definition.Description}";

                    return $"{Definition.DisplayName} Lv.{CurrentLevel + 1}/{Definition.MaxLevel}\n{Definition.Description}";
                }
            }
        }

        private void Awake()
        {
            CacheComponents();
        }

        private void OnValidate()
        {
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

            while (choices.Count < count && availableDefinitions.Count > 0)
            {
                int index = Random.Range(0, availableDefinitions.Count);
                UpgradeDefinition definition = availableDefinitions[index];
                availableDefinitions.RemoveAt(index);
                choices.Add(new UpgradeChoice(definition, GetLevel(definition.UpgradeType)));
            }

            return choices;
        }

        public void ApplyUpgrade(UpgradeDefinition definition)
        {
            // 선택된 강화 데이터를 실제 담당 컴포넌트로 전달한다.
            if (definition == null || !CanApply(definition))
                return;

            CacheComponents();

            if (!definition.Unlimited)
                upgradeLevels[definition.UpgradeType] = Mathf.Min(GetLevel(definition.UpgradeType) + 1, definition.MaxLevel);

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
            }

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

            return GetLevel(definition.UpgradeType) < definition.MaxLevel;
        }

        private int GetLevel(UpgradeType upgradeType)
        {
            return upgradeLevels.TryGetValue(upgradeType, out int level) ? level : 0;
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

            if (playerController == null)
                playerController = GetComponent<global::PlayerController>();
        }
    }
}
