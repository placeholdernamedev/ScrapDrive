using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    public float acceleration = 30f;
    public float maxSpeed = 20f;
    public float turnSpeed = 120f;
    public float dragOnGround = 4f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // helps stability
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");   // W/S
        float turnInput = Input.GetAxis("Horizontal"); // A/D

        // Forward movement
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        }

        // Turning (only when moving a bit)
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Extra drag so it doesn't feel floaty
        if (moveInput == 0)
        {
            rb.linearDamping = 8f; // stronger braking
        }
        else
        {
        rb.linearDamping = dragOnGround;
        }
    }
}
