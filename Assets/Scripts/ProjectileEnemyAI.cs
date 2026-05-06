using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileEnemyAI : MonoBehaviour
{

    public Transform player;
    public Transform car;
    public LineRenderer laserbeam;
    public Transform firePoint;

    public float patrolRange = 10f;
    public float waitAtPointTime = 2f;

    public float detectionRange = 20f;
    public float attackRange = 15f;
    public float attackCoolDown = 3f;
    public int damage = 20;

    public float laserbeamduration = 0.5f;

    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 patrolCenter;
    private float waitTime;
    private float attackTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        patrolCenter = transform.position;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (car == null && player != null)
        {
            car = player;
        }

        if (laserbeam != null)
        {
            laserbeam.enabled = false;
        }

        PickNewPatrolPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (car == null) return;

        float distanceToCar = Vector3.Distance(transform.position, car.position);

        if (distanceToCar <= detectionRange)
        {
            StopAndAttack();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        agent.isStopped = false;

        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("IsAttacking", false);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTime += Time.deltaTime;

            if (waitTime >= waitAtPointTime)
            {
                PickNewPatrolPoint();
                waitTime = 0f;
            }
        }
    }

    void PickNewPatrolPoint()
    {
        Vector3 randomPoint = patrolCenter + UnityEngine.Random.insideUnitSphere * patrolRange;
        randomPoint.y = transform.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void StopAndAttack()
    {
        agent.isStopped = true;

        animator.SetFloat("Speed", 0);
        animator.SetBool("IsAttacking", true);

        Vector3 lookDirection = car.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6f);
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCoolDown)
        {
            FireLaser();
            attackTimer = 0f;
        }
    }

    void FireLaser()
    {
        Vector3 origin = firePoint.position;
        Vector3 target = car.position + Vector3.up * 1f;
        Vector3 direction = (target - origin).normalized;

        Vector3 laserEndPoint = origin + direction * attackRange;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            laserEndPoint = hit.point;

            if (hit.collider.CompareTag("Player"))
            {
                UnityEngine.Debug.Log("Laser hit player for " + damage + " damage");

                // Add your player/car health script here later:
                // hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }

        laserbeam.enabled = true;
        laserbeam.SetPosition(0, origin);
        laserbeam.SetPosition(1, laserEndPoint);

        CancelInvoke(nameof(HideLaser));
        Invoke(nameof(HideLaser), laserbeamduration);
    }

    void HideLaser()
    {
        if (laserbeam != null)
        {
            laserbeam.enabled = false;
        }
    }

}
