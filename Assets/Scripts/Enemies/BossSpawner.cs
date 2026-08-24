using UnityEngine;
using VampireLike.Combat;
using System;
using System.Collections;
using VampireLike.Growth;
using VampireLike.Audio;
using VampireLike.World;

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
        private BossPhaseDecorationController bossPhaseDecorationController;

        [SerializeField]
        private int bossWaveInterval = 5;

        [SerializeField]
        private float minSpawnDistance = 5f;

        [SerializeField]
        private float maxSpawnDistance = 7f;

        [SerializeField]
        private float cameraSafeSpawnPadding = 2f;

        // 보스별 프리팹 수치를 기본 기준으로 사용한다. 전체 보정이 필요할 때만 Inspector에서 배율을 올린다.
        [SerializeField]
        private float baseBossHealthMultiplier = 1f;

        [SerializeField]
        private float bossHealthBalanceMultiplier = 1.05f;

        [SerializeField]
        private float healthMultiplierPerBossStage = 1.12f;

        [SerializeField]
        private float healthMultiplierPerAppearance = 1f;

        [SerializeField]
        private float contactDamageMultiplierPerBossStage = 1.12f;

        [SerializeField]
        private float contactDamageMultiplierPerAppearance = 1f;

        [SerializeField]
        private float patternDamageMultiplierPerBossStage = 1.12f;

        [SerializeField]
        private float moveSpeedMultiplierPerAppearance = 1f;

        [SerializeField]
        private float maxBossMoveSpeed = 1.8f;

        [SerializeField]
        private float bossArenaBoundsScale = 0.5f;

        [SerializeField]
        private float bossArenaMinWidth = 18f;

        [SerializeField]
        private float bossArenaMinHeight = 12f;

        [SerializeField]
        private float bossArenaScreenPadding = 0.25f;

        [SerializeField]
        private float bossArenaTopHudPadding = 1.25f;

        [SerializeField]
        private float bossArenaBottomPadding = 0f;

        [SerializeField]
        private float bossArenaPlayerBottomVisualAllowance = 2f;

        [SerializeField]
        private float bossSpawnPlayerSeparation = 1.4f;

        [SerializeField]
        private float bossDeathGemAttractDelay = 0.45f;

        private GameObject activeBoss;
        private EnemyHealth activeBossHealth;
        private int activeBossStage;
        private int lastBossSpawnWave;
        private bool hasPausedWaveProgress;
        private bool hasClearedBossPhaseDecorations;
        private Camera mainCamera;

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

            ResolveBossPhaseDecorationController();
            mainCamera = Camera.main;
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
            RestoreBossPhaseDecorationsIfNeeded();

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
                MapBoundary.ClearTemporaryBounds(this);
                RestoreBossPhaseDecorationsIfNeeded();
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
            cameraSafeSpawnPadding = Mathf.Max(0f, cameraSafeSpawnPadding);
            baseBossHealthMultiplier = Mathf.Max(1f, baseBossHealthMultiplier);
            bossHealthBalanceMultiplier = Mathf.Max(1f, bossHealthBalanceMultiplier);
            healthMultiplierPerBossStage = Mathf.Max(1f, healthMultiplierPerBossStage);
            healthMultiplierPerAppearance = Mathf.Max(1f, healthMultiplierPerAppearance);
            contactDamageMultiplierPerBossStage = Mathf.Max(1f, contactDamageMultiplierPerBossStage);
            contactDamageMultiplierPerAppearance = Mathf.Max(1f, contactDamageMultiplierPerAppearance);
            patternDamageMultiplierPerBossStage = Mathf.Max(1f, patternDamageMultiplierPerBossStage);
            moveSpeedMultiplierPerAppearance = Mathf.Max(1f, moveSpeedMultiplierPerAppearance);
            maxBossMoveSpeed = Mathf.Max(0.1f, maxBossMoveSpeed);
            bossArenaBoundsScale = Mathf.Clamp(bossArenaBoundsScale, 0.3f, 1f);
            bossArenaMinWidth = Mathf.Max(6f, bossArenaMinWidth);
            bossArenaMinHeight = Mathf.Max(6f, bossArenaMinHeight);
            bossArenaScreenPadding = Mathf.Max(0f, bossArenaScreenPadding);
            bossArenaTopHudPadding = Mathf.Max(0f, bossArenaTopHudPadding);
            bossArenaBottomPadding = Mathf.Max(0f, bossArenaBottomPadding);
            bossArenaPlayerBottomVisualAllowance = Mathf.Max(0f, bossArenaPlayerBottomVisualAllowance);
            bossSpawnPlayerSeparation = Mathf.Max(0f, bossSpawnPlayerSeparation);
            bossDeathGemAttractDelay = Mathf.Max(0f, bossDeathGemAttractDelay);

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

            SetWaveProgressPaused(true);
            Bounds arenaBounds = ActivateBossArena();
            ClearBossPhaseDecorations();
            Vector2 spawnPosition = GetBossSpawnPosition(arenaBounds);
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
            Debug.Log($"Boss appeared - Wave {wave}");
            GameSfx.Play(SfxType.BossAppear);
            GameBgm.PlayBoss(bossStage);
            BossSpawned?.Invoke(bossStage, activeBoss);
        }

        private void HandleActiveBossDied(EnemyHealth defeatedBoss)
        {
            int defeatedStage = activeBossStage;
            BossDefeated?.Invoke(defeatedStage, defeatedBoss);
            StartCoroutine(AttractFieldExperienceGemsAfterDelay());
            GameBgm.Play(BgmType.Battle);
            UnsubscribeActiveBossDeath();
            MapBoundary.ClearTemporaryBounds(this);
            RestoreBossPhaseDecorationsIfNeeded();
        }

        private void UnsubscribeActiveBossDeath()
        {
            if (activeBossHealth != null)
                activeBossHealth.Died -= HandleActiveBossDied;
        }

        private void SetWaveProgressPaused(bool paused)
        {
            if (enemySpawner == null)
                return;

            hasPausedWaveProgress = paused;
            enemySpawner.SetWaveProgressPaused(this, paused);
        }

        private void ResolveBossPhaseDecorationController()
        {
            if (bossPhaseDecorationController != null)
                return;

            bossPhaseDecorationController = FindFirstObjectByType<BossPhaseDecorationController>();

            if (bossPhaseDecorationController != null)
                return;

            GameObject decorationRoot = GameObject.Find("Decorations");

            if (decorationRoot != null)
                bossPhaseDecorationController = decorationRoot.AddComponent<BossPhaseDecorationController>();
        }

        private void ClearBossPhaseDecorations()
        {
            ResolveBossPhaseDecorationController();

            if (bossPhaseDecorationController == null || !bossPhaseDecorationController.HasManagedDecorations)
                return;

            bossPhaseDecorationController.ClearForBossPhase();
            hasClearedBossPhaseDecorations = true;
        }

        private void RestoreBossPhaseDecorationsIfNeeded()
        {
            if (!hasClearedBossPhaseDecorations)
                return;

            if (bossPhaseDecorationController != null)
                bossPhaseDecorationController.RestoreIfCleared();

            hasClearedBossPhaseDecorations = false;
        }

        private Bounds ActivateBossArena()
        {
            if (player == null || !MapBoundary.TryGetBaseWorldBounds(out Bounds baseBounds))
                return default;

            Bounds arenaBounds = TryGetCameraWorldBounds(baseBounds, out Bounds cameraBounds)
                ? cameraBounds
                : CreateFallbackArenaBounds(baseBounds);

            arenaBounds = ApplyBossArenaBottomVisualAllowance(arenaBounds, baseBounds);
            MapBoundary.OverrideTemporaryBounds(this, arenaBounds);
            return arenaBounds;
        }

        private Vector2 GetBossSpawnPosition(Bounds arenaBounds)
        {
            Vector2 spawnPosition = arenaBounds.size == Vector3.zero
                ? (Vector2)(mainCamera == null ? player.position : mainCamera.transform.position)
                : (Vector2)arenaBounds.center;

            if (player == null)
                return MapBoundary.ClampToPlayableArea(spawnPosition);

            Vector2 fromPlayer = spawnPosition - (Vector2)player.position;
            float minDistance = bossSpawnPlayerSeparation;

            if (minDistance <= 0f || fromPlayer.sqrMagnitude >= minDistance * minDistance)
                return MapBoundary.ClampToPlayableArea(spawnPosition);

            Vector2 direction = fromPlayer.sqrMagnitude <= 0.001f ? Vector2.up : fromPlayer.normalized;
            return MapBoundary.ClampToPlayableArea((Vector2)player.position + direction * minDistance);
        }

        private IEnumerator AttractFieldExperienceGemsAfterDelay()
        {
            if (bossDeathGemAttractDelay > 0f)
                yield return new WaitForSeconds(bossDeathGemAttractDelay);

            PlayerExperience playerExperience = player == null ? null : player.GetComponent<PlayerExperience>();

            if (playerExperience == null)
                yield break;

            ExperienceGem[] gems = FindObjectsByType<ExperienceGem>(FindObjectsSortMode.None);

            foreach (ExperienceGem gem in gems)
            {
                if (gem == null || gem.IsClaimed)
                    continue;

                gem.StartAttract(playerExperience);
            }
        }

        private Bounds CreateFallbackArenaBounds(Bounds baseBounds)
        {
            float arenaWidth = Mathf.Clamp(baseBounds.size.x * bossArenaBoundsScale, bossArenaMinWidth, baseBounds.size.x);
            float arenaHeight = Mathf.Clamp(baseBounds.size.y * bossArenaBoundsScale, bossArenaMinHeight, baseBounds.size.y);
            Vector3 arenaSize = new Vector3(arenaWidth, arenaHeight, baseBounds.size.z);
            Vector3 center = player.position;
            ClampArenaCenterToBaseBounds(ref center, arenaSize, baseBounds);

            return new Bounds(center, arenaSize);
        }

        private bool TryGetCameraWorldBounds(Bounds baseBounds, out Bounds cameraBounds)
        {
            cameraBounds = default;

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null || !mainCamera.orthographic)
                return false;

            float halfHeight = Mathf.Max(0.1f, mainCamera.orthographicSize - bossArenaScreenPadding);
            float halfWidth = Mathf.Max(0.1f, mainCamera.orthographicSize * mainCamera.aspect - bossArenaScreenPadding);
            Vector3 arenaSize = new Vector3(halfWidth * 2f, halfHeight * 2f, baseBounds.size.z);
            Vector3 center = mainCamera.transform.position;
            center.z = baseBounds.center.z;

            ApplyScreenSafeBossArenaPadding(ref center, ref arenaSize);
            ClampArenaCenterToBaseBounds(ref center, arenaSize, baseBounds);
            cameraBounds = new Bounds(center, arenaSize);
            return true;
        }

        private void ApplyScreenSafeBossArenaPadding(ref Vector3 center, ref Vector3 arenaSize)
        {
            float topPadding = Mathf.Max(0f, bossArenaTopHudPadding);
            float bottomPadding = Mathf.Max(0f, bossArenaBottomPadding);
            float maxPadding = Mathf.Max(0f, arenaSize.y - 1f);

            if (topPadding + bottomPadding > maxPadding)
            {
                float paddingScale = maxPadding / Mathf.Max(0.01f, topPadding + bottomPadding);
                topPadding *= paddingScale;
                bottomPadding *= paddingScale;
            }

            arenaSize.y = Mathf.Max(1f, arenaSize.y - topPadding - bottomPadding);
            center.y += (bottomPadding - topPadding) * 0.5f;
        }

        private Bounds ApplyBossArenaBottomVisualAllowance(Bounds arenaBounds, Bounds baseBounds)
        {
            // 플레이어 기준점이 발밑에 가까우면 아래쪽 경계가 더 좁게 느껴져 보스전에서만 하단 여유를 준다.
            float allowance = Mathf.Min(
                bossArenaPlayerBottomVisualAllowance,
                Mathf.Max(0f, arenaBounds.min.y - baseBounds.min.y));

            if (allowance <= 0f)
                return arenaBounds;

            float minY = arenaBounds.min.y - allowance;
            float maxY = arenaBounds.max.y;
            Vector3 center = arenaBounds.center;
            Vector3 size = arenaBounds.size;

            center.y = (minY + maxY) * 0.5f;
            size.y = maxY - minY;

            return new Bounds(center, size);
        }

        private static void ClampArenaCenterToBaseBounds(ref Vector3 center, Vector3 arenaSize, Bounds baseBounds)
        {
            float halfWidth = arenaSize.x * 0.5f;
            float halfHeight = arenaSize.y * 0.5f;

            center.x = arenaSize.x >= baseBounds.size.x
                ? baseBounds.center.x
                : Mathf.Clamp(center.x, baseBounds.min.x + halfWidth, baseBounds.max.x - halfWidth);

            center.y = arenaSize.y >= baseBounds.size.y
                ? baseBounds.center.y
                : Mathf.Clamp(center.y, baseBounds.min.y + halfHeight, baseBounds.max.y - halfHeight);

            center.z = baseBounds.center.z;
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
            int bossStage = Mathf.Max(1, wave / bossWaveInterval);
            int stageOffset = Mathf.Max(0, bossStage - 1);
            float healthMultiplier = baseBossHealthMultiplier
                * bossHealthBalanceMultiplier
                * Mathf.Pow(healthMultiplierPerBossStage, stageOffset)
                * Mathf.Pow(healthMultiplierPerAppearance, stageOffset);
            float damageMultiplier = Mathf.Pow(contactDamageMultiplierPerBossStage, stageOffset)
                * Mathf.Pow(contactDamageMultiplierPerAppearance, stageOffset);
            float patternDamageMultiplier = Mathf.Pow(patternDamageMultiplierPerBossStage, stageOffset);
            float speedMultiplier = Mathf.Pow(moveSpeedMultiplierPerAppearance, stageOffset);

            EnemyHealth enemyHealth = boss.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.SetMaxHealth(Mathf.RoundToInt(enemyHealth.MaxHealth * healthMultiplier));

            EnemyContactDamage contactDamage = boss.GetComponent<EnemyContactDamage>();
            if (contactDamage != null)
                contactDamage.SetContactDamage(Mathf.RoundToInt(contactDamage.ContactDamage * damageMultiplier));

            EnemyController enemyController = boss.GetComponent<EnemyController>();
            if (enemyController != null)
                enemyController.SetMoveSpeed(Mathf.Min(maxBossMoveSpeed, enemyController.MoveSpeed * speedMultiplier));

            ApplyBossPatternDamageScaling(boss, patternDamageMultiplier);
        }

        private void ApplyBossPatternDamageScaling(GameObject boss, float multiplier)
        {
            if (boss == null || multiplier <= 0f)
                return;

            MonoBehaviour[] behaviours = boss.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IBossDamageScaler scaler)
                    scaler.ScaleBossDamage(multiplier);
            }
        }

        private Vector2 GetRandomSpawnPosition()
        {
            const int MaxAttempts = 24;

            for (int i = 0; i < MaxAttempts; i++)
            {
                Vector2 position = GetRandomPositionAroundPlayer();

                if (IsInsideCameraSafeArea(position))
                    return position;
            }

            Vector2 fallbackPosition = GetRandomPositionAroundPlayer();
            return ClampToCameraSafeArea(fallbackPosition);
        }

        private Vector2 GetRandomPositionAroundPlayer()
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return MapBoundary.ClampToPlayableArea((Vector2)player.position + direction * distance);
        }

        private bool IsInsideCameraSafeArea(Vector2 position)
        {
            if (!TryGetCameraSafeBounds(out Vector2 min, out Vector2 max))
                return true;

            return position.x >= min.x
                && position.x <= max.x
                && position.y >= min.y
                && position.y <= max.y;
        }

        private Vector2 ClampToCameraSafeArea(Vector2 position)
        {
            if (!TryGetCameraSafeBounds(out Vector2 min, out Vector2 max))
                return position;

            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(position.x, min.x, max.x),
                Mathf.Clamp(position.y, min.y, max.y));

            return MapBoundary.ClampToPlayableArea(clampedPosition);
        }

        private bool TryGetCameraSafeBounds(out Vector2 min, out Vector2 max)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null || !mainCamera.orthographic)
            {
                min = Vector2.zero;
                max = Vector2.zero;
                return false;
            }

            Vector2 center = mainCamera.transform.position;
            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;
            float padding = cameraSafeSpawnPadding;

            min = new Vector2(center.x - halfWidth + padding, center.y - halfHeight + padding);
            max = new Vector2(center.x + halfWidth - padding, center.y + halfHeight - padding);

            if (min.x > max.x || min.y > max.y)
                return false;

            return true;
        }
    }
}
