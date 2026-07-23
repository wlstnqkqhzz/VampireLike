using UnityEngine;
using System.Collections;

namespace VampireLike.Combat
{
    /// <summary>
    /// 플레이어 체력, 접촉 피해, 무적 시간, 피격 연출, 사망 처리를 관리한다.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        // 플레이어 최대 체력이다.
        [SerializeField]
        private int maxHealth = 10;

        // 적과 접촉했을 때 받는 피해량이다.
        [SerializeField]
        private int contactDamage = 1;

        // 피해를 받은 뒤 다시 피해를 받을 수 없도록 막는 시간이다.
        [SerializeField]
        private float invincibleDuration = 1f;

        // 피격 시 빨간색으로 깜빡이는 전체 시간이다.
        [SerializeField]
        private float hitFlashDuration = 0.6f;

        // 피격 점멸의 한 번 깜빡임 간격이다.
        [SerializeField]
        private float hitFlashInterval = 0.08f;

        // 적과 계속 붙어 있을 때 충돌 이벤트 누락을 보완하는 접촉 검사 반경이다.
        [SerializeField]
        private float contactCheckRadius = 0.35f;

        // 접촉 피해를 검사할 Enemy 레이어다.
        [SerializeField]
        private LayerMask enemyLayerMask = 1 << 7;

        private int currentHealth;
        private float invincibleTimer;
        private bool isDead;
        private readonly Collider2D[] contactResults = new Collider2D[8];
        private SpriteRenderer[] spriteRenderers;
        private Color[] originalColors;
        private bool[] originalRendererEnabledStates;
        private Coroutine hitFlashRoutine;

        public bool IsDead => isDead;

        private void Awake()
        {
            // Play 재시작 시 이전 게임 오버 상태가 남지 않게 초기화한다.
            GameState.ResetGame();
            currentHealth = maxHealth;

            if (GetComponent<GameOverUI>() == null)
                gameObject.AddComponent<GameOverUI>();
        }

        private void Start()
        {
            CacheSpriteRenderer();
        }

        private void Update()
        {
            // 무적 시간은 일반 시간 흐름을 따른다. 일시정지 중에는 Time.deltaTime이 0이다.
            if (invincibleTimer > 0f)
                invincibleTimer -= Time.deltaTime;

            CheckEnemyContact();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryApplyContactDamage(collision.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryApplyContactDamage(collision.gameObject);
        }

        private void TryApplyContactDamage(GameObject other)
        {
            // 일시정지/사망 중에는 접촉 피해를 처리하지 않는다.
            if (isDead || Time.timeScale <= 0f)
                return;

            if (!other.CompareTag("Enemy") && other.GetComponentInParent<EnemyHealth>() == null)
                return;

            TakeDamage(GetContactDamage(other));
        }

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            contactDamage = Mathf.Max(1, contactDamage);
            invincibleDuration = Mathf.Max(0f, invincibleDuration);
            hitFlashDuration = Mathf.Max(0f, hitFlashDuration);
            hitFlashInterval = Mathf.Max(0.01f, hitFlashInterval);
            contactCheckRadius = Mathf.Max(0.01f, contactCheckRadius);
        }

        public void TakeDamage(int damage)
        {
            // 무적 시간 중에는 반복 피해를 막는다.
            if (isDead || damage <= 0 || invincibleTimer > 0f)
                return;

            currentHealth -= damage;
            invincibleTimer = invincibleDuration;
            PlayHitFlash();

            if (currentHealth <= 0)
                Die();
        }

        /// <summary>
        /// 최대 체력 강화에서 호출한다. 최대 체력과 현재 체력을 함께 올린다.
        /// </summary>
        public void IncreaseMaxHealth(int amount)
        {
            if (amount <= 0 || isDead)
                return;

            maxHealth += amount;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        /// <summary>
        /// 회복 강화에서 호출한다. 현재 체력을 최대 체력 안에서 회복한다.
        /// </summary>
        public void Heal(int amount)
        {
            if (amount <= 0 || isDead)
                return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        private void CacheSpriteRenderer()
        {
            // PlayerVisual을 포함한 모든 자식 SpriteRenderer의 원래 상태를 저장한다.
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            originalColors = new Color[spriteRenderers.Length];
            originalRendererEnabledStates = new bool[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
                originalRendererEnabledStates[i] = spriteRenderers[i].enabled;
            }
        }

        private void PlayHitFlash()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
                CacheSpriteRenderer();

            if (spriteRenderers == null || spriteRenderers.Length == 0 || hitFlashDuration <= 0f)
                return;

            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);

            hitFlashRoutine = StartCoroutine(HitFlash());
        }

        private IEnumerator HitFlash()
        {
            float elapsedTime = 0f;

            while (elapsedTime < hitFlashDuration)
            {
                SetVisualColors(new Color(1f, 0.15f, 0.1f, 1f));
                yield return new WaitForSeconds(hitFlashInterval);
                elapsedTime += hitFlashInterval;

                RestoreVisualColors();
                yield return new WaitForSeconds(hitFlashInterval);
                elapsedTime += hitFlashInterval;
            }

            RestoreVisuals();
            hitFlashRoutine = null;
        }

        private void CheckEnemyContact()
        {
            // 적과 겹쳐 있는 상태에서도 일정 무적 시간마다 피해를 받도록 보완 검사한다.
            if (isDead || Time.timeScale <= 0f || invincibleTimer > 0f)
                return;

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, contactCheckRadius, contactResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D enemyCollider = contactResults[i];

                if (enemyCollider != null && enemyCollider.GetComponentInParent<EnemyHealth>() != null)
                {
                    TakeDamage(GetContactDamage(enemyCollider.gameObject));
                    return;
                }
            }
        }

        private int GetContactDamage(GameObject enemyObject)
        {
            EnemyContactDamage enemyContactDamage = enemyObject.GetComponentInParent<EnemyContactDamage>();
            return enemyContactDamage == null ? contactDamage : enemyContactDamage.ContactDamage;
        }

        private void SetVisualColors(Color color)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                if (!originalRendererEnabledStates[i])
                    continue;

                spriteRenderers[i].color = color;
            }
        }

        private void RestoreVisuals()
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                spriteRenderers[i].color = originalColors[i];
                spriteRenderers[i].enabled = originalRendererEnabledStates[i];
            }

        }

        private void RestoreVisualColors()
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                    continue;

                spriteRenderers[i].color = originalColors[i];
            }
        }

        private void Die()
        {
            // 현재는 게임 오버 상태 전환과 이동/공격 정지만 처리한다. UI는 이후 단계에서 연결한다.
            isDead = true;
            currentHealth = 0;
            GameState.SetGameOver();

            global::PlayerController playerController = GetComponent<global::PlayerController>();
            PlayerAutoAttack playerAutoAttack = GetComponent<PlayerAutoAttack>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (playerController != null)
                playerController.enabled = false;

            if (playerAutoAttack != null)
                playerAutoAttack.StopAttacking();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}
