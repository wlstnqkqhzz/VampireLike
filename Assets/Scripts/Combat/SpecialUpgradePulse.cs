using System.Collections;
using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 특수 강화의 임시 원형 이펙트를 짧게 커지고 사라지게 만든다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpecialUpgradePulse : MonoBehaviour
    {
        private static Sprite circleSprite;
        private static Sprite squareSprite;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(float duration)
        {
            StartCoroutine(Animate(Mathf.Max(0.05f, duration)));
        }

        public static Sprite GetCircleSprite()
        {
            if (circleSprite != null)
                return circleSprite;

            const int size = 32;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float outerRadius = size * 0.46f;
            float innerRadius = size * 0.28f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    bool isRing = distance <= outerRadius && distance >= innerRadius;
                    texture.SetPixel(x, y, isRing ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return circleSprite;
        }

        public static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
                return squareSprite;

            const int size = 8;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                    texture.SetPixel(x, y, Color.white);
            }

            texture.Apply();
            squareSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return squareSprite;
        }

        private IEnumerator Animate(float duration)
        {
            Vector3 startScale = transform.localScale * 0.35f;
            Vector3 endScale = transform.localScale;
            Color startColor = spriteRenderer.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, endScale, progress);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, progress));
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
