using UnityEngine;

public class CarAudio : MonoBehaviour
{
    public AudioSource idleSource;
    public AudioSource driveSource;

    public Rigidbody rb;

    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;

    public float maxSpeed = 50f;

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        float t = Mathf.Clamp01(speed / maxSpeed);

        // Pitch changes with speed
        idleSource.pitch = Mathf.Lerp(minPitch, 1.2f, t);
        driveSource.pitch = Mathf.Lerp(0.8f, maxPitch, t);

        // Volume blending
        idleSource.volume = Mathf.Lerp(1f, 0.2f, t);
        driveSource.volume = Mathf.Lerp(0.1f, 1f, t);
    }
}