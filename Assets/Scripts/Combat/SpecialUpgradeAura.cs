using UnityEngine;

namespace VampireLike.Combat
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpecialUpgradeAura : MonoBehaviour
    {
        private Transform owner;
        private SpriteRenderer spriteRenderer;
        private float rotateSpeed;
        private float pulseSpeed;
        private float baseScale;
        private float pulseAmount;
        private Color baseColor;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Transform followTarget, Sprite sprite, Color color, float scale, float rotationSpeed, float pulsePerSecond, float pulseSize)
        {
            owner = followTarget;
            rotateSpeed = rotationSpeed;
            pulseSpeed = pulsePerSecond;
            baseScale = Mathf.Max(0.01f, scale);
            pulseAmount = Mathf.Max(0f, pulseSize);
            baseColor = color;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 13;
            transform.localScale = Vector3.one * baseScale;
        }

        private void Update()
        {
            if (owner == null || GameState.IsGameOver)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = owner.position;

            if (Time.timeScale <= 0f)
                return;

            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = Vector3.one * baseScale * pulse;

            if (spriteRenderer != null)
            {
                float alpha = baseColor.a * (0.78f + Mathf.Sin(Time.time * pulseSpeed * 1.3f) * 0.18f);
                spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }
        }
    }
}
