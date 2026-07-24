using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어를 일정 시간만 추적하는 적 투사체를 생성하는 워록 계열 패턴이다.
    /// </summary>
    public class HomingOrbPattern : BossPattern
    {
        protected override bool UseSkillAnimation => true;

        [SerializeField]
        private EnemyProjectileController projectilePrefab;

        [SerializeField]
        private int orbCount = 1;

        [SerializeField]
        private int phaseBonusOrbCount = 1;

        [SerializeField]
        private float spawnInterval = 0.18f;

        [SerializeField]
        private float projectileSpeed = 2.7f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private float projectileLifetime = 5f;

        [SerializeField]
        private float homingDuration = 2.2f;

        [SerializeField]
        private float turnSpeed = 120f;

        [SerializeField]
        private float spawnRadius = 0.35f;

        protected override bool CanExecutePattern()
        {
            return projectilePrefab != null && Player != null;
        }

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);
            int count = orbCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusOrbCount;

            for (int i = 0; i < count && !Boss.IsDead; i++)
            {
                Vector2 direction = ((Vector2)Player.position - (Vector2)transform.position).normalized;

                if (direction.sqrMagnitude <= 0.001f)
                    direction = Vector2.down;

                Vector2 offset = Quaternion.Euler(0f, 0f, i * 360f / Mathf.Max(1, count)) * Vector2.right * spawnRadius;
                EnemyProjectileController projectile = Instantiate(projectilePrefab, (Vector2)transform.position + offset, Quaternion.identity);
                projectile.InitializeHoming(Player, direction, projectileSpeed, damage, projectileLifetime, homingDuration, turnSpeed);

                if (spawnInterval > 0f)
                    yield return new WaitForSeconds(spawnInterval);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            orbCount = Mathf.Max(1, orbCount);
            phaseBonusOrbCount = Mathf.Max(0, phaseBonusOrbCount);
            spawnInterval = Mathf.Max(0f, spawnInterval);
            projectileSpeed = Mathf.Max(0f, projectileSpeed);
            damage = Mathf.Max(1, damage);
            projectileLifetime = Mathf.Max(0.1f, projectileLifetime);
            homingDuration = Mathf.Max(0f, homingDuration);
            turnSpeed = Mathf.Max(0f, turnSpeed);
            spawnRadius = Mathf.Max(0f, spawnRadius);
        }
    }
}
