using UnityEngine;

public static class EnemyDropSpawner
{
    private const float HealthDropChance = 0.20f;
    private const float TurretDropChance = 0.20f;
    private const float PickupHeightOffset = 0.9f;
    private const float PickupRadius = 0.45f;

    public static void TrySpawnDrops(Vector3 worldPosition)
    {
        Vector3 spawnPos = worldPosition + Vector3.up * PickupHeightOffset;

        if (Random.value <= HealthDropChance)
        {
            SpawnPickup(spawnPos + new Vector3(-0.4f, 0f, 0f), UpgradePickup.UpgradeType.CarHealthBoost, CreateMedkitVisual);
        }

        if (Random.value <= TurretDropChance)
        {
            SpawnPickup(spawnPos + new Vector3(0.4f, 0f, 0f), UpgradePickup.UpgradeType.TurretBoost, CreateTurretVisual);
        }
    }

    private static void SpawnPickup(Vector3 position, UpgradePickup.UpgradeType type, System.Action<Transform> createVisual)
    {
        GameObject pickup = new GameObject(type + "Pickup");
        pickup.transform.position = position;

        SphereCollider trigger = pickup.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = PickupRadius;

        Rigidbody rb = pickup.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        UpgradePickup upgrade = pickup.AddComponent<UpgradePickup>();
        upgrade.upgradeType = type;

        GameObject visualRoot = new GameObject("Visual");
        visualRoot.transform.SetParent(pickup.transform, false);
        createVisual(visualRoot.transform);
        visualRoot.AddComponent<FloatingPickupAnimation>();
    }

    private static void CreateMedkitVisual(Transform root)
    {
        GameObject kitBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.Destroy(kitBody.GetComponent<Collider>());
        kitBody.transform.SetParent(root, false);
        kitBody.transform.localScale = new Vector3(0.55f, 0.3f, 0.4f);

        Renderer bodyRenderer = kitBody.GetComponent<Renderer>();
        bodyRenderer.material = CreateUnlitMaterial(new Color(0.82f, 0.08f, 0.08f));

        GameObject crossVert = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.Destroy(crossVert.GetComponent<Collider>());
        crossVert.transform.SetParent(root, false);
        crossVert.transform.localPosition = new Vector3(0f, 0.16f, 0f);
        crossVert.transform.localScale = new Vector3(0.08f, 0.06f, 0.28f);
        crossVert.GetComponent<Renderer>().material = CreateUnlitMaterial(Color.white);

        GameObject crossHorz = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.Destroy(crossHorz.GetComponent<Collider>());
        crossHorz.transform.SetParent(root, false);
        crossHorz.transform.localPosition = new Vector3(0f, 0.16f, 0f);
        crossHorz.transform.localScale = new Vector3(0.28f, 0.06f, 0.08f);
        crossHorz.GetComponent<Renderer>().material = CreateUnlitMaterial(Color.white);
    }

    private static void CreateTurretVisual(Transform root)
    {
        GameObject basePart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.Destroy(basePart.GetComponent<Collider>());
        basePart.transform.SetParent(root, false);
        basePart.transform.localScale = new Vector3(0.22f, 0.08f, 0.22f);
        basePart.GetComponent<Renderer>().material = CreateUnlitMaterial(new Color(0.2f, 0.24f, 0.3f));

        GameObject topPart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.Destroy(topPart.GetComponent<Collider>());
        topPart.transform.SetParent(root, false);
        topPart.transform.localPosition = new Vector3(0f, 0.17f, 0f);
        topPart.transform.localScale = new Vector3(0.16f, 0.06f, 0.16f);
        topPart.GetComponent<Renderer>().material = CreateUnlitMaterial(new Color(0.35f, 0.4f, 0.5f));

        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.Destroy(barrel.GetComponent<Collider>());
        barrel.transform.SetParent(root, false);
        barrel.transform.localPosition = new Vector3(0f, 0.18f, 0.27f);
        barrel.transform.localScale = new Vector3(0.08f, 0.08f, 0.35f);
        barrel.GetComponent<Renderer>().material = CreateUnlitMaterial(new Color(0.55f, 0.65f, 0.78f));
    }

    private static Material CreateUnlitMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        Material mat = new Material(shader);
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        if (mat.HasProperty("_Color"))
        {
            mat.SetColor("_Color", color);
        }

        return mat;
    }
}
