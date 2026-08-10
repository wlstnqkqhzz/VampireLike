using UnityEngine;

namespace VampireLike.VFX
{
    /// <summary>
    /// 전투 중 캐릭터와 드롭 아이템이 화면 아래쪽에 있을수록 앞에 보이도록 정렬합니다.
    /// SpriteRenderer가 달린 자식 오브젝트에 붙여도 실제 정렬 기준은 루트 Transform으로 지정할 수 있습니다.
    /// </summary>
    [DisallowMultipleComponent]
    public class SpriteDepthSorter : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer targetRenderer;

        [SerializeField]
        private Transform sortOrigin;

        [SerializeField]
        private int baseOrder = 1000;

        [SerializeField]
        private float orderPerUnit = 6f;

        [SerializeField]
        private int orderOffset;

        private void Awake()
        {
            CacheReferences();
            ApplySortingOrder();
        }

        private void LateUpdate()
        {
            ApplySortingOrder();
        }

        public void Configure(SpriteRenderer renderer, Transform origin, int nextBaseOrder, float nextOrderPerUnit, int nextOrderOffset = 0)
        {
            targetRenderer = renderer;
            sortOrigin = origin;
            baseOrder = nextBaseOrder;
            orderPerUnit = Mathf.Max(0.1f, nextOrderPerUnit);
            orderOffset = nextOrderOffset;
            ApplySortingOrder();
        }

        private void CacheReferences()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();

            if (sortOrigin == null)
                sortOrigin = transform;
        }

        private void ApplySortingOrder()
        {
            CacheReferences();

            if (targetRenderer == null || sortOrigin == null)
                return;

            int depthOrder = Mathf.RoundToInt(-sortOrigin.position.y * orderPerUnit);
            targetRenderer.sortingOrder = baseOrder + depthOrder + orderOffset;
        }
    }
}
