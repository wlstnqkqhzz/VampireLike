using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 중심에서 여러 방향으로 적 전용 투사체를 발사하는 패턴이다.
    /// </summary>
    public class RadialProjectilePattern : BossPattern
    {
        protected override bool UseSkillAnimation => true;

        [SerializeField]
        private EnemyProjectileController projectilePrefab;

        [SerializeField]
        private int projectileCount = 8;

        [SerializeField]
        private int phaseBonusProjectileCount = 2;

        [SerializeField]
        private float projectileSpeed = 3.5f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private float projectileLifetime = 6f;

        [SerializeField]
        private float startAngle;

        [SerializeField]
        private float prepareTime = 0.45f;

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);

            if (prepareTime > 0f)
                yield return new WaitForSeconds(prepareTime);

            int count = projectileCount + Mathf.Max(0, Boss.CurrentPhase - 1) * phaseBonusProjectileCount;
            FireRadial(count, startAngle);
        }

        protected void FireRadial(int count, float angleOffset)
        {
            if (projectilePrefab == null || count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                float angle = (angleOffset + 360f * i / count) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                EnemyProjectileController projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.Initialize(direction, projectileSpeed, damage, projectileLifetime);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            projectileCount = Mathf.Max(1, projectileCount);
            phaseBonusProjectileCount = Mathf.Max(0, phaseBonusProjectileCount);
            projectileSpeed = Mathf.Max(0f, projectileSpeed);
            damage = Mathf.Max(1, damage);
            projectileLifetime = Mathf.Max(0.1f, projectileLifetime);
            prepareTime = Mathf.Max(0f, prepareTime);
        }
    }
}
