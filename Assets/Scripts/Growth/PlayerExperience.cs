using UnityEngine;

namespace VampireLike.Growth
{
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField]
        private int currentExperience;

        public int CurrentExperience => currentExperience;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            currentExperience += amount;
            Debug.Log($"Experience: {currentExperience}");
        }
    }
}
