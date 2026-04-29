using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleHudRuntime : MonoBehaviour
{
    private VehicleInteraction vehicleInteraction;
    private CarHealth carHealth;
    private FuelSystem fuelSystem;
    private PlayerHealth playerHealth;
    private float nextSetupAttemptTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHud()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "Zak_Dev")
        {
            return;
        }

        var hudRoot = new GameObject("VehicleHudRuntime");
        hudRoot.AddComponent<VehicleHudRuntime>();
    }

    private void Update()
    {
        if (vehicleInteraction == null || carHealth == null || fuelSystem == null || playerHealth == null)
        {
            if (Time.time >= nextSetupAttemptTime)
            {
                TrySetup();
                nextSetupAttemptTime = Time.time + 1f;
            }
        }
    }

    private void TrySetup()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        vehicleInteraction = FindFirstObjectByType<VehicleInteraction>();
        if (vehicleInteraction == null)
        {
            return;
        }

        var carObject = vehicleInteraction.gameObject;

        carHealth = carObject.GetComponent<CarHealth>();

        fuelSystem = carObject.GetComponent<FuelSystem>();
        if (fuelSystem == null)
        {
            fuelSystem = carObject.AddComponent<FuelSystem>();
        }

        if (fuelSystem != null)
        {
            if (fuelSystem.vehicleInteraction == null)
            {
                fuelSystem.vehicleInteraction = vehicleInteraction;
            }

            if (fuelSystem.carMovement == null)
            {
                fuelSystem.carMovement = carObject.GetComponent<CarMovement>();
            }
        }
    }

    private void OnGUI()
    {
        const float left = 20f;
        const float top = 20f;
        const float width = 320f;
        const float rowHeight = 22f;
        const float gap = 8f;

        DrawBar("Car Health", carHealth != null ? carHealth.currentHealth : 0f, carHealth != null ? carHealth.maxHealth : 1f,
            new Rect(left, top, width, rowHeight), new Color(0.2f, 0.85f, 0.2f));
        DrawBar("Fuel", fuelSystem != null ? fuelSystem.currentFuel : 0f, fuelSystem != null ? fuelSystem.maxFuel : 1f,
            new Rect(left, top + (rowHeight + gap), width, rowHeight), new Color(1f, 0.8f, 0.2f));
        DrawBar("Player Health", playerHealth != null ? playerHealth.currentHealth : 0f, playerHealth != null ? playerHealth.maxHealth : 1f,
            new Rect(left, top + (rowHeight + gap) * 2f, width, rowHeight), new Color(0.2f, 0.6f, 1f));
    }

    private static void DrawBar(string label, float value, float maxValue, Rect rect, Color fillColor)
    {
        float safeMax = Mathf.Max(1f, maxValue);
        float normalized = Mathf.Clamp01(value / safeMax);

        Color previous = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        GUI.color = fillColor;
        GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, (rect.width - 4f) * normalized, rect.height - 4f), Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 13;
        style.normal.textColor = Color.white;
        GUI.Label(rect, label + ": " + Mathf.RoundToInt(value) + " / " + Mathf.RoundToInt(safeMax), style);

        GUI.color = previous;
    }
}
