using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    public float maxFuel = 100f;
    public float currentFuel;
    public float fuelConsumptionRate = 5f;
    private bool isFueled = true;

    public VehicleInteraction vehicleInteraction;
    public CarMovement carMovement;

    [Header("Low Fuel Warning")]
    public AudioSource lowFuelSource;
    public float lowFuelThreshold = 15f;

private bool warningPlaying = false;

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

        if(currentFuel == 0 && isFueled)
        {
            isFueled = false;
            carMovement.setFueled(false);
        }

        if (currentFuel <= lowFuelThreshold)
        {
            if (!warningPlaying)
            {
                lowFuelSource.Play();
                warningPlaying = true;
            }
        }
        else
        {
            if (warningPlaying)
            {
                lowFuelSource.Stop();
                warningPlaying = false;
            }
        }
    }

    public void Refuel(float amount)
    {
        if (amount <= 0f) return;

        float missingFuel = Mathf.Max(0f, maxFuel - currentFuel);
        float refillAmount = Mathf.Min(amount, missingFuel);
        currentFuel += refillAmount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel); // makes sure fuel level does not go above max fuel.
        isFueled = true;
        if (carMovement != null)
        {
            carMovement.setFueled(true);
        }
    }
}