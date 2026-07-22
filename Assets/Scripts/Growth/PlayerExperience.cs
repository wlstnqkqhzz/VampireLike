using UnityEngine;

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

        public int CurrentExperience => currentExperience;
        public int CurrentLevel => currentLevel;
        public int ExperienceToNextLevel => experienceToNextLevel;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            currentExperience += amount;
            Debug.Log($"Experience: {currentExperience}/{experienceToNextLevel}");
            CheckLevelUp();
        }

        private void OnValidate()
        {
            currentExperience = Mathf.Max(0, currentExperience);
            currentLevel = Mathf.Max(1, currentLevel);
            experienceToNextLevel = Mathf.Max(1, experienceToNextLevel);
            nextLevelExperienceMultiplier = Mathf.Max(1f, nextLevelExperienceMultiplier);
        }

        private void CheckLevelUp()
        {
            while (currentExperience >= experienceToNextLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = Mathf.CeilToInt(experienceToNextLevel * nextLevelExperienceMultiplier);
                Debug.Log($"Level Up! Level {currentLevel} / Next EXP: {currentExperience}/{experienceToNextLevel}");
            }
        }
    }
}
