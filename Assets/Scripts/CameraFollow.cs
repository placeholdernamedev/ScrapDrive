using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Position")]
    public float distance = 7f;
    public float height = 3f;
    public float positionSmoothTime = 0.1f;

    [Header("Rotation")]
    public float lookAhead = 3f;

    private Vector3 positionVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        FollowPosition();
        FollowRotation();
    }

    void FollowPosition()
    {
        // Get movement direction (prevents reverse camera issues)
        Vector3 moveDir = target.forward;

        Vector3 desiredPosition =
            target.position
            - moveDir * distance
            + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime
        );
    }

    void FollowRotation()
    {
        Vector3 lookTarget = target.position + target.forward * lookAhead;

        Quaternion targetRotation = Quaternion.LookRotation(
            lookTarget - transform.position
        );

        transform.rotation = targetRotation; // instant rotation (snappy)
    }

    // Call this when entering/exiting car
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}