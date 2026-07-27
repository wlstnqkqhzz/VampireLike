using System.Collections;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 적에게 적용되는 둔화 같은 임시 상태 이상을 관리한다.
    /// </summary>
    public class EnemyStatusEffects : MonoBehaviour
    {
        private EnemyController enemyController;
        private SpriteRenderer spriteRenderer;
        private Color originalColor = Color.white;
        private float baseMoveSpeed;
        private Coroutine slowRoutine;

        private void Awake()
        {
            enemyController = GetComponent<EnemyController>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (enemyController != null)
                baseMoveSpeed = enemyController.MoveSpeed;

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        private void OnDisable()
        {
            RestoreSlow();
        }

        public void ApplySlow(float moveSpeedMultiplier, float duration)
        {
            if (enemyController == null)
                enemyController = GetComponent<EnemyController>();

            if (enemyController == null)
                return;

            if (baseMoveSpeed <= 0f)
                baseMoveSpeed = enemyController.MoveSpeed;

            if (slowRoutine != null)
                StopCoroutine(slowRoutine);

            slowRoutine = StartCoroutine(SlowRoutine(Mathf.Clamp(moveSpeedMultiplier, 0.1f, 1f), Mathf.Max(0.1f, duration)));
        }

        private IEnumerator SlowRoutine(float moveSpeedMultiplier, float duration)
        {
            enemyController.SetMoveSpeed(baseMoveSpeed * moveSpeedMultiplier);

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(0.45f, 0.8f, 1f, originalColor.a);

            yield return new WaitForSeconds(duration);

            RestoreSlow();
        }

        private void RestoreSlow()
        {
            if (slowRoutine != null)
            {
                StopCoroutine(slowRoutine);
                slowRoutine = null;
            }

            if (enemyController != null && baseMoveSpeed > 0f)
                enemyController.SetMoveSpeed(baseMoveSpeed);

            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
}
