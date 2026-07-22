using UnityEngine;
using System.Collections.Generic;

namespace VampireLike.Combat
{
    /// <summary>
    /// 발사된 투사체의 직선 이동, 수명, 적 충돌 피해, 관통 처리를 담당한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileController : MonoBehaviour
    {
        // 투사체 이동 속도다.
        [SerializeField]
        private float moveSpeed = 8f;

        // 기본 피해량이다. 발사 시 공격력 배율을 곱해 실제 피해량을 계산한다.
        [SerializeField]
        private int damage = 1;

        // 충돌하지 않아도 자동 제거되는 시간이다.
        [SerializeField]
        private float lifeTime = 3f;

        private Rigidbody2D rb;
        private Vector2 moveDirection = Vector2.right;
        private float lifeTimer;
        private int effectiveDamage;
        private int remainingPierceCount;
        private readonly HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

        private void Awake()
        {
            // 투사체는 중력 없이 회전 고정 상태로 물리 이동한다.
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
            effectiveDamage = damage;
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
            // 적과 충돌했을 때만 피해를 주고, 같은 적을 중복 타격하지 않게 막는다.
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth == null)
                return;

            if (hitEnemies.Contains(enemyHealth))
                return;

            hitEnemies.Add(enemyHealth);
            enemyHealth.TakeDamage(effectiveDamage);

            if (remainingPierceCount > 0)
            {
                remainingPierceCount--;
                return;
            }

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
            Launch(direction, 1f, 0);
        }

        /// <summary>
        /// 발사 순간의 방향, 공격력 배율, 관통 횟수를 설정한다.
        /// </summary>
        public void Launch(Vector2 direction, float damageMultiplier, int pierceCount)
        {
            if (direction.sqrMagnitude <= 0f)
                return;

            moveDirection = direction.normalized;
            transform.right = moveDirection;
            effectiveDamage = Mathf.Max(1, Mathf.RoundToInt(damage * Mathf.Max(0.1f, damageMultiplier)));
            remainingPierceCount = Mathf.Max(0, pierceCount);
            hitEnemies.Clear();
        }
    }
}
