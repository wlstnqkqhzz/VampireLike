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
            const int width = 16;
            const int height = 8;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool blade = x >= y && x >= height - 1 - y;
                    Color color = blade ? new Color(0.85f, 0.95f, 1f, 1f) : Color.clear;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}
