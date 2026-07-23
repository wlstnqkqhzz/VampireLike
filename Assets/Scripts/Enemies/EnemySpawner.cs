using System;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어 주변 바깥쪽 원형 범위에서 적을 생성하고, 웨이브가 오를수록 생성 난이도와 적 종류를 확장한다.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        private class EnemySpawnEntry
        {
            [SerializeField]
            private GameObject enemyPrefab;

            [SerializeField]
            private int unlockWave = 1;

            [SerializeField]
            private int spawnWeight = 1;

            public GameObject EnemyPrefab => enemyPrefab;
            public int UnlockWave => unlockWave;
            public int SpawnWeight => spawnWeight;

            public void Validate()
            {
                unlockWave = Mathf.Max(1, unlockWave);
                spawnWeight = Mathf.Max(0, spawnWeight);
            }
        }

        // 기존 단일 적 프리팹이다. Enemy Spawn Entries가 비어 있을 때 fallback으로 사용한다.
        [SerializeField]
        private GameObject enemyPrefab;

        // 웨이브별로 생성 가능한 적 프리팹 목록이다.
        [SerializeField]
        private EnemySpawnEntry[] enemySpawnEntries;

        // 생성 기준이 되는 플레이어 위치다.
        [SerializeField]
        private Transform player;

        // 게임 시작 시 적 생성 시간 간격이다.
        [SerializeField]
        private float spawnInterval = 2f;

        // 웨이브가 올라가도 더 이상 줄어들지 않을 최소 생성 간격이다.
        [SerializeField]
        private float minimumSpawnInterval = 0.45f;

        // 시작 웨이브 번호다.
        [SerializeField]
        private int startingWave = 1;

        // 이 시간이 지날 때마다 다음 웨이브로 넘어간다.
        [SerializeField]
        private float waveDuration = 20f;

        // 웨이브 상승 1회마다 생성 간격에 곱할 값이다. 0.9면 10% 빨라진다.
        [SerializeField]
        private float spawnIntervalMultiplier = 0.9f;

        // 웨이브 변경을 콘솔에서도 확인할지 정한다.
        [SerializeField]
        private bool logWaveChanges = true;

        // 플레이어에게 너무 가까이 생성되지 않도록 하는 최소 거리다.
        [SerializeField]
        private float minSpawnDistance = 4f;

        // 플레이어 주변 어느 정도 바깥까지 생성할지 정하는 최대 거리다.
        [SerializeField]
        private float maxSpawnDistance = 6f;

        // 게임 시작 시 유지할 최대 적 수다.
        [SerializeField]
        private int maxEnemyCount = 30;

        // 웨이브 상승 1회마다 늘어나는 최대 적 수다.
        [SerializeField]
        private int maxEnemyCountIncrease = 5;

        // 최대 적 수가 무한히 커지지 않도록 막는 상한이다.
        [SerializeField]
        private int maxEnemyCountLimit = 120;

        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
        private float spawnTimer;
        private float waveTimer;
        private float currentSpawnInterval;
        private int currentMaxEnemyCount;
        private int currentWave;

        public event Action<int> WaveChanged;

        public int CurrentWave => currentWave;
        public float CurrentSpawnInterval => currentSpawnInterval;
        public int CurrentMaxEnemyCount => currentMaxEnemyCount;
        public int AliveEnemyCount => spawnedEnemies.Count;
        public float WaveProgress => waveDuration <= 0f ? 0f : Mathf.Clamp01(waveTimer / waveDuration);

        private void Awake()
        {
            if (player == null)
                player = GameObject.Find("Player")?.transform;

            currentWave = startingWave;
            currentSpawnInterval = spawnInterval;
            currentMaxEnemyCount = maxEnemyCount;
        }

        private void Update()
        {
            if (player == null || GameState.IsGameOver)
                return;

            UpdateWaveTimer();
            RemoveMissingEnemies();

            if (spawnedEnemies.Count >= currentMaxEnemyCount)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < currentSpawnInterval)
                return;

            spawnTimer = 0f;
            SpawnEnemy();
        }

        private void OnValidate()
        {
            spawnInterval = Mathf.Max(0.1f, spawnInterval);
            minimumSpawnInterval = Mathf.Clamp(minimumSpawnInterval, 0.05f, spawnInterval);
            startingWave = Mathf.Max(1, startingWave);
            waveDuration = Mathf.Max(1f, waveDuration);
            spawnIntervalMultiplier = Mathf.Clamp(spawnIntervalMultiplier, 0.1f, 1f);
            minSpawnDistance = Mathf.Max(0f, minSpawnDistance);
            maxSpawnDistance = Mathf.Max(minSpawnDistance, maxSpawnDistance);
            maxEnemyCount = Mathf.Max(0, maxEnemyCount);
            maxEnemyCountIncrease = Mathf.Max(0, maxEnemyCountIncrease);
            maxEnemyCountLimit = Mathf.Max(maxEnemyCount, maxEnemyCountLimit);

            if (enemySpawnEntries == null)
                return;

            foreach (EnemySpawnEntry entry in enemySpawnEntries)
                entry?.Validate();
        }

        private void UpdateWaveTimer()
        {
            waveTimer += Time.deltaTime;

            if (waveTimer < waveDuration)
                return;

            waveTimer -= waveDuration;
            AdvanceWave();
        }

        private void AdvanceWave()
        {
            currentWave++;
            currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval * spawnIntervalMultiplier);
            currentMaxEnemyCount = Mathf.Min(maxEnemyCountLimit, currentMaxEnemyCount + maxEnemyCountIncrease);
            WaveChanged?.Invoke(currentWave);

            if (logWaveChanges)
                Debug.Log($"Wave {currentWave} started. Spawn Interval: {currentSpawnInterval:0.00}, Max Enemies: {currentMaxEnemyCount}");
        }

        private void SpawnEnemy()
        {
            GameObject selectedEnemyPrefab = GetEnemyPrefabForCurrentWave();

            if (selectedEnemyPrefab == null)
                return;

            Vector2 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity, transform);
            spawnedEnemies.Add(enemy);
        }

        private GameObject GetEnemyPrefabForCurrentWave()
        {
            if (enemySpawnEntries == null || enemySpawnEntries.Length == 0)
                return enemyPrefab;

            int totalWeight = 0;

            foreach (EnemySpawnEntry entry in enemySpawnEntries)
            {
                if (!CanSpawn(entry))
                    continue;

                totalWeight += entry.SpawnWeight;
            }

            if (totalWeight <= 0)
                return enemyPrefab;

            int randomWeight = UnityEngine.Random.Range(0, totalWeight);

            foreach (EnemySpawnEntry entry in enemySpawnEntries)
            {
                if (!CanSpawn(entry))
                    continue;

                if (randomWeight < entry.SpawnWeight)
                    return entry.EnemyPrefab;

                randomWeight -= entry.SpawnWeight;
            }

            return enemyPrefab;
        }

        private bool CanSpawn(EnemySpawnEntry entry)
        {
            return entry != null
                && entry.EnemyPrefab != null
                && entry.SpawnWeight > 0
                && currentWave >= entry.UnlockWave;
        }

        private Vector2 GetRandomSpawnPosition()
        {
            // 원형 방향을 무작위로 뽑고, 최소/최대 거리 사이에 적을 생성한다.
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return (Vector2)player.position + direction * distance;
        }

        private void RemoveMissingEnemies()
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (spawnedEnemies[i] == null)
                    spawnedEnemies.RemoveAt(i);
            }
        }
    }
}
