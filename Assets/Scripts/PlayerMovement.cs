using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private CameraFollow cameraFollow;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        ResolveCameraTransform();

        velocity.y = -2f; // forces player onto ground initially
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            ResolveCameraTransform();
        }

        MovePlayer();
    }

    void ResolveCameraTransform()
    {
        if (cameraTransform != null) return;

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            return;
        }

        cameraFollow = FindFirstObjectByType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraTransform = cameraFollow.transform;
        }
    }

    void MovePlayer()
    {
        if (!enabled || controller == null || !controller.enabled) return;
        
        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move;
        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            move = camRight * x + camForward * z;
        }
        else
        {
            move = transform.right * x + transform.forward * z;
        }

        // Movement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Optional: rotate toward movement direction
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}