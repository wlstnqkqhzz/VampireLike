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

        [SerializeField]
        private float minimumAttackInterval = 0.15f;

        [SerializeField]
        private float projectileDamageMultiplier = 1f;

        [SerializeField]
        private int projectileCount = 1;

        [SerializeField]
        private float projectileSpreadAngle = 12f;

        [SerializeField]
        private int projectilePierceCount;

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
            minimumAttackInterval = Mathf.Max(0.05f, minimumAttackInterval);
            attackInterval = Mathf.Max(minimumAttackInterval, attackInterval);
            attackRange = Mathf.Max(0f, attackRange);
            projectileDamageMultiplier = Mathf.Max(0.1f, projectileDamageMultiplier);
            projectileCount = Mathf.Max(1, projectileCount);
            projectileSpreadAngle = Mathf.Max(0f, projectileSpreadAngle);
            projectilePierceCount = Mathf.Max(0, projectilePierceCount);
        }

        public void StopAttacking()
        {
            isStopped = true;
        }

        public void MultiplyAttackInterval(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            attackInterval = Mathf.Max(minimumAttackInterval, attackInterval * multiplier);
        }

        public void MultiplyProjectileDamage(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            projectileDamageMultiplier *= multiplier;
        }

        public void AddProjectileCount(int amount)
        {
            projectileCount = Mathf.Max(1, projectileCount + amount);
        }

        public void AddProjectilePierceCount(int amount)
        {
            projectilePierceCount = Mathf.Max(0, projectilePierceCount + amount);
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

            int shotCount = Mathf.Max(1, projectileCount);
            float firstAngle = shotCount == 1 ? 0f : -projectileSpreadAngle * (shotCount - 1) * 0.5f;

            for (int i = 0; i < shotCount; i++)
            {
                float angle = firstAngle + projectileSpreadAngle * i;
                Vector2 shotDirection = Rotate(direction, angle);
                ProjectileController projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                projectile.Launch(shotDirection, projectileDamageMultiplier, projectilePierceCount);
            }
        }

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }
    }
}
