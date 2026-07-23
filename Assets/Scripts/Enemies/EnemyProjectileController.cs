using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스와 적이 발사하는 플레이어 대상 전용 투사체다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyProjectileController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 4f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private float lifetime = 5f;

        private Rigidbody2D rb;
        private Vector2 direction = Vector2.down;

        public void Initialize(Vector2 moveDirection, float projectileSpeed, int projectileDamage, float projectileLifetime)
        {
            direction = moveDirection.sqrMagnitude <= 0.001f ? Vector2.down : moveDirection.normalized;
            speed = Mathf.Max(0f, projectileSpeed);
            damage = Mathf.Max(1, projectileDamage);
            lifetime = Mathf.Max(0.1f, projectileLifetime);
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0f)
                Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                return;

            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        private void OnValidate()
        {
            speed = Mathf.Max(0f, speed);
            damage = Mathf.Max(1, damage);
            lifetime = Mathf.Max(0.1f, lifetime);
        }
    }
}
