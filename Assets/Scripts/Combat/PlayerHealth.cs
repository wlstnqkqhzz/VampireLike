using UnityEngine;
using System.Collections;

namespace VampireLike.Combat
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth = 10;

        [SerializeField]
        private int contactDamage = 1;

        [SerializeField]
        private float invincibleDuration = 1f;

        [SerializeField]
        private float hitFlashDuration = 0.6f;

        [SerializeField]
        private float hitFlashInterval = 0.08f;

        [SerializeField]
        private float contactCheckRadius = 0.35f;

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
            GameState.ResetGame();
            currentHealth = maxHealth;
        }

        private void Start()
        {
            CacheSpriteRenderer();
        }

        private void Update()
        {
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
            if (isDead || Time.timeScale <= 0f)
                return;

            if (!other.CompareTag("Enemy") && other.GetComponentInParent<EnemyHealth>() == null)
                return;

            TakeDamage(contactDamage);
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
            if (isDead || damage <= 0 || invincibleTimer > 0f)
                return;

            currentHealth -= damage;
            invincibleTimer = invincibleDuration;
            PlayHitFlash();

            if (currentHealth <= 0)
                Die();
        }

        private void CacheSpriteRenderer()
        {
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
            if (isDead || Time.timeScale <= 0f || invincibleTimer > 0f)
                return;

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, contactCheckRadius, contactResults, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D enemyCollider = contactResults[i];

                if (enemyCollider != null && enemyCollider.GetComponentInParent<EnemyHealth>() != null)
                {
                    TakeDamage(contactDamage);
                    return;
                }
            }
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
