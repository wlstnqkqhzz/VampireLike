using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 패턴이 실행되기 전에 공격 범위를 짧게 보여주는 공통 경고 표시 도구입니다.
    /// 실제 패턴 로직과 분리해서 Dash, Shockwave, Cone 계열 패턴이 재사용할 수 있게 합니다.
    /// </summary>
    public static class BossTelegraph
    {
        private static Sprite squareSprite;
        private static Sprite circleSprite;
        private static Material meshMaterial;

        /// <summary>
        /// 돌진처럼 방향이 정해진 공격의 진행 경로를 얇은 선으로 표시합니다.
        /// </summary>
        public static GameObject ShowLine(Vector2 start, Vector2 direction, float length, float width, float duration, Color color, int sortingOrder = 1450)
        {
            if (duration <= 0f || length <= 0f || width <= 0f)
                return null;

            Vector2 normalizedDirection = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;
            GameObject telegraph = CreateSpriteObject("Boss Line Telegraph", GetSquareSprite(), color, sortingOrder);
            telegraph.transform.position = start + normalizedDirection * (length * 0.5f);
            telegraph.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg);
            telegraph.transform.localScale = new Vector3(length, width, 1f);
            telegraph.AddComponent<BossTelegraphVisual>().Initialize(duration, color, 0.05f);
            return telegraph;
        }

        /// <summary>
        /// 충격파나 지정 위치 폭발처럼 원형 범위 공격의 반경을 표시합니다.
        /// </summary>
        public static GameObject ShowCircle(Vector2 center, float radius, float duration, Color color, int sortingOrder = 1450)
        {
            if (duration <= 0f || radius <= 0f)
                return null;

            GameObject telegraph = CreateSpriteObject("Boss Circle Telegraph", GetCircleSprite(), color, sortingOrder);
            telegraph.transform.position = center;
            telegraph.transform.localScale = Vector3.one * radius * 2f;
            telegraph.AddComponent<BossTelegraphVisual>().Initialize(duration, color, 0.04f);
            return telegraph;
        }

        /// <summary>
        /// 전방 베기나 브레스처럼 부채꼴 범위 공격의 위험 영역을 표시합니다.
        /// </summary>
        public static GameObject ShowCone(Vector2 origin, Vector2 direction, float radius, float angle, float duration, Color color, int sortingOrder = 1450)
        {
            if (duration <= 0f || radius <= 0f || angle <= 0f)
                return null;

            Vector2 normalizedDirection = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;
            GameObject telegraph = new GameObject("Boss Cone Telegraph");
            telegraph.transform.position = origin;

            MeshFilter meshFilter = telegraph.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = telegraph.AddComponent<MeshRenderer>();
            meshRenderer.material = GetMeshMaterial();
            meshRenderer.material.color = color;
            meshRenderer.sortingOrder = sortingOrder;
            meshFilter.mesh = CreateConeMesh(normalizedDirection, radius, angle, 24);

            telegraph.AddComponent<BossTelegraphVisual>().Initialize(duration, color, 0.035f);
            return telegraph;
        }

        private static GameObject CreateSpriteObject(string name, Sprite sprite, Color color, int sortingOrder)
        {
            GameObject telegraph = new GameObject(name);
            SpriteRenderer renderer = telegraph.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingLayerName = "Default";
            renderer.sortingOrder = sortingOrder;
            return telegraph;
        }

        private static Sprite GetSquareSprite()
        {
            if (squareSprite != null)
                return squareSprite;

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.name = "BossTelegraphSquare";
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            squareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return squareSprite;
        }

        private static Sprite GetCircleSprite()
        {
            if (circleSprite != null)
                return circleSprite;

            const int size = 128;
            const float edgeStart = 0.72f;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "BossTelegraphCircle";
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    float edge = Mathf.Clamp01((distance - edgeStart) / (1f - edgeStart));
                    float alpha = distance <= 1f ? Mathf.Lerp(0.12f, 1f, edge) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            circleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return circleSprite;
        }

        private static Material GetMeshMaterial()
        {
            if (meshMaterial != null)
                return meshMaterial;

            meshMaterial = new Material(Shader.Find("Sprites/Default"));
            return meshMaterial;
        }

        private static Mesh CreateConeMesh(Vector2 direction, float radius, float angle, int segments)
        {
            Mesh mesh = new Mesh { name = "BossConeTelegraphMesh" };
            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 3];
            vertices[0] = Vector3.zero;

            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float halfAngle = angle * 0.5f;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = baseAngle - halfAngle + angle * i / segments;
                Vector2 point = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                vertices[i + 1] = point * radius;

                if (i >= segments)
                    continue;

                int triangleIndex = i * 3;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            return mesh;
        }
    }

    /// <summary>
    /// 경고 표시가 준비 시간 동안 밝아지고 사라지도록 제어하는 가벼운 런타임 컴포넌트입니다.
    /// </summary>
    public class BossTelegraphVisual : MonoBehaviour
    {
        private float duration;
        private float elapsedTime;
        private float pulseStrength;
        private Color baseColor;
        private Vector3 baseScale;
        private SpriteRenderer spriteRenderer;
        private MeshRenderer meshRenderer;

        public void Initialize(float telegraphDuration, Color color, float pulse)
        {
            duration = Mathf.Max(0.01f, telegraphDuration);
            baseColor = color;
            pulseStrength = Mathf.Max(0f, pulse);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();
            baseScale = transform.localScale;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float brightness = Mathf.Lerp(0.45f, 1.05f, progress);
            float pulse = 1f + Mathf.Sin(progress * Mathf.PI * 6f) * pulseStrength;
            Color currentColor = baseColor;
            currentColor.a = Mathf.Lerp(baseColor.a * 0.55f, baseColor.a, progress);
            currentColor *= brightness;
            currentColor.a = Mathf.Clamp01(currentColor.a);

            if (spriteRenderer != null)
                spriteRenderer.color = currentColor;

            if (meshRenderer != null)
                meshRenderer.material.color = currentColor;

            transform.localScale = baseScale * pulse;

            if (elapsedTime >= duration)
                Destroy(gameObject);
        }
    }
}
