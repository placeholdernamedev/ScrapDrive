using UnityEngine;

public class VehicleInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public CarMovement carController;          // your car control script object
    public CharacterController playerController;    // your player movement script
    public CameraFollow cameraFollow;
    public TurretController turretController;

    [Header("Camera Targets")]
    public Transform playerCameraTarget;
    public Transform carCameraTarget;

    [Header("Exit Settings")]
    public Transform exitPoint;

    [Header("UI")]
    public GameObject enterPromptUI;
    public TMPro.TMP_Text promptText;
    public GameObject crosshair;

    private bool canEnter = false;
    private bool inVehicle = false;
    public bool InVehicle => inVehicle;
    public bool CanEnter => canEnter;

    void Start()
    {
        if (enterPromptUI != null)
            enterPromptUI.SetActive(false);
    }

    void Update()
    {
        UpdatePromptUI();

        // keep the player riding on the car so enemies that target the player chase the car
        if (inVehicle && player != null && carController != null)
        {
            player.position = carController.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (CanEnter && !InVehicle)
            {
                EnterVehicle();
            }
            else if (InVehicle)
            {
                ExitVehicle();
            }
        }
    }

    void UpdatePromptUI()
    {
        if (InVehicle)
        {
            enterPromptUI.SetActive(true);
            promptText.text = "Press Z to Exit the Vehicle";
        }
        else if (CanEnter)
        {
            enterPromptUI.SetActive(true);
            promptText.text = "Press Z to Enter the Vehicle";
        }
        else
        {
            enterPromptUI.SetActive(false);
        }
    }

    void EnterVehicle()
    {
        inVehicle = true;

        playerController.enabled = false;

        // hide player visuals (used to SetActive(false) but that froze its Transform position)
        foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        carController.enabled = true;
        turretController.enabled = true;
        crosshair.SetActive(true);
        cameraFollow.currentTarget = carCameraTarget;
    }

    void ExitVehicle()
    {
        inVehicle = false;

        carController.enabled = false;
        turretController.enabled = false;
        crosshair.SetActive(false);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;

        if (cc) cc.enabled = true;

        playerController.enabled = true;

        // show player visuals
        foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }

        cameraFollow.currentTarget = playerCameraTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }
}
