using UnityEngine;
using VampireLike.Combat;
using System;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 지정된 웨이브 간격마다 보스 1마리를 생성하고, 등장 웨이브에 따라 능력치를 강화한다.
    /// </summary>
    public class BossSpawner : MonoBehaviour
    {
        [System.Serializable]
        private class BossSpawnEntry
        {
            [SerializeField]
            private int bossStage = 1;

            [SerializeField]
            private GameObject bossPrefab;

            public int BossStage => bossStage;
            public GameObject BossPrefab => bossPrefab;

            public void Validate()
            {
                bossStage = Mathf.Max(1, bossStage);
            }
        }

        [SerializeField]
        private EnemySpawner enemySpawner;

        [SerializeField]
        private GameObject bossPrefab;

        [SerializeField]
        private BossSpawnEntry[] bossSpawnEntries;

        [SerializeField]
        private Transform player;

        [SerializeField]
        private int bossWaveInterval = 5;

        [SerializeField]
        private float minSpawnDistance = 5f;

        [SerializeField]
        private float maxSpawnDistance = 7f;

        [SerializeField]
        private float healthMultiplierPerAppearance = 1.35f;

        [SerializeField]
        private float contactDamageMultiplierPerAppearance = 1.25f;

        [SerializeField]
        private float moveSpeedMultiplierPerAppearance = 1.08f;

        [SerializeField]
        private float maxBossMoveSpeed = 1.8f;

        private GameObject activeBoss;
        private EnemyHealth activeBossHealth;
        private int activeBossStage;
        private int lastBossSpawnWave;
        private bool hasPausedWaveProgress;

        public EnemyHealth ActiveBossHealth => activeBossHealth;
        public bool HasActiveBoss => activeBossHealth != null && !activeBossHealth.IsDead;
        public int ActiveBossStage => activeBossStage;
        public event Action<int, GameObject> BossSpawned;
        public event Action<int, EnemyHealth> BossDefeated;

        private void Awake()
        {
            if (enemySpawner == null)
                enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (player == null)
                player = GameObject.Find("Player")?.transform;
        }

        private void OnEnable()
        {
            if (enemySpawner == null)
                enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (enemySpawner != null)
                enemySpawner.WaveChanged += HandleWaveChanged;
        }

        private void OnDisable()
        {
            UnsubscribeActiveBossDeath();

            if (enemySpawner != null)
            {
                enemySpawner.WaveChanged -= HandleWaveChanged;
                SetWaveProgressPaused(false);
            }
        }

        private void Update()
        {
            if (activeBoss == null || activeBossHealth == null || activeBossHealth.IsDead)
            {
                UnsubscribeActiveBossDeath();
                activeBoss = null;
                activeBossHealth = null;
                activeBossStage = 0;
                SetWaveProgressPaused(false);
            }
            else
            {
                SetWaveProgressPaused(true);
            }

            if (activeBoss != null)
                return;

            if (enemySpawner == null || player == null || GameState.IsGameOver)
                return;

            TrySpawnBoss(enemySpawner.CurrentWave);
        }

        private void OnValidate()
        {
            bossWaveInterval = Mathf.Max(1, bossWaveInterval);
            minSpawnDistance = Mathf.Max(0f, minSpawnDistance);
            maxSpawnDistance = Mathf.Max(minSpawnDistance, maxSpawnDistance);
            healthMultiplierPerAppearance = Mathf.Max(1f, healthMultiplierPerAppearance);
            contactDamageMultiplierPerAppearance = Mathf.Max(1f, contactDamageMultiplierPerAppearance);
            moveSpeedMultiplierPerAppearance = Mathf.Max(1f, moveSpeedMultiplierPerAppearance);
            maxBossMoveSpeed = Mathf.Max(0.1f, maxBossMoveSpeed);

            if (bossSpawnEntries == null)
                return;

            foreach (BossSpawnEntry entry in bossSpawnEntries)
                entry?.Validate();
        }

        private void HandleWaveChanged(int wave)
        {
            TrySpawnBoss(wave);
        }

        private void TrySpawnBoss(int wave)
        {
            if (player == null)
                return;

            if (wave <= 0 || wave % bossWaveInterval != 0)
                return;

            if (lastBossSpawnWave == wave)
                return;

            if (activeBoss != null)
            {
                lastBossSpawnWave = wave;
                return;
            }

            SpawnBoss(wave);
        }

        private void SpawnBoss(int wave)
        {
            int bossStage = Mathf.Max(1, wave / bossWaveInterval);
            GameObject selectedBossPrefab = GetBossPrefabForStage(bossStage);

            if (selectedBossPrefab == null)
            {
                Debug.LogWarning($"Boss prefab is missing for stage {bossStage}.");
                lastBossSpawnWave = wave;
                return;
            }

            Vector2 spawnPosition = GetRandomSpawnPosition();
            activeBoss = Instantiate(selectedBossPrefab, spawnPosition, Quaternion.identity, transform);
            activeBossHealth = activeBoss.GetComponent<EnemyHealth>();
            activeBossStage = bossStage;
            lastBossSpawnWave = wave;

            if (activeBossHealth != null)
                activeBossHealth.Died += HandleActiveBossDied;

            BossController bossController = activeBoss.GetComponent<BossController>();
            if (bossController != null)
                bossController.InitializeBoss(bossStage, player);

            ApplyBossScaling(activeBoss, wave);
            SetWaveProgressPaused(true);
            Debug.Log($"Boss appeared - Wave {wave}");
            BossSpawned?.Invoke(bossStage, activeBoss);
        }

        private void HandleActiveBossDied(EnemyHealth defeatedBoss)
        {
            int defeatedStage = activeBossStage;
            BossDefeated?.Invoke(defeatedStage, defeatedBoss);
            UnsubscribeActiveBossDeath();
        }

        private void UnsubscribeActiveBossDeath()
        {
            if (activeBossHealth != null)
                activeBossHealth.Died -= HandleActiveBossDied;
        }

        private void SetWaveProgressPaused(bool paused)
        {
            if (enemySpawner == null || hasPausedWaveProgress == paused)
                return;

            hasPausedWaveProgress = paused;
            enemySpawner.SetWaveProgressPaused(paused);
        }

        private GameObject GetBossPrefabForStage(int bossStage)
        {
            if (bossSpawnEntries == null || bossSpawnEntries.Length == 0)
                return bossPrefab;

            GameObject selectedPrefab = null;
            int selectedStage = 0;

            foreach (BossSpawnEntry entry in bossSpawnEntries)
            {
                if (entry == null || entry.BossPrefab == null)
                    continue;

                if (entry.BossStage > bossStage || entry.BossStage < selectedStage)
                    continue;

                selectedPrefab = entry.BossPrefab;
                selectedStage = entry.BossStage;
            }

            return selectedPrefab != null ? selectedPrefab : bossPrefab;
        }

        private void ApplyBossScaling(GameObject boss, int wave)
        {
            int appearanceIndex = Mathf.Max(1, wave / bossWaveInterval);
            float healthMultiplier = Mathf.Pow(healthMultiplierPerAppearance, appearanceIndex - 1);
            float damageMultiplier = Mathf.Pow(contactDamageMultiplierPerAppearance, appearanceIndex - 1);
            float speedMultiplier = Mathf.Pow(moveSpeedMultiplierPerAppearance, appearanceIndex - 1);

            EnemyHealth enemyHealth = boss.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.SetMaxHealth(Mathf.RoundToInt(enemyHealth.MaxHealth * healthMultiplier));

            EnemyContactDamage contactDamage = boss.GetComponent<EnemyContactDamage>();
            if (contactDamage != null)
                contactDamage.SetContactDamage(Mathf.RoundToInt(contactDamage.ContactDamage * damageMultiplier));

            EnemyController enemyController = boss.GetComponent<EnemyController>();
            if (enemyController != null)
                enemyController.SetMoveSpeed(Mathf.Min(maxBossMoveSpeed, enemyController.MoveSpeed * speedMultiplier));
        }

        private Vector2 GetRandomSpawnPosition()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return (Vector2)player.position + direction * distance;
        }
    }
}
