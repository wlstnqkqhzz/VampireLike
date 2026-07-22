using UnityEngine;

namespace VampireLike.Growth
{
    /// <summary>
    /// 적이 죽었을 때 떨어지는 경험치 보석이다. 플레이어에게 끌려가며 획득된다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        // 획득 시 플레이어에게 줄 경험치 양이다.
        [SerializeField]
        private int experienceAmount = 1;

        // 플레이어에게 끌려가기 시작할 때의 기본 속도다.
        [SerializeField]
        private float attractSpeed = 5f;

        // 흡수 연출 중 점점 빨라지는 가속도다.
        [SerializeField]
        private float attractAcceleration = 18f;

        // 이 거리 안까지 가까워지면 실제 획득 처리한다.
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
            // StartAttract가 호출된 뒤 플레이어 위치로 점점 빨려 들어간다.
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
            // 자석 범위에 들어오거나 직접 닿으면 즉시 삭제하지 않고 흡수 연출을 시작한다.
            if (isCollected || playerExperience == null)
                return;

            targetExperience = playerExperience;
            target = playerExperience.transform;
            isAttracting = true;
        }

        public void Collect(PlayerExperience playerExperience)
        {
            // 중복 획득을 막고 경험치를 더한 뒤 보석을 제거한다.
            if (isCollected || playerExperience == null)
                return;

            isCollected = true;
            playerExperience.AddExperience(experienceAmount);
            Destroy(gameObject);
        }
    }
}
