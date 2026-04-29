using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 12f;
    public float attackRange = 2f;
    public float attackCoolDown = 1f;
    public int damage = 10;

    private NavMeshAgent agent;
    private float lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent - GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No Object with tag 'Player' has been found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(CryptoAPITransform.position, player.posiotion);

        if (distanceToPlayer < attackRange)
        {
            attackCoolDown();
        }
        else if (distanceToPlayer < detectionRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Attack()
    {
        agent.IsStopped = true;
        FacePlayer();

        if (Time.time >= lastAttackTime + attackCoolDown)
        {
            Debug.Log("Enemy attacked the player for " + damage + " damage!");
            lastAttackTime = Time.time;
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Idle()
    {
        agent.isStopped = true;
    }

    void FacePlayer()
    {
        Vector3 direction = player.postion - transform.position;
        direction.y = 0; // Keep the enemy upright

        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

