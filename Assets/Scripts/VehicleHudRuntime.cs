using UnityEngine;

public class VehicleHudRuntime : MonoBehaviour
{
    private VehicleInteraction vehicleInteraction;
    private CarHealth carHealth;
    private FuelSystem fuelSystem;
    private float nextSetupAttemptTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHud()
    {
        var hudRoot = new GameObject("VehicleHudRuntime");
        hudRoot.AddComponent<VehicleHudRuntime>();
    }

    private void Update()
    {
        if (vehicleInteraction == null || carHealth == null || fuelSystem == null)
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
        if (vehicleInteraction == null || !vehicleInteraction.InVehicle)
        {
            return;
        }

        const int left = 16;
        const int top = 16;
        const int width = 320;
        const int height = 26;

        if (carHealth != null)
        {
            GUI.Label(new Rect(left, top, width, height),
                "Car Health: " + Mathf.RoundToInt(carHealth.currentHealth) + " / " + carHealth.maxHealth);
        }

        if (fuelSystem != null)
        {
            GUI.Label(new Rect(left, top + 26, width, height),
                "Fuel: " + Mathf.RoundToInt(fuelSystem.currentFuel) + " / " + fuelSystem.maxFuel);
        }
    }
}
