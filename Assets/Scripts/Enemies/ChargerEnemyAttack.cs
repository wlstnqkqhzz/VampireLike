using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어가 사정거리 안에 들어오면 잠깐 멈춘 뒤 저장한 방향으로 돌진하는 적 행동이다.
    /// 돌진 중 방향을 다시 바꾸지 않아 플레이어가 준비 동작을 보고 피할 수 있다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyController))]
    public class ChargerEnemyAttack : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private float triggerRange = 3.2f;

        [SerializeField]
        private float prepareTime = 0.55f;

        [SerializeField]
        private float dashSpeed = 5.2f;

        [SerializeField]
        private float dashDuration = 0.32f;

        [SerializeField]
        private float cooldown = 2.4f;

        [SerializeField]
        private Color prepareColor = new Color(1f, 0.55f, 0.2f, 1f);

        private Rigidbody2D rb;
        private EnemyController enemyController;
        private SpriteRenderer spriteRenderer;
        private Color originalColor = Color.white;
        private float nextReadyTime;
        private bool isCharging;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyController = GetComponent<EnemyController>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;

            if (target == null)
                target = GameObject.Find("Player")?.transform;
        }

        private void Update()
        {
            if (isCharging || target == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (Time.time < nextReadyTime)
                return;

            if (Vector2.Distance(transform.position, target.position) > triggerRange)
                return;

            StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            isCharging = true;
            enemyController.SetMovementEnabled(false);

            Vector2 direction = ((Vector2)target.position - rb.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector2.down;

            if (spriteRenderer != null)
                spriteRenderer.color = prepareColor;

            yield return new WaitForSeconds(prepareTime);

            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;

            float elapsedTime = 0f;

            while (elapsedTime < dashDuration && !GameState.IsGameOver)
            {
                rb.MovePosition(rb.position + direction * dashSpeed * Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            enemyController.SetMovementEnabled(true);
            nextReadyTime = Time.time + cooldown;
            isCharging = false;
        }

        private void OnValidate()
        {
            triggerRange = Mathf.Max(0.1f, triggerRange);
            prepareTime = Mathf.Max(0f, prepareTime);
            dashSpeed = Mathf.Max(0f, dashSpeed);
            dashDuration = Mathf.Max(0f, dashDuration);
            cooldown = Mathf.Max(0.1f, cooldown);
        }
    }
}
