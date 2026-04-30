using UnityEngine;

public class GasStation : MonoBehaviour
{
    public float refuelRate = 25f;
    private VehicleInteraction currentVehicle;
    private FuelSystem currentFuel;
    private bool playerInside = false;
    
    [Header("UI")]
    public GameObject enterPromptUI;
    public TMPro.TMP_Text promptText;

    void Start()
    {
        // Make sure UI is hidden at start
        if (enterPromptUI != null)
            enterPromptUI.SetActive(false);
    }

    // PLAYER ENTERS/EXITS STATION
    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }

        if (other.CompareTag("Car"))
        {
            currentVehicle = other.GetComponent<VehicleInteraction>();
            currentFuel = other.GetComponent<FuelSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }

        if (other.CompareTag("Car"))
        {
            currentVehicle = null;
            currentFuel = null;
        }
    }

    // HANDLE REFUELING
    private void Update()
    {
        if (!playerInside || currentVehicle == null || currentFuel == null)
        {
            if (enterPromptUI != null)
                enterPromptUI.SetActive(false);

            return;
        }

        // Only show UI if actually able to refuel
        bool canRefuel = !currentVehicle.InVehicle &&
                        currentFuel.currentFuel < currentFuel.maxFuel;

        if (canRefuel)
        {
            enterPromptUI.SetActive(true);
            promptText.text = "Hold X to Refuel";

            if (Input.GetKey(KeyCode.X))
            {
                currentFuel.Refuel(refuelRate * Time.deltaTime);
            }
        }
        else
        {
            enterPromptUI.SetActive(false);
        }
    }
}