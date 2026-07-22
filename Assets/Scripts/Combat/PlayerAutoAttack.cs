using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 일정 시간마다 공격 범위 안의 가장 가까운 적을 찾아 투사체를 발사한다.
    /// </summary>
    public class PlayerAutoAttack : MonoBehaviour
    {
        // 발사할 투사체 프리팹이다.
        [SerializeField]
        private ProjectileController projectilePrefab;

        // 발사 위치다. 비어 있으면 플레이어 위치를 사용한다.
        [SerializeField]
        private Transform firePoint;

        // 공격 사이의 대기 시간이다. 공격 속도 강화는 이 값을 줄인다.
        [SerializeField]
        private float attackInterval = 1f;

        // 가장 가까운 적을 찾을 최대 거리다.
        [SerializeField]
        private float attackRange = 6f;

        // 공격 속도 강화가 누적되어도 이 값보다 빠르게는 공격하지 않는다.
        [SerializeField]
        private float minimumAttackInterval = 0.15f;

        // 투사체 피해 강화로 누적되는 공격력 배율이다.
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

        /// <summary>
        /// 공격 간격 강화에서 호출한다. 예: 0.88을 곱하면 공격 간격이 12% 줄어든다.
        /// </summary>
        public void MultiplyAttackInterval(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            attackInterval = Mathf.Max(minimumAttackInterval, attackInterval * multiplier);
        }

        /// <summary>
        /// 투사체 공격력 강화에서 호출한다.
        /// </summary>
        public void MultiplyProjectileDamage(float multiplier)
        {
            if (multiplier <= 0f)
                return;

            projectileDamageMultiplier *= multiplier;
        }

        /// <summary>
        /// 다중 발사 강화에서 호출한다.
        /// </summary>
        public void AddProjectileCount(int amount)
        {
            projectileCount = Mathf.Max(1, projectileCount + amount);
        }

        /// <summary>
        /// 관통탄 강화에서 호출한다.
        /// </summary>
        public void AddProjectilePierceCount(int amount)
        {
            projectilePierceCount = Mathf.Max(0, projectilePierceCount + amount);
        }

        private EnemyHealth FindClosestEnemyInRange()
        {
            // EnemyHealth.ActiveEnemies를 순회해 매 프레임 FindObject 계열 호출을 피한다.
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
            // 발사 시점의 방향을 기준으로 직선 투사체를 만든다. 유도탄은 아니다.
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
