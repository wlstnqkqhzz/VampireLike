using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Growth
{
    public class PlayerUpgradeController : MonoBehaviour
    {
        [SerializeField]
        private UpgradeDefinition[] upgradeDefinitions;

        private readonly Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
        private PlayerAutoAttack autoAttack;
        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private global::PlayerController playerController;

        public readonly struct UpgradeChoice
        {
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
            if (definition == null || !CanApply(definition))
                return;

            CacheComponents();

            if (!definition.Unlimited)
                upgradeLevels[definition.UpgradeType] = GetLevel(definition.UpgradeType) + 1;

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
            List<UpgradeDefinition> availableDefinitions = new List<UpgradeDefinition>();

            if (upgradeDefinitions == null)
                return availableDefinitions;

            foreach (UpgradeDefinition definition in upgradeDefinitions)
            {
                if (definition != null && CanApply(definition))
                    availableDefinitions.Add(definition);
            }

            return availableDefinitions;
        }

        private bool CanApply(UpgradeDefinition definition)
        {
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
