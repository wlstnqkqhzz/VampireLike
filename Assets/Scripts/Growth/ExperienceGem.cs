using UnityEngine;

namespace VampireLike.Growth
{
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        [SerializeField]
        private int experienceAmount = 1;

        public int ExperienceAmount => experienceAmount;

        private void Awake()
        {
            Collider2D gemCollider = GetComponent<Collider2D>();
            gemCollider.isTrigger = true;
        }

        private void OnValidate()
        {
            experienceAmount = Mathf.Max(1, experienceAmount);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExperience = other.GetComponentInParent<PlayerExperience>();

            if (playerExperience == null)
                return;

            playerExperience.AddExperience(experienceAmount);
            Destroy(gameObject);
        }
    }
}
