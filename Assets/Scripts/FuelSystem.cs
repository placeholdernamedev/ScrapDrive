using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    public float maxFuel = 100f;
    public float currentFuel;
    public float fuelConsumptionRate = 5f;

    public VehicleInteraction vehicleInteraction;

    void Start()
    {
        currentFuel = maxFuel;
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && (vehicleInteraction.InVehicle == true))
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime; // per second consumption
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel); // makes sure minimum is zero
        }
    }

    public void Refuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel); // makes sure fuel level does not go above max fuel.
    }
}