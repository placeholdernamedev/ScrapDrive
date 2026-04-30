using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform currentTarget;

    public Vector3 offset = new Vector3(0, 3, -6);
    public float mouseSensitivity = 4f;
    public float minPitch = -35f;
    public float maxPitch = 65f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;

        // Start with free cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (!currentTarget) return;
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButton(1)) // Hold RMB to control camera
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPosition = currentTarget.position + orbitRotation * offset;
        transform.position = desiredPosition;

        Quaternion desiredRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        transform.rotation = desiredRotation;
    }
}