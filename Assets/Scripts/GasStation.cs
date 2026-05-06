using UnityEngine;

public class GasStation : MonoBehaviour
{
    public float refuelRate = 25f;
    private VehicleInteraction currentVehicle;
    private FuelSystem currentFuel;
    private bool playerInside = false;

    private bool _uiActiveByThisStation = false;

    [Header("UI")]
    public GameObject enterPromptUI;
    public TMPro.TMP_Text promptText;

    void Start()
    {
        if (enterPromptUI != null) enterPromptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = true;
        if (other.CompareTag("Car"))
        {
            currentVehicle = other.GetComponent<VehicleInteraction>();
            currentFuel = other.GetComponent<FuelSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = false;
        if (other.CompareTag("Car"))
        {
            currentVehicle = null;
            currentFuel = null;
        }
    }

    private void Update()
    {
        if (currentVehicle == null || currentFuel == null)
        {
            if (_uiActiveByThisStation && enterPromptUI != null)
            {
                enterPromptUI.SetActive(false);
                _uiActiveByThisStation = false;
            }
            return;
        }

        bool canRefuel = !currentVehicle.InVehicle &&
                         currentFuel.currentFuel < currentFuel.maxFuel;

        if (canRefuel)
        {
            if (enterPromptUI != null) enterPromptUI.SetActive(true);
            _uiActiveByThisStation = true;
            if (promptText != null)
            {
                int curR = Mathf.RoundToInt(currentFuel.currentFuel);
                int maxR = Mathf.RoundToInt(currentFuel.maxFuel);
                promptText.text = $"Fuel: {curR} / {maxR}\nHold X to Refuel";
            }
            if (Input.GetKey(KeyCode.X))
                currentFuel.Refuel(refuelRate * Time.deltaTime);
        }
        else
        {
            if (_uiActiveByThisStation && enterPromptUI != null)
            {
                enterPromptUI.SetActive(false);
                _uiActiveByThisStation = false;
            }
        }
    }
}
