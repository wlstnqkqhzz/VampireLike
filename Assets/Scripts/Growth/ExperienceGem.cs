using UnityEngine;

namespace VampireLike.Growth
{
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        [SerializeField]
        private int experienceAmount = 1;

        [SerializeField]
        private float attractSpeed = 5f;

        [SerializeField]
        private float attractAcceleration = 18f;

        [SerializeField]
        private float collectDistance = 0.12f;

        private bool isCollected;
        private bool isAttracting;
        private float currentAttractSpeed;
        private Transform target;
        private PlayerExperience targetExperience;

        public int ExperienceAmount => experienceAmount;

        private void Awake()
        {
            Collider2D gemCollider = GetComponent<Collider2D>();
            gemCollider.isTrigger = true;
        }

        private void OnValidate()
        {
            experienceAmount = Mathf.Max(1, experienceAmount);
            attractSpeed = Mathf.Max(0.1f, attractSpeed);
            attractAcceleration = Mathf.Max(0f, attractAcceleration);
            collectDistance = Mathf.Max(0.01f, collectDistance);
        }

        private void Update()
        {
            if (!isAttracting || isCollected || target == null || targetExperience == null)
                return;

            currentAttractSpeed += attractAcceleration * Time.deltaTime;
            float speed = attractSpeed + currentAttractSpeed;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) <= collectDistance)
                Collect(targetExperience);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExperience = other.GetComponentInParent<PlayerExperience>();

            if (playerExperience == null)
                return;

            StartAttract(playerExperience);
        }

        public void StartAttract(PlayerExperience playerExperience)
        {
            if (isCollected || playerExperience == null)
                return;

            targetExperience = playerExperience;
            target = playerExperience.transform;
            isAttracting = true;
        }

        public void Collect(PlayerExperience playerExperience)
        {
            if (isCollected || playerExperience == null)
                return;

            isCollected = true;
            playerExperience.AddExperience(experienceAmount);
            Destroy(gameObject);
        }
    }
}
