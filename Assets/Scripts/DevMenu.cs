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

        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.85f);
        GUI.Box(new Rect(10, 10, 280, 175), "DEV MENU  (P to close)");
        GUI.backgroundColor = prevColor;

        if (GUI.Button(new Rect(20, 45, 260, 30), "Next Level"))
            GoToNextLevel();

        bool newInf = GUI.Toggle(new Rect(20, 85, 260, 25), infiniteFuel, "  Infinite Fuel");
        if (newInf != infiniteFuel)
        {
            infiniteFuel = newInf;
            if (!infiniteFuel) cachedFuel = null;
        }

        GUI.Label(new Rect(20, 115, 260, 60),
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
}
