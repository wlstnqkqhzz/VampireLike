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
        private static Sprite filledCircleSprite;
        private static Sprite squareSprite;
        private static Sprite diamondSprite;
        private static Sprite starSprite;
        private static Sprite webSprite;
        private static Sprite coneSprite;
        private SpriteRenderer spriteRenderer;
        private float rotateSpeed;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(float duration)
        {
            StartCoroutine(Animate(Mathf.Max(0.05f, duration)));
        }

        public void Play(float duration, float rotationSpeed)
        {
            rotateSpeed = rotationSpeed;
            Play(duration);
        }

        public static Sprite GetCircleSprite()
        {
            if (circleSprite != null)
                return circleSprite;

            const int size = 96;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
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

        public static Sprite GetFilledCircleSprite()
        {
            if (filledCircleSprite != null)
                return filledCircleSprite;

            const int size = 96;
            Texture2D texture = CreateTransparentTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.44f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            filledCircleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return filledCircleSprite;
        }

        public static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
                return squareSprite;

            const int size = 16;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
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

        public static Sprite GetDiamondSprite()
        {
            if (diamondSprite != null)
                return diamondSprite;

            const int size = 64;
            Texture2D texture = CreateTransparentTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.38f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Mathf.Abs(x - center.x) + Mathf.Abs(y - center.y);
                    bool filled = distance <= radius;
                    bool edge = distance > radius - 2f && distance <= radius + 0.5f;
                    texture.SetPixel(x, y, filled || edge ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            diamondSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return diamondSprite;
        }

        public static Sprite GetStarSprite()
        {
            if (starSprite != null)
                return starSprite;

            const int size = 96;
            Texture2D texture = CreateTransparentTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    float dy = Mathf.Abs(y - center.y);
                    bool cross = dx < 2f || dy < 2f;
                    bool diagonal = Mathf.Abs(dx - dy) < 1.4f;
                    bool nearCenter = Vector2.Distance(new Vector2(x, y), center) < size * 0.32f;
                    texture.SetPixel(x, y, nearCenter && (cross || diagonal) ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            starSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return starSprite;
        }

        public static Sprite GetWebSprite()
        {
            if (webSprite != null)
                return webSprite;

            const int size = 64;
            Texture2D texture = CreateTransparentTexture(size, size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distance = Vector2.Distance(point, center);
                    float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
                    bool ring = Mathf.Abs(distance - 12f) < 0.8f || Mathf.Abs(distance - 21f) < 0.8f || Mathf.Abs(distance - 29f) < 0.8f;
                    bool spoke = Mathf.Abs(Mathf.Sin(angle * 4f)) < 0.045f && distance < 30f;
                    bool inside = distance < 30f;
                    texture.SetPixel(x, y, inside && (ring || spoke) ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            webSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return webSprite;
        }

        public static Sprite GetConeSprite()
        {
            if (coneSprite != null)
                return coneSprite;

            const int width = 128;
            const int height = 96;
            Texture2D texture = CreateTransparentTexture(width, height);
            Vector2 origin = new Vector2(4f, height * 0.5f);
            float maxDistance = width - 6f;
            float halfAngle = 32f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 direction = new Vector2(x - origin.x, y - origin.y);
                    float distance = direction.magnitude;
                    float angle = Vector2.Angle(Vector2.right, direction);
                    bool inside = distance <= maxDistance && angle <= halfAngle;
                    bool edge = inside && (Mathf.Abs(angle - halfAngle) < 1.8f || Mathf.Abs(distance - maxDistance) < 1.5f);
                    texture.SetPixel(x, y, edge || (inside && x % 7 == 0) ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            coneSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.08f, 0.5f), width);
            return coneSprite;
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
                transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, progress));
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private static Texture2D CreateTransparentTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    texture.SetPixel(x, y, Color.clear);
            }

            return texture;
        }
    }
}
