using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어에게 가까이 접근하면 경고 후 폭발하여 범위 피해를 주는 자폭형 적 행동이다.
    /// 폭발을 시작하면 기본 이동을 멈추고, 폭발 후 자기 자신을 제거한다.
    /// </summary>
    [RequireComponent(typeof(EnemyController))]
    public class ExploderEnemyAttack : MonoBehaviour
    {
        [SerializeField]
        private float triggerRange = 1.15f;

        [SerializeField]
        private float warningDuration = 0.75f;

        [SerializeField]
        private float explosionRadius = 1.2f;

        [SerializeField]
        private int damage = 2;

        [SerializeField]
        private LayerMask playerLayerMask = 1 << 6;

        [SerializeField]
        private GameObject warningPrefab;

        [SerializeField]
        private GameObject explosionPrefab;

        [SerializeField]
        private float effectLifetime = 0.35f;

        private readonly Collider2D[] hitResults = new Collider2D[4];
        private Transform target;
        private EnemyController enemyController;
        private bool isExploding;

        private void Awake()
        {
            target = GameObject.Find("Player")?.transform;
            enemyController = GetComponent<EnemyController>();
        }

        private void Update()
        {
            if (isExploding || target == null || GameState.IsGameOver || Time.timeScale <= 0f)
                return;

            if (Vector2.Distance(transform.position, target.position) <= triggerRange)
                StartCoroutine(ExplodeRoutine());
        }

        private IEnumerator ExplodeRoutine()
        {
            isExploding = true;
            enemyController.SetMovementEnabled(false);

            GameObject warning = SpawnEffect(warningPrefab);
            yield return new WaitForSeconds(warningDuration);

            if (warning != null)
                Destroy(warning);

            ApplyDamage();
            GameObject explosion = SpawnEffect(explosionPrefab);

            if (explosion != null)
                Destroy(explosion, effectLifetime);

            Destroy(gameObject);
        }

        private GameObject SpawnEffect(GameObject prefab)
        {
            if (prefab == null)
                return null;

            GameObject effect = Instantiate(prefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * explosionRadius * 2f;
            return effect;
        }

        private void ApplyDamage()
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, hitResults, playerLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                PlayerHealth playerHealth = hitResults[i].GetComponentInParent<PlayerHealth>();

                if (playerHealth == null)
                    continue;

                playerHealth.TakeDamage(damage);
                return;
            }
        }

        private void OnValidate()
        {
            triggerRange = Mathf.Max(0.1f, triggerRange);
            warningDuration = Mathf.Max(0f, warningDuration);
            explosionRadius = Mathf.Max(0.1f, explosionRadius);
            damage = Mathf.Max(1, damage);
            effectLifetime = Mathf.Max(0.05f, effectLifetime);
        }
    }
}
