using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform car;

    private Transform currentTarget;

    public float detectionRange = 12f;
    public float attackRange = 4f;
    public float attackCoolDown = 1f;
    public int damage = 10;

    private NavMeshAgent agent;
    private Animator animator;

    private VehicleInteraction vi;
    private float lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (car != null)
        {
           vi = car.GetComponent<VehicleInteraction>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (player == null && car == null) return;

        if (vi != null && vi.InVehicle && car != null)
        {
            currentTarget = car;
        }
        else
        {
            currentTarget = player;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget < attackRange)
        {
            Attack();
        }
        else if (distanceToTarget < detectionRange)
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
        FaceTarget();

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
            
        if (Time.time >= lastAttackTime + attackCoolDown)
        {

            if (animator != null)
            {
                Debug.Log("Attack trigger fired");
                animator.SetTrigger("Attack");
            }

            IDamageable damageable = currentTarget.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            Debug.Log("Enemy attacked the " + currentTarget.name + " for " + damage + " damage!");

            lastAttackTime = Time.time;
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(currentTarget.position);

        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0; // Keep the enemy upright

        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

