using UnityEngine;
using VampireLike.Combat;
using VampireLike.VFX;

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

        [SerializeField]
        private LayerMask playerLayerMask = 1 << 6;

        [SerializeField]
        private float visibleHitboxCoverage = 0.72f;

        private Rigidbody2D rb;
        private Collider2D projectileCollider;
        private SpriteRenderer spriteRenderer;
        private PlayerHealth playerHealth;
        private Vector2 direction = Vector2.down;
        private Transform homingTarget;
        private float homingDuration;
        private float turnSpeed;
        private float homingElapsedTime;
        private bool useHoming;
        private bool isDestroying;

        public void Initialize(Vector2 moveDirection, float projectileSpeed, int projectileDamage, float projectileLifetime)
        {
            direction = moveDirection.sqrMagnitude <= 0.001f ? Vector2.down : moveDirection.normalized;
            speed = Mathf.Max(0f, projectileSpeed);
            damage = Mathf.Max(1, projectileDamage);
            lifetime = Mathf.Max(0.1f, projectileLifetime);
            useHoming = false;
            homingTarget = null;
        }

        /// <summary>
        /// 워록 같은 보스가 사용하는 유도 투사체를 초기화한다.
        /// </summary>
        public void InitializeHoming(Transform target, Vector2 initialDirection, float projectileSpeed, int projectileDamage, float projectileLifetime, float duration, float rotationSpeed)
        {
            Initialize(initialDirection, projectileSpeed, projectileDamage, projectileLifetime);
            homingTarget = target;
            homingDuration = Mathf.Max(0f, duration);
            turnSpeed = Mathf.Max(0f, rotationSpeed);
            homingElapsedTime = 0f;
            useHoming = target != null && homingDuration > 0f && turnSpeed > 0f;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            CombatVFX.AttachTrail(gameObject, CombatVFXKind.Explosion, 0.07f, 0.18f);
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0f)
                DestroyProjectile(true);
        }

        private void FixedUpdate()
        {
            UpdateHomingDirection();
            Vector2 startPosition = rb.position;
            Vector2 nextPosition = startPosition + direction * speed * Time.fixedDeltaTime;

            if (TrySweepDamagePlayer(startPosition, nextPosition))
                return;

            rb.MovePosition(nextPosition);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamagePlayer(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision == null || collision.collider == null)
                return;

            TryDamagePlayer(collision.collider);
        }

        private bool TrySweepDamagePlayer(Vector2 startPosition, Vector2 nextPosition)
        {
            if (isDestroying)
                return true;

            Vector2 delta = nextPosition - startPosition;
            float distance = delta.magnitude;

            if (distance <= 0.0001f)
                return false;

            float sweepRadius = GetSweepRadius();

            if (TryDamagePlayerByHurtbox(startPosition, nextPosition, sweepRadius))
                return true;

            RaycastHit2D hit = Physics2D.CircleCast(
                startPosition,
                sweepRadius,
                delta.normalized,
                distance,
                playerLayerMask);

            if (hit.collider == null)
                return false;

            TryDamagePlayer(hit.collider);
            return isDestroying;
        }

        private float GetSweepRadius()
        {
            float radius = 0.08f;

            if (projectileCollider is CircleCollider2D circleCollider)
                radius = Mathf.Max(radius, circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y));

            if (projectileCollider != null)
                radius = Mathf.Max(radius, Mathf.Min(projectileCollider.bounds.extents.x, projectileCollider.bounds.extents.y));

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                radius = Mathf.Max(radius, Mathf.Max(spriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y) * visibleHitboxCoverage);

            return radius;
        }

        private bool TryDamagePlayerByHurtbox(Vector2 startPosition, Vector2 nextPosition, float sweepRadius)
        {
            if (playerHealth == null)
                playerHealth = FindAnyObjectByType<PlayerHealth>();

            if (playerHealth == null || !playerHealth.IsHitByProjectileSweep(startPosition, nextPosition, sweepRadius))
                return false;

            TryDamagePlayer(playerHealth);
            return isDestroying;
        }

        private void TryDamagePlayer(Component hit)
        {
            if (isDestroying || hit == null)
                return;

            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                return;

            playerHealth.TakeDamage(damage);
            CombatVFX.PlayBurst(transform.position, CombatVFXKind.Explosion, 0.48f, 0.2f);
            DestroyProjectile(false);
        }

        private void OnValidate()
        {
            speed = Mathf.Max(0f, speed);
            damage = Mathf.Max(1, damage);
            lifetime = Mathf.Max(0.1f, lifetime);
            visibleHitboxCoverage = Mathf.Clamp(visibleHitboxCoverage, 0.1f, 1f);
        }

        private void UpdateHomingDirection()
        {
            if (!useHoming || homingTarget == null || homingElapsedTime >= homingDuration)
                return;

            Vector2 toTarget = ((Vector2)homingTarget.position - rb.position).normalized;

            if (toTarget.sqrMagnitude <= 0.001f)
                return;

            float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            float nextAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.fixedDeltaTime);
            direction = new Vector2(Mathf.Cos(nextAngle * Mathf.Deg2Rad), Mathf.Sin(nextAngle * Mathf.Deg2Rad));
            homingElapsedTime += Time.fixedDeltaTime;
        }

        private void DestroyProjectile(bool playFadeEffect)
        {
            if (isDestroying)
                return;

            isDestroying = true;

            if (playFadeEffect)
                CombatVFX.PlayBurst(transform.position, CombatVFXKind.ArcaneImpact, 0.32f, 0.14f);

            Destroy(gameObject);
        }
    }
}
