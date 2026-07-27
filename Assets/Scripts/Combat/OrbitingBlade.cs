using System.Collections.Generic;
using UnityEngine;

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

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBladeSprite();
            spriteRenderer.color = new Color(0.86f, 0.95f, 1f, 0.95f);
            spriteRenderer.sortingOrder = 12;
        }

        private void Update()
        {
            if (owner == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            angle += rotateSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * radius;
            transform.position = owner.position + offset;
            transform.right = new Vector2(-Mathf.Sin(radians), Mathf.Cos(radians));

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
            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, 0.18f, hitResults, enemyLayerMask);

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
                nextDamageTimes[enemy] = Time.time + damageInterval;
            }
        }

        private static Sprite CreateBladeSprite()
        {
            const int width = 24;
            const int height = 10;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedX = x / (float)(width - 1);
                    float centerY = (height - 1) * 0.5f + Mathf.Sin(normalizedX * Mathf.PI) * 1.5f;
                    float thickness = Mathf.Lerp(1.2f, 3.2f, Mathf.Sin(normalizedX * Mathf.PI));
                    bool blade = Mathf.Abs(y - centerY) <= thickness && x > 1 && x < width - 1;
                    bool edge = blade && Mathf.Abs(y - centerY) > thickness - 1.1f;
                    bool hilt = x <= 3 && Mathf.Abs(y - (height - 1) * 0.5f) <= 2f;
                    Color color = Color.clear;

                    if (blade)
                        color = edge ? new Color(0.45f, 0.86f, 1f, 1f) : new Color(0.9f, 0.98f, 1f, 1f);

                    if (hilt)
                        color = new Color(0.45f, 0.28f, 0.9f, 1f);

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 18f);
        }
    }
}
