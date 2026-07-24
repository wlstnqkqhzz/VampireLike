using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// Greed Lord의 탐욕 단계에 따라 색상과 시각 크기만 조절한다.
    /// 실제 충돌 판정은 건드리지 않아 보스가 갑자기 피하기 어려워지는 문제를 막는다.
    /// </summary>
    public class GreedBossVisualController : MonoBehaviour
    {
        [SerializeField]
        private Transform visualRoot;

        [SerializeField]
        private SpriteRenderer baseSpriteRenderer;

        [SerializeField]
        private Color[] phaseColors =
        {
            new Color(0.95f, 0.9f, 0.72f, 1f),
            new Color(1f, 0.82f, 0.35f, 1f),
            new Color(1f, 0.6f, 0.18f, 1f),
            new Color(1f, 0.92f, 0.18f, 1f)
        };

        [SerializeField]
        private float[] phaseVisualScales = { 1f, 1.08f, 1.18f, 1.32f };

        private Vector3 baseScale = Vector3.one;

        private void Awake()
        {
            if (visualRoot == null)
                visualRoot = transform;

            if (baseSpriteRenderer == null)
                baseSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

            baseScale = visualRoot.localScale;
        }

        public void ApplyGreedLevel(int greedLevel)
        {
            int index = Mathf.Clamp(greedLevel - 1, 0, 3);

            if (baseSpriteRenderer != null && phaseColors != null && phaseColors.Length > 0)
                baseSpriteRenderer.color = phaseColors[Mathf.Clamp(index, 0, phaseColors.Length - 1)];

            if (visualRoot != null && visualRoot != transform)
                visualRoot.localScale = baseScale * GetArrayValue(phaseVisualScales, index, 1f);
        }

        private static float GetArrayValue(float[] values, int index, float fallback)
        {
            if (values == null || values.Length == 0)
                return fallback;

            return values[Mathf.Clamp(index, 0, values.Length - 1)];
        }
    }
}
