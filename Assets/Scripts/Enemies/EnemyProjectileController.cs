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

        private Rigidbody2D rb;
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

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
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
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

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
