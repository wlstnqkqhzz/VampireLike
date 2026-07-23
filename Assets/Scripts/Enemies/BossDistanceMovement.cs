using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 플레이어와 적정 거리를 유지하도록 이동시키는 보조 이동 컴포넌트다.
    /// EnemyController 기본 추적을 끈 보스 프리팹에서 사용한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BossController))]
    public class BossDistanceMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 1.15f;

        [SerializeField]
        private float minDistance = 2.7f;

        [SerializeField]
        private float maxDistance = 4.4f;

        [SerializeField]
        private float strafeWeight = 0.35f;

        private BossController bossController;
        private Rigidbody2D rb;

        private void Awake()
        {
            bossController = GetComponent<BossController>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (bossController == null || bossController.Player == null || bossController.IsDead || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (bossController.State != BossState.Chasing)
                return;

            Vector2 currentPosition = rb.position;
            Vector2 toPlayer = (Vector2)bossController.Player.position - currentPosition;
            float distance = toPlayer.magnitude;

            if (distance <= 0.001f)
                return;

            Vector2 directionToPlayer = toPlayer / distance;
            Vector2 moveDirection;

            if (distance < minDistance)
                moveDirection = -directionToPlayer;
            else if (distance > maxDistance)
                moveDirection = directionToPlayer;
            else
                moveDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafeWeight;

            if (moveDirection.sqrMagnitude <= 0.001f)
                return;

            moveDirection.Normalize();
            rb.MovePosition(currentPosition + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            minDistance = Mathf.Max(0f, minDistance);
            maxDistance = Mathf.Max(minDistance, maxDistance);
            strafeWeight = Mathf.Max(0f, strafeWeight);
        }
    }
}
