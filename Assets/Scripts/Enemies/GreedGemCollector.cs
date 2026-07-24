using UnityEngine;
using VampireLike.Combat;
using VampireLike.Growth;

namespace VampireLike.Enemies
{
    /// <summary>
    /// Greed Lord가 일정 주기마다 주변 경험치 보석을 찾아 끌어당기고 흡수한다.
    /// 매 프레임 전체 검색을 하지 않고 Physics2D 반경 검색을 주기적으로만 수행한다.
    /// </summary>
    [RequireComponent(typeof(GreedBossController))]
    public class GreedGemCollector : MonoBehaviour
    {
        [SerializeField]
        private float gemSearchInterval = 0.35f;

        [SerializeField]
        private float gemSearchRadius = 8f;

        [SerializeField]
        private int maxClaimsPerSearch = 3;

        [SerializeField]
        private LayerMask gemLayerMask = ~0;

        private readonly Collider2D[] gemResults = new Collider2D[32];
        private GreedBossController greedBoss;
        private float searchTimer;

        private void Awake()
        {
            greedBoss = GetComponent<GreedBossController>();
        }

        private void OnValidate()
        {
            gemSearchInterval = Mathf.Max(0.05f, gemSearchInterval);
            gemSearchRadius = Mathf.Max(0.1f, gemSearchRadius);
            maxClaimsPerSearch = Mathf.Max(1, maxClaimsPerSearch);
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || GameState.IsGameOver)
                return;

            searchTimer += Time.deltaTime;

            if (searchTimer < gemSearchInterval)
                return;

            searchTimer = 0f;
            ClaimNearbyGems();
        }

        private void ClaimNearbyGems()
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, gemSearchRadius, gemResults, gemLayerMask);
            int claimedCount = 0;

            for (int i = 0; i < hitCount && claimedCount < maxClaimsPerSearch; i++)
            {
                Collider2D hit = gemResults[i];

                if (hit == null)
                    continue;

                ExperienceGem gem = hit.GetComponentInParent<ExperienceGem>();

                if (gem == null || gem.IsClaimed)
                    continue;

                if (gem.StartAttractToBoss(transform, HandleGemAbsorbed))
                    claimedCount++;
            }
        }

        private void HandleGemAbsorbed(int experienceAmount)
        {
            greedBoss.AbsorbExperience(experienceAmount);
        }
    }
}
