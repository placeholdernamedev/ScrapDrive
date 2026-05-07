using UnityEngine;

public class GasStation : MonoBehaviour
{
    public float refuelRate = 25f;
    private VehicleInteraction currentVehicle;
    private FuelSystem currentFuel;
    private Transform currentPlayer;
    private bool playerInside = false;

    private bool _uiActiveByThisStation = false;

    [Header("UI")]
    public GameObject enterPromptUI;
    public TMPro.TMP_Text promptText;

    [Header("Service Side")]
    [Tooltip("Local-space direction the pumps face. Refueling only works when the player stands on this side of the station.")]
    public Vector3 serviceDirection = Vector3.right;

    [Tooltip("How tolerant the wrong-side check is. -1 = anywhere (always correct), 0 = the +half, 1 = strictly aligned with the service direction.")]
    [Range(-1f, 1f)] public float serviceTolerance = 0.15f;

    [Header("Warning Flash")]
    [Tooltip("How long warnings stay on screen.")]
    public float warningDuration = 1.6f;
    public Color warningColor = new Color(1f, 0.35f, 0.35f, 1f);

    private float _warningEndTime = -1f;
    private string _warningText = string.Empty;

    void Start()
    {
        if (enterPromptUI != null) enterPromptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            currentPlayer = other.transform;
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
            currentPlayer = null;
        }
        if (other.CompareTag("Car"))
        {
            currentVehicle = null;
            currentFuel = null;
        }
    }

    private bool OnServiceSide()
    {
        if (currentPlayer == null) return false;

        Vector3 local = serviceDirection.sqrMagnitude > 0.0001f ? serviceDirection.normalized : Vector3.right;
        Vector3 dirWS = transform.TransformDirection(local);

        Vector3 toPlayer = currentPlayer.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) return true;

        return Vector3.Dot(toPlayer.normalized, dirWS) >= serviceTolerance;
    }

    private void Flash(string msg)
    {
        _warningText = msg;
        _warningEndTime = Time.time + warningDuration;
    }

    private bool WarningActive() => Time.time < _warningEndTime;

    private void Update()
    {
        if (currentFuel == null)
        {
            if (WarningActive())
            {
                ShowWarning();
                return;
            }
            HidePrompt();
            return;
        }

        bool fuelFull = currentFuel.currentFuel >= currentFuel.maxFuel;
        bool inVehicle = currentVehicle != null && currentVehicle.InVehicle;

        if (Input.GetKeyDown(KeyCode.X) && !fuelFull)
        {
            if (inVehicle)
            {
                Flash($"Press {KbdText.Kbd("Z")} to exit the car before refueling");
            }
            else if (playerInside && !OnServiceSide())
            {
                Flash("Wrong side - use the pumps on the other side");
            }
        }

        if (WarningActive())
        {
            ShowWarning();
            return;
        }

        bool canRefuel = !inVehicle && !fuelFull && playerInside && OnServiceSide();

        if (canRefuel)
        {
            ShowPrompt();
            int curR = Mathf.RoundToInt(currentFuel.currentFuel);
            int maxR = Mathf.RoundToInt(currentFuel.maxFuel);
            if (promptText != null)
            {
                promptText.color = Color.white;
                promptText.text = $"Fuel: {curR} / {maxR}\nHold {KbdText.Kbd("X")} to refuel";
            }
            if (Input.GetKey(KeyCode.X))
                currentFuel.Refuel(refuelRate * Time.deltaTime);
        }
        else
        {
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (enterPromptUI != null) enterPromptUI.SetActive(true);
        _uiActiveByThisStation = true;
    }

    private void HidePrompt()
    {
        if (_uiActiveByThisStation && enterPromptUI != null)
        {
            enterPromptUI.SetActive(false);
            _uiActiveByThisStation = false;
        }
    }

    private void ShowWarning()
    {
        if (enterPromptUI != null) enterPromptUI.SetActive(true);
        _uiActiveByThisStation = true;
        if (promptText != null)
        {
            promptText.color = warningColor;
            promptText.text = $"<b>{_warningText}</b>";
        }
    }
}
