using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Growth;

namespace VampireLike.Combat
{
    public class EnemyHealth : MonoBehaviour
    {
        private static readonly List<EnemyHealth> activeEnemies = new List<EnemyHealth>();

        [SerializeField]
        private int maxHealth = 3;

        [SerializeField]
        private float hitFlashDuration = 0.08f;

        [SerializeField]
        private ExperienceGem experienceGemPrefab;

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private Color originalColor = Color.white;
        private Coroutine hitFlashRoutine;

        public static IReadOnlyList<EnemyHealth> ActiveEnemies => activeEnemies;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
            if (IsDead || damage <= 0)
                return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            PlayHitFlash();
        }

        private void PlayHitFlash()
        {
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
            DropExperienceGem();
            Destroy(gameObject);
        }

        private void DropExperienceGem()
        {
            if (experienceGemPrefab == null)
                return;

            Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
        }
    }
}
