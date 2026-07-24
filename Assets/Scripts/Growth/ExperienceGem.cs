using System;
using UnityEngine;

namespace VampireLike.Growth
{
    /// <summary>
    /// 적이 죽었을 때 떨어지는 경험치 보석이다.
    /// 플레이어뿐 아니라 히든 보스도 같은 보석을 끌어당겨 흡수할 수 있게 소유권을 구분한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        public enum GemCollectorType
        {
            None,
            Player,
            GreedBoss
        }

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
        private Action<int> bossCollectCallback;
        private GemCollectorType collectorType = GemCollectorType.None;

        public int ExperienceAmount => experienceAmount;
        public bool IsClaimed => isCollected;
        public GemCollectorType CollectorType => collectorType;

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
            if (!isAttracting || isCollected || target == null)
                return;

            currentAttractSpeed += attractAcceleration * Time.deltaTime;
            float speed = attractSpeed + currentAttractSpeed;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) > collectDistance)
                return;

            if (collectorType == GemCollectorType.Player)
                Collect(targetExperience);
            else if (collectorType == GemCollectorType.GreedBoss)
                CollectByBoss();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExperience = other.GetComponentInParent<PlayerExperience>();

            if (playerExperience != null)
                StartAttract(playerExperience);
        }

        public void StartAttract(PlayerExperience playerExperience)
        {
            if (isCollected || playerExperience == null)
                return;

            targetExperience = playerExperience;
            target = playerExperience.transform;
            bossCollectCallback = null;
            collectorType = GemCollectorType.Player;
            currentAttractSpeed = 0f;
            isAttracting = true;
        }

        public bool StartAttractToBoss(Transform bossTarget, Action<int> onCollected)
        {
            // 플레이어가 이미 가져가는 중인 보석은 빼앗지 않는다.
            if (isCollected || bossTarget == null || collectorType == GemCollectorType.Player)
                return false;

            targetExperience = null;
            target = bossTarget;
            bossCollectCallback = onCollected;
            collectorType = GemCollectorType.GreedBoss;
            currentAttractSpeed = 0f;
            isAttracting = true;
            return true;
        }

        public void SetExperienceAmount(int amount)
        {
            experienceAmount = Mathf.Max(1, amount);
        }

        public void Collect(PlayerExperience playerExperience)
        {
            if (isCollected || playerExperience == null)
                return;

            isCollected = true;
            collectorType = GemCollectorType.Player;
            playerExperience.AddExperience(experienceAmount);
            Destroy(gameObject);
        }

        private void CollectByBoss()
        {
            if (isCollected)
                return;

            isCollected = true;
            bossCollectCallback?.Invoke(experienceAmount);
            Destroy(gameObject);
        }
    }
}
