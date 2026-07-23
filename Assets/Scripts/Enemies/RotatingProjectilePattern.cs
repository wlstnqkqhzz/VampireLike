using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 시작 각도를 회전시키며 여러 번 방사형 탄막을 발사하는 패턴이다.
    /// </summary>
    public class RotatingProjectilePattern : RadialProjectilePattern
    {
        [SerializeField]
        private int volleyCount = 4;

        [SerializeField]
        private float volleyInterval = 0.25f;

        [SerializeField]
        private float rotationPerVolley = 18f;

        [SerializeField]
        private int projectilesPerVolley = 10;

        protected override IEnumerator ExecutePattern()
        {
            Boss.SetState(BossState.Preparing, false);

            for (int i = 0; i < volleyCount && !Boss.IsDead; i++)
            {
                FireRadial(projectilesPerVolley + Mathf.Max(0, Boss.CurrentPhase - 1) * 2, rotationPerVolley * i);

                if (volleyInterval > 0f)
                    yield return new WaitForSeconds(volleyInterval);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            volleyCount = Mathf.Max(1, volleyCount);
            volleyInterval = Mathf.Max(0f, volleyInterval);
            projectilesPerVolley = Mathf.Max(1, projectilesPerVolley);
        }
    }
}
