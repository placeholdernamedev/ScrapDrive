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
    public int swarmSize1 = 5;
    public int swarmSize2 = 5;
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
