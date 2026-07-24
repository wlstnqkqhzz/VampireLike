using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 플레이어 방향을 미리 고정한 뒤 빠르게 돌진하는 패턴이다.
    /// </summary>
    public class DashPattern : BossPattern
    {
        protected override bool UseAutomaticAnimation => false;

        [SerializeField]
        private float prepareTime = 0.7f;

        [SerializeField]
        private float dashSpeed = 6f;

        [SerializeField]
        private float dashDuration = 0.35f;

        [SerializeField]
        private float endLag = 0.25f;

        protected override IEnumerator ExecutePattern()
        {
            if (Player == null || BossRigidbody == null)
                yield break;

            Boss.SetState(BossState.Preparing, false);
            Vector2 dashDirection = ((Vector2)Player.position - BossRigidbody.position).normalized;

            if (dashDirection.sqrMagnitude <= 0.001f)
                dashDirection = Vector2.down;

            Boss.FaceDirection(dashDirection);
            Boss.ShowAttackFrame(0);

            yield return new WaitForSeconds(prepareTime);

            Boss.SetState(BossState.Attacking, false);
            Boss.FaceDirection(dashDirection);
            Boss.ShowAttackFrame(1);
            float elapsedTime = 0f;

            while (elapsedTime < dashDuration && !Boss.IsDead)
            {
                Vector2 nextPosition = BossRigidbody.position + dashDirection * dashSpeed * Time.fixedDeltaTime;
                BossRigidbody.MovePosition(nextPosition);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            BossRigidbody.linearVelocity = Vector2.zero;
            Boss.SetState(BossState.Recovering, false);
            yield return new WaitForSeconds(endLag);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            prepareTime = Mathf.Max(0f, prepareTime);
            dashSpeed = Mathf.Max(0f, dashSpeed);
            dashDuration = Mathf.Max(0f, dashDuration);
            endLag = Mathf.Max(0f, endLag);
        }
    }
}
