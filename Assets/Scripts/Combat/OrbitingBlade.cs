using System.Collections.Generic;
using UnityEngine;
using VampireLike.VFX;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어 주변을 회전하며 닿은 적에게 주기적으로 피해를 주는 특수 강화 투사체다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class OrbitingBlade : MonoBehaviour
    {
        private readonly Collider2D[] hitResults = new Collider2D[16];
        private readonly Dictionary<EnemyHealth, float> nextDamageTimes = new Dictionary<EnemyHealth, float>();
        private Transform owner;
        private LayerMask enemyLayerMask;
        private float radius = 1f;
        private float angle;
        private float rotateSpeed = 180f;
        private float damageInterval = 0.35f;
        private int damage = 1;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private SpriteRenderer arcRenderer;
        private Vector2 orbitDirection = Vector2.right;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBladeSprite();
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 16;
            transform.localScale = Vector3.one * 0.58f;

            glowRenderer = CreateGlowRenderer();
            arcRenderer = CreateArcRenderer();
        }

        private void Update()
        {
            if (owner == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            angle += rotateSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;
            orbitDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            transform.position = owner.position + (Vector3)(orbitDirection * radius);
            transform.right = orbitDirection;
            AnimateBladeVFX();

            DamageNearbyEnemies();
        }

        public void Configure(Transform ownerTransform, float orbitRadius, float speed, float startAngle, int bladeDamage, float hitInterval, LayerMask targetLayerMask)
        {
            owner = ownerTransform;
            radius = Mathf.Max(0.2f, orbitRadius);
            rotateSpeed = speed;
            angle = startAngle;
            damage = Mathf.Max(1, bladeDamage);
            damageInterval = Mathf.Max(0.05f, hitInterval);
            enemyLayerMask = targetLayerMask;
        }

        private void DamageNearbyEnemies()
        {
            Vector2 hitCenter = (Vector2)transform.position + orbitDirection * 0.26f;
            int hitCount = Physics2D.OverlapCircleNonAlloc(hitCenter, 0.2f, hitResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = hitResults[i];

                if (hit == null)
                    continue;

                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

                if (enemy == null || enemy.IsDead)
                    continue;

                if (nextDamageTimes.TryGetValue(enemy, out float nextTime) && Time.time < nextTime)
                    continue;

                enemy.TakeDamage(damage);
                Vector3 hitPosition = hitCenter;
                CombatVFX.PlayBurst(hitPosition, CombatVFXKind.ArcaneImpact, 0.32f, 0.14f);
                nextDamageTimes[enemy] = Time.time + damageInterval;
            }
        }

        private SpriteRenderer CreateGlowRenderer()
        {
            GameObject glow = new GameObject("Rotating Blade Aura");
            glow.transform.SetParent(transform, false);
            SpriteRenderer renderer = glow.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateBladeGlowSprite();
            renderer.color = new Color(0.58f, 0.86f, 1f, 0.26f);
            renderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            renderer.transform.localScale = Vector3.one * 0.96f;
            return renderer;
        }

        private SpriteRenderer CreateArcRenderer()
        {
            GameObject arc = new GameObject("Rotating Blade Slash Arc");
            arc.transform.SetParent(transform, false);
            SpriteRenderer renderer = arc.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSlashArcSprite();
            renderer.color = new Color(0.42f, 0.78f, 1f, 0.2f);
            renderer.sortingOrder = spriteRenderer.sortingOrder - 2;
            renderer.transform.localPosition = new Vector3(-0.08f, 0f, 0f);
            renderer.transform.localScale = Vector3.one;
            return renderer;
        }

        private void AnimateBladeVFX()
        {
            if (glowRenderer == null)
                return;

            float pulse = 0.22f + Mathf.Sin(Time.time * 7f + angle * 0.04f) * 0.06f;
            glowRenderer.color = new Color(0.58f, 0.86f, 1f, pulse);

            if (arcRenderer == null)
                return;

            float arcAlpha = 0.14f + Mathf.Sin(Time.time * 8f + angle * 0.03f) * 0.04f;
            arcRenderer.color = new Color(0.42f, 0.78f, 1f, arcAlpha);
        }

        private static Sprite CreateBladeSprite()
        {
            const int width = 92;
            const int height = 30;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            Vector2 guardCenter = new Vector2(18f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedX = x / (float)(width - 1);
                    float bladeProgress = Mathf.Clamp01((x - 20f) / (width - 25f));
                    float centerY = (height - 1) * 0.5f - Mathf.Sin(bladeProgress * Mathf.PI) * 2.2f;
                    float baseWidth = Mathf.Lerp(4.8f, 0.2f, Mathf.Pow(bladeProgress, 1.45f));
                    float tipTaper = Mathf.Clamp01((width - 2f - x) / 10f);
                    float halfWidth = baseWidth * tipTaper;
                    float edgeDistance = Mathf.Abs(y - centerY);
                    bool blade = x >= 20 && x < width - 2 && edgeDistance <= halfWidth;
                    bool brightEdge = blade && edgeDistance > halfWidth * 0.55f;
                    bool centerRidge = blade && edgeDistance < 0.85f && bladeProgress > 0.05f && bladeProgress < 0.88f;
                    bool magicEdge = blade && edgeDistance > halfWidth * 0.78f && bladeProgress > 0.16f;
                    bool pointGlow = bladeProgress > 0.84f && edgeDistance <= halfWidth + 1f && x < width - 2;
                    bool guard = Vector2.Distance(new Vector2(x, y), guardCenter) < 4.2f
                        || (x >= 14 && x <= 22 && Mathf.Abs(y - height * 0.5f) < 2.4f)
                        || (x >= 16 && x <= 20 && Mathf.Abs(y - height * 0.5f) < 7.5f);
                    bool grip = x >= 3 && x < 17 && Mathf.Abs(y - height * 0.5f) < 2.6f;
                    bool pommel = Vector2.Distance(new Vector2(x, y), new Vector2(4f, height * 0.5f)) < 3.4f;
                    Color color = Color.clear;

                    if (magicEdge)
                        color = new Color(0.5f, 0.9f, 1f, 0.68f);

                    if (blade)
                    {
                        if (centerRidge)
                            color = new Color(0.96f, 0.97f, 1f, 1f);
                        else if (brightEdge)
                            color = new Color(0.78f, 0.9f, 1f, 1f);
                        else if (y > centerY)
                            color = new Color(0.21f, 0.28f, 0.38f, 1f);
                        else
                            color = new Color(0.52f, 0.65f, 0.78f, 1f);
                    }

                    if (pointGlow)
                        color = Color.Lerp(color, new Color(0.72f, 0.96f, 1f, 1f), 0.35f);

                    if (guard)
                        color = new Color(0.14f, 0.24f, 0.34f, 1f);

                    if (grip)
                        color = new Color(0.08f, 0.1f, 0.13f, 1f);

                    if (pommel)
                        color = new Color(0.5f, 0.82f, 0.92f, 1f);

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.2f, 0.5f), 96f);
        }

        private static Sprite CreateBladeGlowSprite()
        {
            const int width = 104;
            const int height = 40;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float bladeProgress = Mathf.Clamp01((x - 22f) / (width - 28f));
                    float centerY = (height - 1) * 0.5f - Mathf.Sin(bladeProgress * Mathf.PI) * 2.6f;
                    float baseWidth = Mathf.Lerp(5.8f, 0.2f, Mathf.Pow(bladeProgress, 1.45f));
                    float tipTaper = Mathf.Clamp01((width - 4f - x) / 11f);
                    float halfWidth = baseWidth * tipTaper;
                    float distance = Mathf.Abs(y - centerY);
                    float alpha = distance <= halfWidth + 2f && x > 18 && x < width - 4
                        ? Mathf.Clamp01(1f - distance / (halfWidth + 2f)) * Mathf.Sin(bladeProgress * Mathf.PI) * 0.3f
                        : 0f;

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.2f, 0.5f), 96f);
        }

        private static Sprite CreateSlashArcSprite()
        {
            const int size = 72;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y) - center;
                    float radius = point.magnitude / (size * 0.5f);
                    float angle = Mathf.Repeat(Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg + 360f, 360f);
                    bool arc = radius > 0.58f && radius < 0.78f && angle > 205f && angle < 330f;
                    float distance = Mathf.Abs(radius - 0.68f);
                    float angleFade = Mathf.Sin(Mathf.InverseLerp(205f, 330f, angle) * Mathf.PI);
                    float alpha = arc ? Mathf.Clamp01(1f - distance / 0.1f) * angleFade * 0.45f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 96f);
        }
    }
}
