using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public Transform car;

    public int swarmSize = 5;
    public float spawnRadius = 10f;
    public float spawnInterval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnSwarm();
            timer = 0f;
        }
    }

    void SpawnSwarm()
    {
        for (int i = 0; i < swarmSize; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;

            Vector3 spawnPosition = transform.position + randomOffset;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            MeleeEnemyAI enemyAI = enemy.GetComponent<MeleeEnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.player = player;
                enemyAI.car = car;
            }
        }
    }
}