using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowScreens : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private bool hasStarted;
    private bool isGameOver;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGameFlowScreens()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "Zak_Dev")
        {
            return;
        }

        GameObject root = new GameObject("GameFlowScreens");
        root.AddComponent<GameFlowScreens>();
    }

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        PauseGame();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            return;
        }

        if (hasStarted && !playerHealth.IsAlive)
        {
            isGameOver = true;
            PauseGame();
        }
    }

    private void OnGUI()
    {
        if (!hasStarted)
        {
            DrawStartScreen();
            return;
        }

        if (isGameOver)
        {
            DrawGameOverScreen();
        }
    }

    private void DrawStartScreen()
    {
        DrawOverlay();
        DrawCenteredLabel("SCRAPDRIVE", -70f, 40);
        DrawCenteredLabel("Press Begin to start", -24f, 26);

        if (DrawCenteredButton("Begin", 22f, 220f, 48f))
        {
            hasStarted = true;
            ResumeGame();
        }
    }

    private void DrawGameOverScreen()
    {
        DrawOverlay();
        DrawCenteredLabel("GAME OVER", -35f, 44);

        if (DrawCenteredButton("Restart", 30f, 220f, 48f))
        {
            ResumeGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private static void DrawOverlay()
    {
        Color previous = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.78f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previous;
    }

    private static void DrawCenteredLabel(string text, float yOffset, int fontSize)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = fontSize;
        style.normal.textColor = Color.white;

        Rect rect = new Rect(0f, Screen.height * 0.5f + yOffset, Screen.width, 50f);
        GUI.Label(rect, text, style);
    }

    private static bool DrawCenteredButton(string text, float yOffset, float width, float height)
    {
        Rect rect = new Rect((Screen.width - width) * 0.5f, Screen.height * 0.5f + yOffset, width, height);
        return GUI.Button(rect, text);
    }

    private static void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private static void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
