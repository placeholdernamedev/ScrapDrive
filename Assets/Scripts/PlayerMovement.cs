using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;

    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        ResolveCameraTransform();

        velocity.y = -2f; // stick to ground
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            ResolveCameraTransform();
        }
        if (controller == null || !controller.enabled || !gameObject.activeInHierarchy)
            return;
        else{
            MovePlayer();
            ApplyGravity();
            UpdateAnimation();
        }
    }

    void ResolveCameraTransform()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void MovePlayer()
    {
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

        // Apply movement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Face the direction the camera is looking
        if (cameraTransform != null)
        {
            Vector3 lookDir = cameraTransform.forward;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(x, z);
        float speed = Mathf.Clamp01(input.magnitude);

        anim.SetFloat("Speed", speed);
    }
}