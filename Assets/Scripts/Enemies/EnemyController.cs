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

        [SerializeField]
        private float separationRadius = 0.55f;

        [SerializeField]
        private float separationWeight = 0.45f;

        [SerializeField]
        private LayerMask enemyLayerMask = 1 << 7;

        private Rigidbody2D rb;
        private Collider2D enemyCollider;
        private readonly Collider2D[] separationResults = new Collider2D[8];
        private bool movementEnabled = true;

        public float MoveSpeed => moveSpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyCollider = GetComponent<Collider2D>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            IgnoreEnemyToEnemyCollision();

            if (target == null)
                target = GameObject.Find("Player")?.transform;
        }

        private void FixedUpdate()
        {
            if (!movementEnabled || target == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            // Rigidbody2D 기반 이동으로 물리 충돌과 자연스럽게 맞물리게 한다.
            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = target.position;
            Vector2 toTarget = targetPosition - currentPosition;

            if (toTarget.sqrMagnitude <= stoppingDistance * stoppingDistance)
                return;

            Vector2 chaseDirection = toTarget.normalized;
            Vector2 separationDirection = GetSeparationDirection(currentPosition);
            Vector2 moveDirection = chaseDirection + separationDirection * separationWeight;

            if (moveDirection.sqrMagnitude <= 0.001f)
                moveDirection = chaseDirection;
            else
                moveDirection.Normalize();

            Vector2 nextPosition = currentPosition + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPosition);
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            stoppingDistance = Mathf.Max(0f, stoppingDistance);
            separationRadius = Mathf.Max(0f, separationRadius);
            separationWeight = Mathf.Max(0f, separationWeight);
        }

        public void SetMoveSpeed(float value)
        {
            moveSpeed = Mathf.Max(0f, value);
        }

        public void SetMovementEnabled(bool isEnabled)
        {
            movementEnabled = isEnabled;

            if (!movementEnabled && rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        private Vector2 GetSeparationDirection(Vector2 currentPosition)
        {
            if (separationRadius <= 0f || separationWeight <= 0f)
                return Vector2.zero;

            int hitCount = Physics2D.OverlapCircleNonAlloc(currentPosition, separationRadius, separationResults, enemyLayerMask);
            Vector2 separation = Vector2.zero;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D other = separationResults[i];

                if (other == null || other == enemyCollider)
                    continue;

                Vector2 away = currentPosition - (Vector2)other.transform.position;
                float sqrDistance = away.sqrMagnitude;

                if (sqrDistance <= 0.0001f)
                    continue;

                separation += away.normalized / Mathf.Max(sqrDistance, 0.05f);
            }

            return separation.sqrMagnitude > 1f ? separation.normalized : separation;
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
