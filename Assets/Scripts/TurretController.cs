using UnityEngine;
using UnityEngine.UI;

public class TurretController : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;
    public Transform barrel;
    public Transform muzzle;

    [Header("Layers")]
    public LayerMask hitMask; // decides what bullets can hit or pass through

    [Header("UI")]
    public RectTransform crosshairUI;

    [Header("Shooting")]
    public float fireRate = 0.2f;
    public float damage = 10f;
    public float range = 1000f;

    [Header("Audio")] // for shooting sound
    public AudioSource gunAudioSource;
    public AudioClip gunSound;

    [Header("Rotation")] // limits some movement
    public float rotationSpeed = 10f;
    public float minPitch = -10f;
    public float maxPitch = 45f;

    private float nextFireTime;
    private Camera cam; // will be main camera

    void Start()
    {
        cam = Camera.main;

    }

    void Update()
    {
        Vector3 targetPoint = GetTargetPoint();

        AimTurret(targetPoint);
        UpdateCrosshair(targetPoint);

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) // hold left click
        {
            Shoot(targetPoint);

            nextFireTime = Time.time + fireRate; // controls fire rate
        }
    }

    Vector3 GetTargetPoint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitMask))
        {
            return hit.point;
        }

        return ray.GetPoint(range);
    }

    void AimTurret(Vector3 targetPoint)
    {
        // --- Pivot (Y only) ---
        Vector3 direction = targetPoint - pivot.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // --- Barrel (X only) ---
        Vector3 localTarget = pivot.InverseTransformPoint(targetPoint);

        float angle = Mathf.Atan2(localTarget.y, localTarget.z) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minPitch, maxPitch);

        barrel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void Shoot(Vector3 targetPoint)
{
    Vector3 direction = (targetPoint - muzzle.position).normalized;

    Ray ray = new Ray(muzzle.position, direction);
    RaycastHit hit;

    Vector3 endPoint;

    if (Physics.Raycast(ray, out hit, range, hitMask))
    {
        endPoint = hit.point;

        var damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
    else
    {
        endPoint = muzzle.position + direction * range;
    }

    gunAudioSource.PlayOneShot(gunSound); // plays gunshot audio when shot

    StartCoroutine(DrawLine(muzzle.position, endPoint)); // draws a line for a set period of time
}

System.Collections.IEnumerator DrawLine(Vector3 start, Vector3 end)
{
    GameObject lineObj = new GameObject("ShotLine");
    LineRenderer lr = lineObj.AddComponent<LineRenderer>();

    lr.startWidth = 0.05f;
    lr.endWidth = 0.05f;
    lr.positionCount = 2;
    lr.SetPosition(0, start);
    lr.SetPosition(1, end);

    // creates a material for the line
    lr.material = new Material(Shader.Find("Sprites/Default"));

    // set color to yellow
    lr.startColor = Color.yellow;
    lr.endColor = Color.yellow;

    yield return new WaitForSeconds(0.05f);

    Destroy(lineObj);
}

    void UpdateCrosshair(Vector3 targetPoint)
    {
        if (crosshairUI == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(targetPoint);
        crosshairUI.position = screenPos;
    }
}
