using UnityEngine;

namespace VampireLike.VFX
{
    public class CombatVFXSettings : MonoBehaviour
    {
        [SerializeField]
        private float sizeMultiplier = 1f;

        [SerializeField]
        private float durationMultiplier = 1f;

        [SerializeField]
        private float alphaMultiplier = 1f;

        [SerializeField]
        private int sortingOrderOffset;

        [SerializeField]
        private bool enableProjectileTrails = true;

        [SerializeField]
        private float trailWidthMultiplier = 1f;

        public float SizeMultiplier => Mathf.Max(0.1f, sizeMultiplier);
        public float DurationMultiplier => Mathf.Max(0.1f, durationMultiplier);
        public float AlphaMultiplier => Mathf.Clamp(alphaMultiplier, 0.1f, 1.5f);
        public int SortingOrderOffset => sortingOrderOffset;
        public bool EnableProjectileTrails => enableProjectileTrails;
        public float TrailWidthMultiplier => Mathf.Max(0.1f, trailWidthMultiplier);
    }
}
