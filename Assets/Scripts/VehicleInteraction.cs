using UnityEngine;

public class VehicleInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public CarMovement carController;          // your car control script object
    public CharacterController playerController;    // your player movement script
    public CameraFollow cameraFollow;

    [Header("Camera Targets")]
    public Transform playerCameraTarget;
    public Transform carCameraTarget;

    [Header("Exit Settings")]
    public Transform exitPoint;

    [Header("UI")]
    public GameObject enterPromptUI;
    public TMPro.TMP_Text promptText;

    private bool canEnter = false;
    private bool inVehicle = false;
    public bool InVehicle => inVehicle; // note that this is capital here, there's a difference, this is a read only property for other scripts to access
    public bool CanEnter => canEnter; // same deal as above here.

    void Start()
    {
        // Make sure UI is hidden at start
        if (enterPromptUI != null)
            enterPromptUI.SetActive(false);
    }

    void Update()
    {
        UpdatePromptUI();

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

        // Disable player controls
        playerController.enabled = false;

        // Hide player (optional)
        player.gameObject.SetActive(false);

        // Enable car controls
        carController.enabled = true;

        // Switch camera
        cameraFollow.currentTarget = carCameraTarget;

    }

   void ExitVehicle()
{
    inVehicle = false;

    // Disable car controls
    carController.enabled = false;

    // Disable CharacterController BEFORE moving
    CharacterController cc = player.GetComponent<CharacterController>();
    if (cc) cc.enabled = false;

    // Move player to exit point
    player.position = exitPoint.position;
    player.rotation = exitPoint.rotation;

    // Re-enable CharacterController
    if (cc) cc.enabled = true;

    // Re-enable player controls
    playerController.enabled = true;

    // Show player
    player.gameObject.SetActive(true);

    // Switch camera back
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