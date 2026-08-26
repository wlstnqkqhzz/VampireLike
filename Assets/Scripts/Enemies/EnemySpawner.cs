using System;
using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;
using VampireLike.Settings;
using VampireLike.World;

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
            private int maxWave;

            [SerializeField]
            private int spawnWeight = 1;

            public GameObject EnemyPrefab => enemyPrefab;
            public int UnlockWave => unlockWave;
            public int MaxWave => maxWave;
            public int SpawnWeight => spawnWeight;

            public void Validate()
            {
                unlockWave = Mathf.Max(1, unlockWave);
                maxWave = Mathf.Max(0, maxWave);
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
        private float spawnInterval = 1.15f;

        // 웨이브가 올라가도 더 이상 줄어들지 않을 최소 생성 간격이다.
        [SerializeField]
        private float minimumSpawnInterval = 0.25f;

        // 시작 웨이브 번호다.
        [SerializeField]
        private int startingWave = 1;

        // 이 시간이 지날 때마다 다음 웨이브로 넘어간다.
        [SerializeField]
        private float waveDuration = 20f;

        // 웨이브 상승 1회마다 생성 간격에 곱할 값이다. 0.9면 10% 빨라진다.
        [SerializeField]
        private float spawnIntervalMultiplier = 0.86f;

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
        private int maxEnemyCount = 44;

        [Header("Early Wave Ease")]
        // 초반에는 플레이어 화력이 낮으므로 적 밀도를 천천히 올린다.
        [SerializeField]
        private int earlyEaseEndWave = 5;

        [SerializeField]
        private float earlySpawnIntervalMultiplier = 1.65f;

        [SerializeField]
        private float earlyMaxEnemyMultiplier = 0.55f;

        [SerializeField]
        private int earlyMinimumMaxEnemyCount = 18;

        // 웨이브 상승 1회마다 늘어나는 최대 적 수다.
        [SerializeField]
        private int maxEnemyCountIncrease = 9;

        // 최대 적 수가 무한히 커지지 않도록 막는 상한이다.
        [SerializeField]
        private int maxEnemyCountLimit = 220;

        [SerializeField]
        private int enemiesPerSpawn = 1;

        [SerializeField]
        private int extraEnemyEveryWaves = 8;

        [SerializeField]
        private int maxEnemiesPerSpawn = 3;

        [Header("Mobile Portrait Tuning")]
        [SerializeField]
        private float mobilePortraitSpawnIntervalMultiplier = 1.12f;

        [SerializeField]
        private float mobilePortraitMaxEnemyMultiplier = 0.9f;

        [SerializeField]
        private float mobilePortraitSpawnBuffer = 1.1f;

        [SerializeField]
        private float mobilePortraitSpawnBand = 2.4f;

        [Header("Spawn Visibility")]
        [SerializeField]
        private float cameraSpawnMargin = 0.8f;

        [SerializeField]
        private float cameraSpawnBand = 1.8f;

        [Header("Boss Fight Tuning")]
        [SerializeField]
        private float bossFightSpawnIntervalMultiplier = 4f;

        [SerializeField]
        private float bossFightMaxEnemyMultiplier = 0.18f;

        [SerializeField]
        private int bossFightMinimumMaxEnemyCount = 6;

        [SerializeField]
        private int bossFightMaxEnemyCap = 24;

        [SerializeField]
        private int bossFightMaxEnemiesPerSpawn = 1;

        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
        private float spawnTimer;
        private float waveTimer;
        private float currentSpawnInterval;
        private int currentMaxEnemyCount;
        private int currentWave;
        private bool isWaveProgressPaused;
        private readonly HashSet<object> wavePauseSources = new HashSet<object>();
        private readonly HashSet<object> spawnPauseSources = new HashSet<object>();

        public event Action<int> WaveChanged;

        public int CurrentWave => currentWave;
        public float CurrentSpawnInterval => GetEffectiveCurrentSpawnInterval();
        public int CurrentMaxEnemyCount => GetEffectiveCurrentMaxEnemyCount();
        public int AliveEnemyCount => spawnedEnemies.Count;
        public float WaveProgress => waveDuration <= 0f ? 0f : Mathf.Clamp01(waveTimer / waveDuration);
        public bool IsWaveProgressPaused => isWaveProgressPaused || wavePauseSources.Count > 0;
        public bool IsEnemySpawnPaused => spawnPauseSources.Count > 0;

        private void Awake()
        {
            if (player == null)
                player = GameObject.Find("Player")?.transform;

            currentWave = startingWave;
            RecalculateWaveTuning();
        }

        private void Update()
        {
            if (player == null || GameState.IsGameOver)
                return;

            UpdateWaveTimer();
            RemoveMissingEnemies();

            if (IsEnemySpawnPaused)
                return;

            if (spawnedEnemies.Count >= CurrentMaxEnemyCount)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < CurrentSpawnInterval)
                return;

            spawnTimer = 0f;
            SpawnEnemyBatch();
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
            earlyEaseEndWave = Mathf.Max(1, earlyEaseEndWave);
            earlySpawnIntervalMultiplier = Mathf.Max(1f, earlySpawnIntervalMultiplier);
            earlyMaxEnemyMultiplier = Mathf.Clamp01(earlyMaxEnemyMultiplier);
            earlyMinimumMaxEnemyCount = Mathf.Max(0, earlyMinimumMaxEnemyCount);
            maxEnemyCountIncrease = Mathf.Max(0, maxEnemyCountIncrease);
            maxEnemyCountLimit = Mathf.Max(maxEnemyCount, maxEnemyCountLimit);
            enemiesPerSpawn = Mathf.Max(1, enemiesPerSpawn);
            extraEnemyEveryWaves = Mathf.Max(0, extraEnemyEveryWaves);
            maxEnemiesPerSpawn = Mathf.Max(enemiesPerSpawn, maxEnemiesPerSpawn);
            mobilePortraitSpawnIntervalMultiplier = Mathf.Max(1f, mobilePortraitSpawnIntervalMultiplier);
            mobilePortraitMaxEnemyMultiplier = Mathf.Clamp(mobilePortraitMaxEnemyMultiplier, 0.5f, 1f);
            mobilePortraitSpawnBuffer = Mathf.Max(0f, mobilePortraitSpawnBuffer);
            mobilePortraitSpawnBand = Mathf.Max(0.5f, mobilePortraitSpawnBand);
            cameraSpawnMargin = Mathf.Max(0f, cameraSpawnMargin);
            cameraSpawnBand = Mathf.Max(0.25f, cameraSpawnBand);
            bossFightSpawnIntervalMultiplier = Mathf.Max(1f, bossFightSpawnIntervalMultiplier);
            bossFightMaxEnemyMultiplier = Mathf.Clamp(bossFightMaxEnemyMultiplier, 0.1f, 1f);
            bossFightMinimumMaxEnemyCount = Mathf.Max(0, bossFightMinimumMaxEnemyCount);
            bossFightMaxEnemyCap = Mathf.Max(bossFightMinimumMaxEnemyCount, bossFightMaxEnemyCap);
            bossFightMaxEnemiesPerSpawn = Mathf.Max(1, bossFightMaxEnemiesPerSpawn);

            if (enemySpawnEntries == null)
                return;

            foreach (EnemySpawnEntry entry in enemySpawnEntries)
                entry?.Validate();
        }

        private void UpdateWaveTimer()
        {
            if (IsWaveProgressPaused)
                return;

            waveTimer += Time.deltaTime;

            if (waveTimer < waveDuration)
                return;

            waveTimer -= waveDuration;
            AdvanceWave();
        }

        private void AdvanceWave()
        {
            currentWave++;
            RecalculateWaveTuning();
            WaveChanged?.Invoke(currentWave);

            if (logWaveChanges)
                Debug.Log($"Wave {currentWave} started. Spawn Interval: {currentSpawnInterval:0.00}, Max Enemies: {currentMaxEnemyCount}");
        }

        private void RecalculateWaveTuning()
        {
            int waveOffset = Mathf.Max(0, currentWave - startingWave);
            currentSpawnInterval = Mathf.Max(minimumSpawnInterval, spawnInterval * Mathf.Pow(spawnIntervalMultiplier, waveOffset));
            currentSpawnInterval *= GetEarlySpawnIntervalMultiplier(currentWave);

            if (ShouldUseMobilePortraitTuning())
                currentSpawnInterval *= mobilePortraitSpawnIntervalMultiplier;

            int baseMaxEnemyCount = maxEnemyCount + maxEnemyCountIncrease * waveOffset;
            int easedMaxEnemyCount = Mathf.RoundToInt(baseMaxEnemyCount * GetEarlyMaxEnemyMultiplier(currentWave));
            currentMaxEnemyCount = Mathf.Min(maxEnemyCountLimit, Mathf.Max(earlyMinimumMaxEnemyCount, easedMaxEnemyCount));

            if (ShouldUseMobilePortraitTuning())
                currentMaxEnemyCount = Mathf.Max(earlyMinimumMaxEnemyCount, Mathf.RoundToInt(currentMaxEnemyCount * mobilePortraitMaxEnemyMultiplier));
        }

        private float GetEarlySpawnIntervalMultiplier(int wave)
        {
            if (wave >= earlyEaseEndWave)
                return 1f;

            float progress = Mathf.InverseLerp(1f, earlyEaseEndWave, wave);
            return Mathf.Lerp(earlySpawnIntervalMultiplier, 1f, progress);
        }

        private float GetEarlyMaxEnemyMultiplier(int wave)
        {
            if (wave >= earlyEaseEndWave)
                return 1f;

            float progress = Mathf.InverseLerp(1f, earlyEaseEndWave, wave);
            return Mathf.Lerp(earlyMaxEnemyMultiplier, 1f, progress);
        }

        /// <summary>
        /// 보스전처럼 현재 웨이브를 고정해야 할 때 웨이브 타이머만 멈춘다. 일반 적 생성은 계속 진행된다.
        /// </summary>
        public void SetWaveProgressPaused(bool paused)
        {
            isWaveProgressPaused = paused;
        }

        /// <summary>
        /// 보스처럼 여러 시스템이 동시에 웨이브 정지를 요청할 수 있으므로,
        /// 요청자별로 잠금을 관리해서 한쪽이 해제해도 다른 보스 잠금이 풀리지 않게 한다.
        /// </summary>
        public void SetWaveProgressPaused(object source, bool paused)
        {
            if (source == null)
            {
                SetWaveProgressPaused(paused);
                return;
            }

            if (paused)
                wavePauseSources.Add(source);
            else
                wavePauseSources.Remove(source);
        }

        public void SetEnemySpawningPaused(object source, bool paused)
        {
            if (source == null)
                return;

            if (paused)
                spawnPauseSources.Add(source);
            else
                spawnPauseSources.Remove(source);
        }

        private void SpawnEnemyBatch()
        {
            int spawnCount = GetSpawnCountForCurrentWave();

            for (int i = 0; i < spawnCount; i++)
            {
                if (spawnedEnemies.Count >= CurrentMaxEnemyCount)
                    return;

                SpawnEnemy();
            }
        }

        private int GetSpawnCountForCurrentWave()
        {
            int extraCount = extraEnemyEveryWaves <= 0 ? 0 : (currentWave - 1) / extraEnemyEveryWaves;
            int spawnCount = Mathf.Clamp(enemiesPerSpawn + extraCount, 1, maxEnemiesPerSpawn);

            if (IsWaveProgressPaused)
                return Mathf.Min(spawnCount, bossFightMaxEnemiesPerSpawn);

            return spawnCount;
        }

        private float GetEffectiveCurrentSpawnInterval()
        {
            if (!IsWaveProgressPaused)
                return currentSpawnInterval;

            return currentSpawnInterval * bossFightSpawnIntervalMultiplier;
        }

        private int GetEffectiveCurrentMaxEnemyCount()
        {
            if (!IsWaveProgressPaused)
                return currentMaxEnemyCount;

            int bossFightMax = Mathf.RoundToInt(currentMaxEnemyCount * bossFightMaxEnemyMultiplier);
            bossFightMax = Mathf.Min(bossFightMax, bossFightMaxEnemyCap);
            return Mathf.Max(bossFightMinimumMaxEnemyCount, bossFightMax);
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
                && currentWave >= entry.UnlockWave
                && (entry.MaxWave <= 0 || currentWave <= entry.MaxWave);
        }

        private Vector2 GetRandomSpawnPosition()
        {
            // 원형 방향을 무작위로 뽑고, 최소/최대 거리 사이에 적을 생성한다.
            if (TryGetCameraOutsideSpawnPosition(out Vector2 offscreenPosition))
                return offscreenPosition;

            Vector2 playerPosition = player.position;
            GetEffectiveSpawnDistanceRange(out float effectiveMinDistance, out float effectiveMaxDistance);

            for (int i = 0; i < 16; i++)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float distance = UnityEngine.Random.Range(effectiveMinDistance, effectiveMaxDistance);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidatePosition = MapBoundary.ClampToPlayableArea(playerPosition + direction * distance);

                if ((candidatePosition - playerPosition).sqrMagnitude >= effectiveMinDistance * effectiveMinDistance * 0.64f)
                    return candidatePosition;
            }

            return MapBoundary.ClampToPlayableArea(playerPosition + Vector2.right * effectiveMinDistance);
        }

        private bool TryGetCameraOutsideSpawnPosition(out Vector2 spawnPosition)
        {
            spawnPosition = default;

            Camera mainCamera = Camera.main;

            if (mainCamera == null || player == null)
                return false;

            Rect cameraRect = GetCameraWorldRect(mainCamera);
            Rect playableRect = GetPlayableSpawnRect();
            GetEffectiveSpawnDistanceRange(out float effectiveMinDistance, out float effectiveMaxDistance);

            for (int i = 0; i < 24; i++)
            {
                Vector2 candidate = GetRandomPositionOutsideCamera(cameraRect);
                candidate.x = Mathf.Clamp(candidate.x, playableRect.xMin, playableRect.xMax);
                candidate.y = Mathf.Clamp(candidate.y, playableRect.yMin, playableRect.yMax);

                if (cameraRect.Contains(candidate))
                    continue;

                float sqrDistance = ((Vector2)player.position - candidate).sqrMagnitude;

                if (sqrDistance < effectiveMinDistance * effectiveMinDistance * 0.64f)
                    continue;

                if (sqrDistance > effectiveMaxDistance * effectiveMaxDistance * 2.25f)
                    continue;

                spawnPosition = candidate;
                return true;
            }

            return false;
        }

        private Vector2 GetRandomPositionOutsideCamera(Rect cameraRect)
        {
            float offset = cameraSpawnMargin + UnityEngine.Random.Range(0f, cameraSpawnBand);
            int side = UnityEngine.Random.Range(0, 4);

            switch (side)
            {
                case 0:
                    return new Vector2(UnityEngine.Random.Range(cameraRect.xMin, cameraRect.xMax), cameraRect.yMax + offset);
                case 1:
                    return new Vector2(cameraRect.xMax + offset, UnityEngine.Random.Range(cameraRect.yMin, cameraRect.yMax));
                case 2:
                    return new Vector2(UnityEngine.Random.Range(cameraRect.xMin, cameraRect.xMax), cameraRect.yMin - offset);
                default:
                    return new Vector2(cameraRect.xMin - offset, UnityEngine.Random.Range(cameraRect.yMin, cameraRect.yMax));
            }
        }

        private static Rect GetCameraWorldRect(Camera mainCamera)
        {
            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;
            Vector3 cameraPosition = mainCamera.transform.position;

            return new Rect(cameraPosition.x - halfWidth, cameraPosition.y - halfHeight, halfWidth * 2f, halfHeight * 2f);
        }

        private Rect GetPlayableSpawnRect()
        {
            if (MapBoundary.TryGetWorldBounds(out Bounds bounds))
            {
                float inset = Mathf.Min(Mathf.Max(0.1f, cameraSpawnMargin * 0.5f), Mathf.Min(bounds.size.x, bounds.size.y) * 0.45f);
                return Rect.MinMaxRect(bounds.min.x + inset, bounds.min.y + inset, bounds.max.x - inset, bounds.max.y - inset);
            }

            GetEffectiveSpawnDistanceRange(out float effectiveMinDistance, out float effectiveMaxDistance);
            Vector2 center = player.position;
            float radius = Mathf.Max(effectiveMaxDistance, effectiveMinDistance + cameraSpawnBand);
            return Rect.MinMaxRect(center.x - radius, center.y - radius, center.x + radius, center.y + radius);
        }

        private void GetEffectiveSpawnDistanceRange(out float effectiveMinDistance, out float effectiveMaxDistance)
        {
            effectiveMinDistance = minSpawnDistance;
            effectiveMaxDistance = maxSpawnDistance;

            if (!ShouldUseMobilePortraitTuning() || Camera.main == null)
                return;

            Camera mainCamera = Camera.main;
            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;
            float visibleRadius = Mathf.Max(halfWidth, halfHeight);

            effectiveMinDistance = Mathf.Max(minSpawnDistance, visibleRadius + mobilePortraitSpawnBuffer);
            effectiveMaxDistance = Mathf.Max(effectiveMinDistance + 0.5f, effectiveMinDistance + mobilePortraitSpawnBand);
        }

        private static bool ShouldUseMobilePortraitTuning()
        {
            return GameOptions.IsMobileDisplayMode && Screen.height > Screen.width;
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
