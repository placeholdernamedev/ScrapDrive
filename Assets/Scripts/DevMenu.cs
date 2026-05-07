using UnityEngine;
using UnityEngine.SceneManagement;

public class DevMenu : MonoBehaviour
{
    private static DevMenu instance;

    private bool show = false;
    private bool infiniteFuel = false;
    private FuelSystem cachedFuel;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            show = !show;

        if (infiniteFuel)
        {
            if (cachedFuel == null)
                cachedFuel = Object.FindFirstObjectByType<FuelSystem>();
            if (cachedFuel != null)
                cachedFuel.currentFuel = cachedFuel.maxFuel;
        }
    }

    void OnGUI()
    {
        if (!show) return;

        // Center the menu on the screen
        const float menuW = 280f;
        const float menuH = 215f;
        float x0 = (Screen.width - menuW) * 0.5f;
        float y0 = (Screen.height - menuH) * 0.5f;

        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.85f);
        GUI.Box(new Rect(x0, y0, menuW, menuH), "DEV MENU  (P to close)");
        GUI.backgroundColor = prevColor;

        if (GUI.Button(new Rect(x0 + 10, y0 + 35, 260, 30), "Next Level"))
            GoToNextLevel();

        if (GUI.Button(new Rect(x0 + 10, y0 + 70, 260, 30), "Teleport to End"))
            TeleportToEnd();

        bool newInf = GUI.Toggle(new Rect(x0 + 10, y0 + 110, 260, 25), infiniteFuel, "  Infinite Fuel");
        if (newInf != infiniteFuel)
        {
            infiniteFuel = newInf;
            if (!infiniteFuel) cachedFuel = null;
        }

        GUI.Label(new Rect(x0 + 10, y0 + 140, 260, 60),
            "Status:\n  Infinite fuel: " + (infiniteFuel ? "ON" : "OFF"));
    }

    void GoToNextLevel()
    {
        var trigger = Object.FindFirstObjectByType<LevelEndTrigger>();
        if (trigger == null || string.IsNullOrEmpty(trigger.nextSceneName))
        {
            Debug.LogWarning("[DevMenu] No LevelEndTrigger or nextSceneName set in this scene.");
            return;
        }
        Debug.Log("[DevMenu] Loading " + trigger.nextSceneName);
        SceneManager.LoadScene(trigger.nextSceneName);
    }

    void TeleportToEnd()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Vector3 playerPos, carPos;

        if (sceneName == "Level1")
        {
            playerPos = new Vector3(285f, 1f, 1465f);
            carPos = new Vector3(290f, 1f, 1465f);
        }
        else if (sceneName == "Level2")
        {
            playerPos = new Vector3(-5f, 1f, 945f);
            carPos = new Vector3(0f, 1f, 945f);
        }
        else if (sceneName == "Level3")
        {
            playerPos = new Vector3(-5f, 1f, 1790f);
            carPos = new Vector3(0f, 1f, 1790f);
        }
        else
        {
            Debug.LogWarning("[DevMenu] No teleport defined for scene: " + sceneName);
            return;
        }

        var player = GameObject.Find("Player");
        var car = GameObject.Find("Car With Turret");

        if (car != null)
        {
            var carRb = car.GetComponent<Rigidbody>();
            if (carRb != null)
            {
                carRb.linearVelocity = Vector3.zero;
                carRb.angularVelocity = Vector3.zero;
            }
            car.transform.position = carPos;
            car.transform.rotation = Quaternion.identity;
        }

        if (player != null)
        {
            var charCtl = player.GetComponent<CharacterController>();
            if (charCtl != null) charCtl.enabled = false;
            player.transform.position = playerPos;
            player.transform.rotation = Quaternion.identity;
            if (charCtl != null) charCtl.enabled = true;
        }

        Debug.Log("[DevMenu] Teleported to end of " + sceneName);
    }
}
