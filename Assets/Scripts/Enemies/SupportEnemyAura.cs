using System.Collections.Generic;
using VampireLike.Combat;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 주변 일반 적의 이동 속도를 높이고 체력을 조금 보강하는 지원형 적 행동이다.
    /// 반복적인 전체 검색 대신 일정 간격으로만 주변 콜라이더를 확인한다.
    /// </summary>
    public class SupportEnemyAura : MonoBehaviour
    {
        [SerializeField]
        private float auraRadius = 2.8f;

        [SerializeField]
        private float refreshInterval = 1f;

        [SerializeField]
        private float speedMultiplier = 1.25f;

        [SerializeField]
        private int healthBonus = 1;

        [SerializeField]
        private LayerMask enemyLayerMask = 1 << 7;

        [SerializeField]
        private Color auraColor = new Color(0.55f, 1f, 0.65f, 1f);

        private readonly Collider2D[] results = new Collider2D[24];
        private readonly HashSet<EnemyController> buffedEnemies = new HashSet<EnemyController>();
        private float nextRefreshTime;
        private SpriteRenderer spriteRenderer;
        private Color originalColor = Color.white;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        private void Update()
        {
            if (GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (Time.time < nextRefreshTime)
                return;

            nextRefreshTime = Time.time + refreshInterval;
            RefreshAura();
        }

        private void OnDisable()
        {
            ClearSpeedBuffs();
        }

        private void RefreshAura()
        {
            ClearSpeedBuffs();

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, auraRadius, results, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                EnemyController enemyController = results[i].GetComponentInParent<EnemyController>();

                if (enemyController == null || enemyController.gameObject == gameObject)
                    continue;

                enemyController.SetMoveSpeed(enemyController.MoveSpeed * speedMultiplier);
                buffedEnemies.Add(enemyController);

                EnemyHealth enemyHealth = enemyController.GetComponent<EnemyHealth>();

                if (enemyHealth != null && healthBonus > 0)
                    enemyHealth.Heal(healthBonus);
            }

            if (spriteRenderer != null)
                spriteRenderer.color = buffedEnemies.Count > 0 ? auraColor : originalColor;
        }

        private void ClearSpeedBuffs()
        {
            foreach (EnemyController enemyController in buffedEnemies)
            {
                if (enemyController == null)
                    continue;

                enemyController.SetMoveSpeed(enemyController.MoveSpeed / Mathf.Max(0.01f, speedMultiplier));
            }

            buffedEnemies.Clear();

            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }

        private void OnValidate()
        {
            auraRadius = Mathf.Max(0.1f, auraRadius);
            refreshInterval = Mathf.Max(0.1f, refreshInterval);
            speedMultiplier = Mathf.Max(1f, speedMultiplier);
            healthBonus = Mathf.Max(0, healthBonus);
        }
    }
}
