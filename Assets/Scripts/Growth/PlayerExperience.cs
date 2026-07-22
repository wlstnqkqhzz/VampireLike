using UnityEngine;
using System;

namespace VampireLike.Growth
{
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField]
        private int currentExperience;

        [SerializeField]
        private int currentLevel = 1;

        [SerializeField]
        private int experienceToNextLevel = 5;

        [SerializeField]
        private float nextLevelExperienceMultiplier = 1.5f;

        [SerializeField]
        private float pickupRadius = 0.45f;

        [SerializeField]
        private LayerMask pickupLayerMask = ~0;

        private LevelUpChoiceUI levelUpChoiceUI;
        private bool hasPendingLevelUpChoice;
        private readonly Collider2D[] pickupResults = new Collider2D[16];

        public int CurrentExperience => currentExperience;
        public int CurrentLevel => currentLevel;
        public int ExperienceToNextLevel => experienceToNextLevel;
        public float PickupRadius => pickupRadius;
        public float ExperienceProgress => experienceToNextLevel <= 0 ? 0f : (float)currentExperience / experienceToNextLevel;
        public event Action<int, int, int> ExperienceChanged;

        private void Awake()
        {
            levelUpChoiceUI = GetComponent<LevelUpChoiceUI>();

            if (levelUpChoiceUI == null)
                levelUpChoiceUI = gameObject.AddComponent<LevelUpChoiceUI>();

            if (GetComponent<PlayerUpgradeController>() == null)
                gameObject.AddComponent<PlayerUpgradeController>();

            if (GetComponent<PlayerExperienceUI>() == null)
                gameObject.AddComponent<PlayerExperienceUI>();
        }

        private void Start()
        {
            NotifyExperienceChanged();
        }

        private void Update()
        {
            CollectNearbyExperienceGems();
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            currentExperience += amount;
            CheckLevelUp();
            NotifyExperienceChanged();
            Debug.Log($"Experience: {currentExperience}/{experienceToNextLevel}");
        }

        public void MultiplyPickupRadius(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            pickupRadius *= multiplier;
        }

        private void OnValidate()
        {
            currentExperience = Mathf.Max(0, currentExperience);
            currentLevel = Mathf.Max(1, currentLevel);
            experienceToNextLevel = Mathf.Max(1, experienceToNextLevel);
            nextLevelExperienceMultiplier = Mathf.Max(1f, nextLevelExperienceMultiplier);
            pickupRadius = Mathf.Max(0.05f, pickupRadius);
        }

        private void CheckLevelUp()
        {
            while (currentExperience >= experienceToNextLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = Mathf.CeilToInt(experienceToNextLevel * nextLevelExperienceMultiplier);
                Debug.Log($"Level Up! Level {currentLevel} / Next EXP: {currentExperience}/{experienceToNextLevel}");
                hasPendingLevelUpChoice = true;
            }

            if (hasPendingLevelUpChoice)
            {
                hasPendingLevelUpChoice = false;
                levelUpChoiceUI.Show(currentLevel);
            }
        }

        private void NotifyExperienceChanged()
        {
            ExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
        }

        private void CollectNearbyExperienceGems()
        {
            if (Time.timeScale <= 0f)
                return;

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRadius, pickupResults, pickupLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = pickupResults[i];

                if (hit == null)
                    continue;

                ExperienceGem gem = hit.GetComponentInParent<ExperienceGem>();

                if (gem != null)
                    gem.StartAttract(this);
            }
        }
    }
}
