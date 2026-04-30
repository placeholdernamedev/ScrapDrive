using UnityEngine;
using UnityEngine.AI;

public class SnapToNavMeshOnStart : MonoBehaviour
{
    public float maxDistance = 5f;

    void Start()
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;
        if (agent.isOnNavMesh) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }
}
