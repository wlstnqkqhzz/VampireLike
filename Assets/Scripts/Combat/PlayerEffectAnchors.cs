using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어 루트 좌표가 발밑 기준일 때도 스킬 이펙트가 몸 중심을 따라가도록 기준점을 관리합니다.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class PlayerEffectAnchors : MonoBehaviour
    {
        private const string PlayerVisualName = "PlayerVisual";
        private const string EffectCenterName = "EffectCenter";
        private const string ShieldCenterName = "ShieldCenter";
        private const string OrbitCenterName = "OrbitCenter";

        [SerializeField]
        private float centerHeightRatio = 0.48f;

        [SerializeField]
        private Vector2 effectCenterOffset = new Vector2(0f, 0.02f);

        [SerializeField]
        private Vector2 shieldCenterOffset = new Vector2(0f, 0.05f);

        [SerializeField]
        private Vector2 orbitCenterOffset = new Vector2(0f, 0.04f);

        private SpriteRenderer visualRenderer;

        public Transform EffectCenter { get; private set; }
        public Transform ShieldCenter { get; private set; }
        public Transform OrbitCenter { get; private set; }

        public Vector3 EffectCenterPosition => EffectCenter == null ? GetVisualCenterWorldPosition(effectCenterOffset) : EffectCenter.position;
        public Vector3 ShieldCenterPosition => ShieldCenter == null ? GetVisualCenterWorldPosition(shieldCenterOffset) : ShieldCenter.position;
        public Vector3 OrbitCenterPosition => OrbitCenter == null ? GetVisualCenterWorldPosition(orbitCenterOffset) : OrbitCenter.position;

        private void Awake()
        {
            EnsureAnchors();
            CacheVisualRenderer();
            UpdateAnchorPositions();
        }

        private void LateUpdate()
        {
            UpdateAnchorPositions();
        }

        private void OnValidate()
        {
            centerHeightRatio = Mathf.Clamp01(centerHeightRatio);
        }

        private void EnsureAnchors()
        {
            EffectCenter = GetOrCreateAnchor(EffectCenterName);
            ShieldCenter = GetOrCreateAnchor(ShieldCenterName);
            OrbitCenter = GetOrCreateAnchor(OrbitCenterName);
        }

        private Transform GetOrCreateAnchor(string anchorName)
        {
            Transform anchor = transform.Find(anchorName);

            if (anchor != null)
                return anchor;

            GameObject anchorObject = new GameObject(anchorName);
            anchor = anchorObject.transform;
            anchor.SetParent(transform, false);
            anchor.localRotation = Quaternion.identity;
            anchor.localScale = Vector3.one;
            return anchor;
        }

        private void CacheVisualRenderer()
        {
            Transform visual = transform.Find(PlayerVisualName);

            if (visual != null)
                visualRenderer = visual.GetComponent<SpriteRenderer>();

            if (visualRenderer == null)
                visualRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void UpdateAnchorPositions()
        {
            if (EffectCenter == null || ShieldCenter == null || OrbitCenter == null)
                EnsureAnchors();

            if (visualRenderer == null || visualRenderer.sprite == null)
                CacheVisualRenderer();

            EffectCenter.position = GetVisualCenterWorldPosition(effectCenterOffset);
            ShieldCenter.position = GetVisualCenterWorldPosition(shieldCenterOffset);
            OrbitCenter.position = GetVisualCenterWorldPosition(orbitCenterOffset);
        }

        private Vector3 GetVisualCenterWorldPosition(Vector2 offset)
        {
            if (visualRenderer == null || visualRenderer.sprite == null)
                return transform.position + (Vector3)offset;

            Bounds bounds = visualRenderer.bounds;
            float centerY = Mathf.Lerp(bounds.min.y, bounds.max.y, centerHeightRatio);
            return new Vector3(bounds.center.x + offset.x, centerY + offset.y, transform.position.z);
        }
    }
}
