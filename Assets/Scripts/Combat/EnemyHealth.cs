using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VampireLike.Growth;
using VampireLike.Enemies;

namespace VampireLike.Combat
{
    /// <summary>
    /// Enemy의 체력, 피격 효과, 사망 시 경험치 보석 드롭을 관리한다.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        // 자동 공격이 매번 Find 계열 함수를 쓰지 않도록 살아 있는 적 목록을 유지한다.
        private static readonly List<EnemyHealth> activeEnemies = new List<EnemyHealth>();

        // 적의 최대 체력이다.
        [SerializeField]
        private int maxHealth = 3;

        // 피격 시 빨간색으로 깜빡이는 시간이다.
        [SerializeField]
        private float hitFlashDuration = 0.08f;

        // 사망 시 생성할 경험치 보석 프리팹이다.
        [SerializeField]
        private ExperienceGem experienceGemPrefab;

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private BossSpriteAnimator bossSpriteAnimator;
        private Color originalColor = Color.white;
        private Coroutine hitFlashRoutine;

        public static IReadOnlyList<EnemyHealth> ActiveEnemies => activeEnemies;
        public bool IsDead { get; private set; }
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public float HealthProgress => maxHealth <= 0 ? 0f : Mathf.Clamp01((float)currentHealth / maxHealth);
        public event Action<EnemyHealth> Died;

        private void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bossSpriteAnimator = GetComponentInChildren<BossSpriteAnimator>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        private void OnEnable()
        {
            if (!activeEnemies.Contains(this))
                activeEnemies.Add(this);
        }

        private void OnDisable()
        {
            activeEnemies.Remove(this);
        }

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            hitFlashDuration = Mathf.Max(0f, hitFlashDuration);
        }

        public void TakeDamage(int damage)
        {
            // 이미 죽었거나 잘못된 피해량이면 무시한다.
            if (IsDead || damage <= 0)
                return;

            currentHealth -= damage;
            bossSpriteAnimator?.PlayHit();

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            PlayHitFlash();
        }

        public void SetMaxHealth(int value)
        {
            maxHealth = Mathf.Max(1, value);
            currentHealth = maxHealth;
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        private void PlayHitFlash()
        {
            // 사망하지 않은 피격만 짧은 색상 변화로 표현한다.
            if (spriteRenderer == null || hitFlashDuration <= 0f)
                return;

            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash());
        }

        private IEnumerator HitFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.color = originalColor;
            hitFlashRoutine = null;
        }

        private void Die()
        {
            IsDead = true;
            GameSessionStats.RecordKill(IsBossEnemy());
            Died?.Invoke(this);
            DropExperienceGem();

            float deathDuration = bossSpriteAnimator == null ? 0f : bossSpriteAnimator.PlayDeath();

            DisableAfterDeath();

            if (deathDuration > 0f)
                Destroy(gameObject, deathDuration);
            else
                Destroy(gameObject);
        }

        private bool IsBossEnemy()
        {
            return GetComponent<BossController>() != null || GetComponent<GreedBossController>() != null;
        }

        private void DisableAfterDeath()
        {
            EnemyController enemyController = GetComponent<EnemyController>();

            if (enemyController != null)
                enemyController.SetMovementEnabled(false);

            EnemyContactDamage contactDamage = GetComponent<EnemyContactDamage>();

            if (contactDamage != null)
                contactDamage.enabled = false;

            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

            foreach (Collider2D targetCollider in colliders)
                targetCollider.enabled = false;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        private void DropExperienceGem()
        {
            // 경험치 시스템이 연결되지 않은 테스트 상황에서도 안전하게 동작한다.
            if (experienceGemPrefab == null)
                return;

            Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
        }
    }
}
