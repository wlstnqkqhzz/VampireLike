using UnityEngine;

namespace VampireLike.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 8f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private float lifeTime = 3f;

        private Rigidbody2D rb;
        private Vector2 moveDirection = Vector2.right;
        private float lifeTimer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
        }

        private void FixedUpdate()
        {
            if (GameState.IsGameOver)
                return;

            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (GameState.IsGameOver)
                return;

            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth == null)
                return;

            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            damage = Mathf.Max(1, damage);
            lifeTime = Mathf.Max(0.1f, lifeTime);
        }

        public void Launch(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0f)
                return;

            moveDirection = direction.normalized;
            transform.right = moveDirection;
        }
    }
}
