using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어와 일정 거리를 유지하면서 적 전용 투사체를 발사하는 원거리 적 행동이다.
    /// 기본 추적 이동은 EnemyController의 stoppingDistance로 멈추고, 이 스크립트는 발사만 담당한다.
    /// </summary>
    public class RangedEnemyAttack : MonoBehaviour
    {
        [SerializeField]
        private EnemyProjectileController projectilePrefab;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private float attackRange = 5f;

        [SerializeField]
        private float attackInterval = 1.8f;

        [SerializeField]
        private float projectileSpeed = 4f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private float projectileLifetime = 4f;

        private float nextAttackTime;

        private void Awake()
        {
            if (target == null)
                target = GameObject.Find("Player")?.transform;
        }

        private void Update()
        {
            if (target == null || projectilePrefab == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (Time.time < nextAttackTime)
                return;

            Vector2 direction = (target.position - transform.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            if (Vector2.Distance(transform.position, target.position) > attackRange)
                return;

            EnemyProjectileController projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Initialize(direction, projectileSpeed, damage, projectileLifetime);
            nextAttackTime = Time.time + attackInterval;
        }

        private void OnValidate()
        {
            attackRange = Mathf.Max(0.1f, attackRange);
            attackInterval = Mathf.Max(0.1f, attackInterval);
            projectileSpeed = Mathf.Max(0f, projectileSpeed);
            damage = Mathf.Max(1, damage);
            projectileLifetime = Mathf.Max(0.1f, projectileLifetime);
        }
    }
}
