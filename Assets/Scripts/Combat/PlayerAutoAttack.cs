using UnityEngine;

namespace VampireLike.Combat
{
    public class PlayerAutoAttack : MonoBehaviour
    {
        [SerializeField]
        private ProjectileController projectilePrefab;

        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private float attackInterval = 1f;

        [SerializeField]
        private float attackRange = 6f;

        private float attackTimer;
        private bool isStopped;

        private void Awake()
        {
            if (firePoint == null)
                firePoint = transform;
        }

        private void Update()
        {
            if (isStopped || GameState.IsGameOver || projectilePrefab == null)
                return;

            attackTimer += Time.deltaTime;

            if (attackTimer < attackInterval)
                return;

            EnemyHealth target = FindClosestEnemyInRange();

            if (target == null)
                return;

            attackTimer = 0f;
            FireAt(target.transform);
        }

        private void OnValidate()
        {
            attackInterval = Mathf.Max(0.05f, attackInterval);
            attackRange = Mathf.Max(0f, attackRange);
        }

        public void StopAttacking()
        {
            isStopped = true;
        }

        private EnemyHealth FindClosestEnemyInRange()
        {
            EnemyHealth closestEnemy = null;
            float closestSqrDistance = attackRange * attackRange;
            Vector2 origin = transform.position;

            foreach (EnemyHealth enemy in EnemyHealth.ActiveEnemies)
            {
                if (enemy == null || enemy.IsDead)
                    continue;

                float sqrDistance = ((Vector2)enemy.transform.position - origin).sqrMagnitude;

                if (sqrDistance > closestSqrDistance)
                    continue;

                closestEnemy = enemy;
                closestSqrDistance = sqrDistance;
            }

            return closestEnemy;
        }

        private void FireAt(Transform target)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)firePoint.position).normalized;

            if (direction.sqrMagnitude <= 0f)
                return;

            ProjectileController projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.Launch(direction);
        }
    }
}
