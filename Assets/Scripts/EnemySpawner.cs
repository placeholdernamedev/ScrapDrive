using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public Transform player;
    public Transform car;

    public int startingSwarm1 = 3;
    public int startingSwarm2 = 3;
    public int swarmSize1 = 5; // melee spawns
    public int swarmSize2 = 5; // projectile spawns
    public float spawnRadius = 10f; // Radius around the spawner where enemies will spawn
    public float spawnInterval = 5f; // Time interval between spawns

    private float timer; // Timer to track time passed



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
        // Spawn melee enemies only if Enemy Prefab 1 is assigned
        if (enemyPrefab1 != null)
        {
            for (int i = 0; i < swarmSize1; i++)
            {
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0;

                Vector3 spawnPosition = transform.position + randomOffset;

                GameObject enemy = Instantiate(enemyPrefab1, spawnPosition, Quaternion.identity);

                MeleeEnemyAI enemyAI = enemy.GetComponent<MeleeEnemyAI>();

                if (enemyAI != null)
                {
                    enemyAI.player = player;
                    enemyAI.car = car;
                }
            }
        }

        // Spawn projectile enemies only if Enemy Prefab 2 is assigned
        if (enemyPrefab2 != null)
        {
            for (int i = 0; i < swarmSize2; i++)
            {
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0;

                Vector3 spawnPosition = transform.position + randomOffset;

                GameObject enemy = Instantiate(enemyPrefab2, spawnPosition, Quaternion.identity);

                ProjectileEnemyAI enemyAI = enemy.GetComponent<ProjectileEnemyAI>();

                if (enemyAI != null)
                {
                    enemyAI.player = player;
                    enemyAI.car = car;
                }
            }
        }
    }
}
