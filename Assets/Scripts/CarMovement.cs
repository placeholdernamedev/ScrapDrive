using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    public float maxSpeed = 15f;
    public float acceleration = 30f;
    public float deceleration = 40f;
    public float turnSpeed = 100f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Helps prevent tipping and reduces jitter
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");   // W/S
        turnInput = Input.GetAxis("Horizontal"); // A/D
    }

    void FixedUpdate()
    {
        // Current forward speed
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // Apply acceleration or deceleration
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - forwardSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.1f) ? acceleration : deceleration;

        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        // Apply force forward
        rb.AddForce(transform.forward * movement, ForceMode.Acceleration);

        // Turning (only when moving a bit)
        if (Mathf.Abs(forwardSpeed) > 0.5f)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}
