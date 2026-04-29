using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int targetPickupCount = 8;
    public Vector2 xRange = new Vector2(-80f, 80f);
    public Vector2 zRange = new Vector2(-80f, 80f);
    public float raycastHeight = 100f;
    public float minGroundY = -20f;
    public float maxGroundSlope = 35f;
    public float respawnCheckInterval = 2f;

    private float nextRespawnCheck;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateSpawner()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "Zak_Dev")
        {
            return;
        }

        GameObject root = new GameObject("UpgradeSpawner");
        root.AddComponent<UpgradeSpawner>();
    }

    private void Start()
    {
        while (transform.childCount < targetPickupCount)
        {
            TrySpawnOnePickup();
        }

        nextRespawnCheck = Time.time + respawnCheckInterval;
    }

    private void Update()
    {
        if (Time.time < nextRespawnCheck)
        {
            return;
        }

        nextRespawnCheck = Time.time + respawnCheckInterval;

        while (transform.childCount < targetPickupCount)
        {
            if (!TrySpawnOnePickup())
            {
                break;
            }
        }
    }

    private bool TrySpawnOnePickup()
    {
        for (int i = 0; i < 16; i++)
        {
            Vector3 point = GetRandomPointAboveGround();
            if (!Physics.Raycast(point, Vector3.down, out RaycastHit hit, raycastHeight * 2f))
            {
                continue;
            }

            if (hit.point.y < minGroundY)
            {
                continue;
            }

            if (Vector3.Angle(hit.normal, Vector3.up) > maxGroundSlope)
            {
                continue;
            }

            if (Physics.CheckSphere(hit.point + Vector3.up * 0.75f, 0.5f))
            {
                continue;
            }

            CreatePickup(hit.point + Vector3.up * 0.5f);
            return true;
        }

        return false;
    }

    private Vector3 GetRandomPointAboveGround()
    {
        float x = Random.Range(xRange.x, xRange.y);
        float z = Random.Range(zRange.x, zRange.y);
        return new Vector3(x, raycastHeight, z);
    }

    private void CreatePickup(Vector3 worldPosition)
    {
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        pickup.name = "UpgradePickup";
        pickup.transform.SetParent(transform);
        pickup.transform.position = worldPosition;
        pickup.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        Collider collider = pickup.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        Rigidbody rb = pickup.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        UpgradePickup upgrade = pickup.AddComponent<UpgradePickup>();
        upgrade.upgradeType = (UpgradePickup.UpgradeType)Random.Range(0, 3);

        Renderer renderer = pickup.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = GetColorForType(upgrade.upgradeType);
        }
    }

    private static Color GetColorForType(UpgradePickup.UpgradeType type)
    {
        switch (type)
        {
            case UpgradePickup.UpgradeType.CarHealthBoost:
                return new Color(0.2f, 0.9f, 0.2f);
            case UpgradePickup.UpgradeType.GasBoost:
                return new Color(1f, 0.85f, 0.2f);
            case UpgradePickup.UpgradeType.PlayerHealthBoost:
                return new Color(0.2f, 0.8f, 1f);
            default:
                return Color.white;
        }
    }
}
