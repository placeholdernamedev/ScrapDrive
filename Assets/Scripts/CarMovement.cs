using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    // Fueled numbers
    public float Facceleration = 200f;
    public float FmaxSpeed = 500f;
    public float FturnSpeed = 120f;
    public float FdragOnGround = 4f;

    // no fuel numbers
    public float NFacceleration = 35f;
    public float NFmaxSpeed = 85f;
    public float NFturnSpeed = 50f;
    public float NFdragOnGround = 1f;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float dragOnGround;
    private Rigidbody rb;
    [SerializeField] private float downforce = 50f;

    // reference for fuel system for checking for fuel
    public FuelSystem fuelSystem;

    public void setFueled(bool IsFueled)
    {
        if (IsFueled)
        {
            acceleration = Facceleration;
            maxSpeed = FmaxSpeed;
            turnSpeed = FturnSpeed;
            dragOnGround = FdragOnGround;
        }
        else
        {
            acceleration = NFacceleration;
            maxSpeed = NFmaxSpeed;
            turnSpeed = NFturnSpeed;
            dragOnGround = NFdragOnGround;
        }
    }

    void Start()
    {
        // car starts fueled, so stats start fueled as well.
        setFueled(true);

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
        float speed = rb.linearVelocity.magnitude;
        rb.AddForce(-transform.up * downforce * speed, ForceMode.Force);
    }
}
