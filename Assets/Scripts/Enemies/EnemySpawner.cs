using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 플레이어 주변 바깥쪽 원형 범위에서 적을 일정 간격으로 생성한다.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        // 생성할 Enemy 프리팹이다.
        [SerializeField]
        private GameObject enemyPrefab;

        // 생성 기준이 되는 플레이어 위치다.
        [SerializeField]
        private Transform player;

        // 적 생성 시간 간격이다.
        [SerializeField]
        private float spawnInterval = 2f;

        // 플레이어에게 너무 가까이 생성되지 않도록 하는 최소 거리다.
        [SerializeField]
        private float minSpawnDistance = 4f;

        // 플레이어 주변 어느 정도 바깥까지 생성할지 정하는 최대 거리다.
        [SerializeField]
        private float maxSpawnDistance = 6f;

        // 생성된 적이 무한정 쌓이지 않도록 제한한다.
        [SerializeField]
        private int maxEnemyCount = 30;

        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
        private float spawnTimer;

        private void Awake()
        {
            if (player == null)
                player = GameObject.Find("Player")?.transform;
        }

        private void Update()
        {
            if (enemyPrefab == null || player == null || GameState.IsGameOver)
                return;

            RemoveMissingEnemies();

            if (spawnedEnemies.Count >= maxEnemyCount)
                return;

            spawnTimer += Time.deltaTime;

            if (spawnTimer < spawnInterval)
                return;

            spawnTimer = 0f;
            SpawnEnemy();
        }

        private void OnValidate()
        {
            spawnInterval = Mathf.Max(0.1f, spawnInterval);
            minSpawnDistance = Mathf.Max(0f, minSpawnDistance);
            maxSpawnDistance = Mathf.Max(minSpawnDistance, maxSpawnDistance);
            maxEnemyCount = Mathf.Max(0, maxEnemyCount);
        }

        private void SpawnEnemy()
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            spawnedEnemies.Add(enemy);
        }

        private Vector2 GetRandomSpawnPosition()
        {
            // 원형 방향을 랜덤으로 뽑고, 최소/최대 거리 사이에 적을 생성한다.
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
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
