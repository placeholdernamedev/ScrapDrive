using UnityEngine;

public class FloatingPickupAnimation : MonoBehaviour
{
    public float bobAmplitude = 0.14f;
    public float bobSpeed = 2.4f;
    public float spinSpeed = 70f;

    private Vector3 startLocalPos;
    private float phaseOffset;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
        phaseOffset = Random.Range(0f, 10f);
    }

    private void Update()
    {
        float t = Time.time + phaseOffset;
        float bob = Mathf.Sin(t * bobSpeed) * bobAmplitude;
        transform.localPosition = startLocalPos + Vector3.up * bob;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }
}
