using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// Enemy가 플레이어를 향해 이동하도록 처리하는 기본 추적 AI다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        // 추적할 대상이다. 비어 있으면 Player를 자동으로 찾는다.
        [SerializeField]
        private Transform target;

        // 초당 이동 속도다.
        [SerializeField]
        private float moveSpeed = 1.5f;

        // 대상에게 이 거리 이하로 가까워지면 이동을 멈춘다.
        [SerializeField]
        private float stoppingDistance = 0f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            IgnoreEnemyToEnemyCollision();

            if (target == null)
                target = GameObject.Find("Player")?.transform;
        }

        private void FixedUpdate()
        {
            if (target == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            // Rigidbody2D 기반 이동으로 물리 충돌과 자연스럽게 맞물리게 한다.
            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = target.position;
            Vector2 toTarget = targetPosition - currentPosition;

            if (toTarget.sqrMagnitude <= stoppingDistance * stoppingDistance)
                return;

            Vector2 nextPosition = currentPosition + toTarget.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPosition);
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            stoppingDistance = Mathf.Max(0f, stoppingDistance);
        }

        private static void IgnoreEnemyToEnemyCollision()
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");

            if (enemyLayer < 0)
                return;

            // 뱀서라이크처럼 적들이 촘촘하게 몰릴 수 있도록 적끼리의 물리 밀어내기를 끈다.
            Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        }
    }
}
