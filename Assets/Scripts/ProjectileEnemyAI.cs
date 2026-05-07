
using UnityEngine;
using UnityEngine.AI;

public class ProjectileEnemyAI : MonoBehaviour
{

    public Transform player;
    public Transform car;
    

    public Transform firePoint;

    public float patrolRange = 10f;
    public float waitAtPointTime = 2f;

    public LayerMask hitMask;

    public float detectionRange = 20f;
    public float attackRange = 15f;
    public float attackCoolDown = 3f;
    public int damage = 20;

    public float laserbeamduration = 0.5f;

    public AudioSource gunAudioSource;
    public AudioClip gunSound;

    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 patrolCenter;
    private float waitTime;
    private float attackTimer;
    private Transform currentTarget;
    private VehicleInteraction vi;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
       
        patrolCenter = transform.position;

        if (car != null)
        {
            vi = car.GetComponent<VehicleInteraction>();
        }

        PickNewPatrolPoint();
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

        if (distanceToTarget <= detectionRange)
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
        if (currentTarget == null) return;

        agent.isStopped = true;

        animator.SetFloat("Speed", 0);

        Vector3 lookDirection = currentTarget.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6f);
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCoolDown)
        {
            animator.SetTrigger("Attack");
            attackTimer = 0f;
        }
    }

    void FireLaser()
    {
        if (firePoint == null || currentTarget == null) return;

        Vector3 targetPoint = currentTarget.position + Vector3.up * 1f;
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        Ray ray = new Ray(firePoint.position, direction);
        RaycastHit hit;

        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, attackRange, hitMask))
        {
            endPoint = hit.point;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

                UnityEngine.Debug.Log("Laser hit " + hit.collider.name + " for " + damage + " damage");
        }
        else
        {
            endPoint = firePoint.position + direction * attackRange;
        }
        gunAudioSource.PlayOneShot(gunSound); 

        StartCoroutine(DrawLaser(firePoint.position, endPoint));
    }

    System.Collections.IEnumerator DrawLaser(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("ShotLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        // creates a material for the line
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // set color to red
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        yield return new WaitForSeconds(laserbeamduration);

        Destroy(lineObj);
    }
}
