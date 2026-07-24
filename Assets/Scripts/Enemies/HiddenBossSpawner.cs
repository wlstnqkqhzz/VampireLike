using System.Collections;
using UnityEngine;
using VampireLike.Combat;
using VampireLike.Growth;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 10단계 최종 보스가 처치된 뒤 히든 보스 Greed Lord를 한 번만 등장시킨다.
    /// 일반 5웨이브 보스 생성 흐름과 분리해 기존 1~10단계 보스 동작을 건드리지 않는다.
    /// </summary>
    public class HiddenBossSpawner : MonoBehaviour
    {
        [SerializeField]
        private BossSpawner bossSpawner;

        [SerializeField]
        private EnemySpawner enemySpawner;

        [SerializeField]
        private Transform player;

        [SerializeField]
        private GameObject hiddenBossPrefab;

        [SerializeField]
        private int triggerBossStage = 10;

        [SerializeField]
        private float spawnDelay = 3f;

        [SerializeField]
        private float introDuration = 2f;

        [SerializeField]
        private float spawnDistanceFromPlayer = 6f;

        [SerializeField]
        private ExperienceGem rewardGemPrefab;

        [SerializeField]
        private float rewardBonusMultiplier = 1.15f;

        [SerializeField]
        private int maxRewardGemCount = 20;

        private GameObject activeHiddenBoss;
        private EnemyHealth activeHiddenBossHealth;
        private GreedBossController activeGreedBoss;
        private Coroutine spawnRoutine;
        private bool hasSpawnedHiddenBoss;
        private bool hasPausedWaveProgress;

        public EnemyHealth ActiveHiddenBossHealth => activeHiddenBossHealth;
        public GreedBossController ActiveGreedBoss => activeGreedBoss;
        public bool HasActiveHiddenBoss => activeHiddenBossHealth != null && !activeHiddenBossHealth.IsDead;

        private void Awake()
        {
            if (bossSpawner == null)
                bossSpawner = FindFirstObjectByType<BossSpawner>();

            if (enemySpawner == null)
                enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (player == null)
                player = GameObject.Find("Player")?.transform;
        }

        private void OnEnable()
        {
            if (bossSpawner == null)
                bossSpawner = FindFirstObjectByType<BossSpawner>();

            if (bossSpawner != null)
                bossSpawner.BossDefeated += HandleBossDefeated;
        }

        private void OnDisable()
        {
            if (bossSpawner != null)
                bossSpawner.BossDefeated -= HandleBossDefeated;

            if (activeHiddenBossHealth != null)
                activeHiddenBossHealth.Died -= HandleHiddenBossDied;

            SetWaveProgressPaused(false);
        }

        private void Update()
        {
            if (spawnRoutine != null)
            {
                SetWaveProgressPaused(true);
                return;
            }

            if (activeHiddenBoss == null || activeHiddenBossHealth == null || activeHiddenBossHealth.IsDead)
            {
                activeHiddenBoss = null;
                activeHiddenBossHealth = null;
                activeGreedBoss = null;

                if (spawnRoutine == null)
                    SetWaveProgressPaused(false);
            }
            else
            {
                SetWaveProgressPaused(true);
            }
        }

        private void LateUpdate()
        {
            if (spawnRoutine != null || HasActiveHiddenBoss)
                SetWaveProgressPaused(true);
        }

        private void OnValidate()
        {
            triggerBossStage = Mathf.Max(1, triggerBossStage);
            spawnDelay = Mathf.Max(0f, spawnDelay);
            introDuration = Mathf.Max(0f, introDuration);
            spawnDistanceFromPlayer = Mathf.Max(0f, spawnDistanceFromPlayer);
            rewardBonusMultiplier = Mathf.Max(1f, rewardBonusMultiplier);
            maxRewardGemCount = Mathf.Max(1, maxRewardGemCount);
        }

        private void HandleBossDefeated(int bossStage, EnemyHealth defeatedBoss)
        {
            if (hasSpawnedHiddenBoss || bossStage != triggerBossStage || GameState.IsGameOver)
                return;

            if (spawnRoutine != null)
                return;

            spawnRoutine = StartCoroutine(SpawnHiddenBossAfterDelay());
        }

        private IEnumerator SpawnHiddenBossAfterDelay()
        {
            hasSpawnedHiddenBoss = true;
            SetWaveProgressPaused(true);

            if (spawnDelay > 0f)
                yield return new WaitForSeconds(spawnDelay);

            if (introDuration > 0f)
            {
                Debug.Log("Greed Lord is coming...");
                yield return new WaitForSeconds(introDuration);
            }

            SpawnHiddenBoss();
            spawnRoutine = null;
        }

        private void SpawnHiddenBoss()
        {
            if (hiddenBossPrefab == null)
            {
                Debug.LogWarning("Hidden boss prefab is missing.");
                SetWaveProgressPaused(false);
                return;
            }

            Vector2 spawnPosition = GetSpawnPosition();
            activeHiddenBoss = Instantiate(hiddenBossPrefab, spawnPosition, Quaternion.identity, transform);
            activeHiddenBossHealth = activeHiddenBoss.GetComponent<EnemyHealth>();
            activeGreedBoss = activeHiddenBoss.GetComponent<GreedBossController>();

            if (activeGreedBoss == null)
                activeGreedBoss = activeHiddenBoss.AddComponent<GreedBossController>();

            if (activeHiddenBoss.GetComponent<GreedGemCollector>() == null)
                activeHiddenBoss.AddComponent<GreedGemCollector>();

            if (activeHiddenBoss.GetComponent<GreedBossVisualController>() == null)
                activeHiddenBoss.AddComponent<GreedBossVisualController>();

            BossController bossController = activeHiddenBoss.GetComponent<BossController>();

            if (bossController != null)
                bossController.InitializeBoss(triggerBossStage + 1, player);

            if (activeHiddenBossHealth != null)
                activeHiddenBossHealth.Died += HandleHiddenBossDied;

            SetWaveProgressPaused(true);
            Debug.Log("Greed Lord appeared.");
        }

        private void HandleHiddenBossDied(EnemyHealth defeatedBoss)
        {
            if (activeGreedBoss != null)
            {
                Debug.Log($"Greed Lord defeated. Absorbed EXP: {activeGreedBoss.TotalAbsorbedExperience}");
                DropAbsorbedExperienceReward(activeGreedBoss.TotalAbsorbedExperience, defeatedBoss.transform.position);
            }

            SetWaveProgressPaused(false);
        }

        private void DropAbsorbedExperienceReward(int absorbedExperience, Vector3 dropPosition)
        {
            if (rewardGemPrefab == null || absorbedExperience <= 0)
                return;

            int totalRewardExperience = Mathf.CeilToInt(absorbedExperience * rewardBonusMultiplier);
            int gemCount = Mathf.Clamp(totalRewardExperience, 1, maxRewardGemCount);
            int baseAmount = Mathf.Max(1, totalRewardExperience / gemCount);
            int remainder = totalRewardExperience % gemCount;

            for (int i = 0; i < gemCount; i++)
            {
                float angle = i * Mathf.PI * 2f / gemCount;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0.35f, 1.2f);
                ExperienceGem gem = Instantiate(rewardGemPrefab, (Vector2)dropPosition + offset, Quaternion.identity);
                gem.SetExperienceAmount(baseAmount + (i < remainder ? 1 : 0));
            }
        }

        private Vector2 GetSpawnPosition()
        {
            if (player == null)
                return transform.position;

            Vector2 direction = Vector2.up;
            return (Vector2)player.position + direction * spawnDistanceFromPlayer;
        }

        private void SetWaveProgressPaused(bool paused)
        {
            if (enemySpawner == null)
                return;

            hasPausedWaveProgress = paused;
            enemySpawner.SetWaveProgressPaused(paused);
        }
    }
}
