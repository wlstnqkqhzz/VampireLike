using UnityEngine;

namespace VampireLike.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private float moveSpeed = 1.5f;

        [SerializeField]
        private float stoppingDistance = 0.2f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (target == null)
                target = GameObject.Find("Player")?.transform;
        }

        private void FixedUpdate()
        {
            if (target == null || Time.timeScale <= 0f)
                return;

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
    }
}
