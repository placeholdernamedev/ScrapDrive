using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform currentTarget;

    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 10f;
    public float rotationSpeed = 12f;

    void LateUpdate()
    {
        if (!currentTarget) return;

        // Position
        Vector3 desiredPosition = currentTarget.position + currentTarget.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Rotation
        Quaternion desiredRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}