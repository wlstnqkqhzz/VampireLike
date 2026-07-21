using System.Collections.Generic;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField]
        private Transform player;

        [SerializeField]
        private float spawnInterval = 2f;

        [SerializeField]
        private float minSpawnDistance = 4f;

        [SerializeField]
        private float maxSpawnDistance = 6f;

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
