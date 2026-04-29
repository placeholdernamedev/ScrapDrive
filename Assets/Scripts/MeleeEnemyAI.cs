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
        agent = GetComponent<NavMeshAgent>();

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            Attack();
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
        agent.isStopped = true;
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
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the enemy upright

        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

