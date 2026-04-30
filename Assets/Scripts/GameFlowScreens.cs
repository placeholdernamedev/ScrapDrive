using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowScreens : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private bool hasStarted;
    private bool isGameOver;
    private bool isPlaytestScene;
    private float deathTimeSurvived;
    private int robotsKilledAtDeath;
    private StopwatchTimer stopwatchTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGameFlowScreens()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        bool supportedScene = activeScene.name == "Zak_Dev" || activeScene.name == "PlaytestLevel";
        if (!supportedScene)
        {
            return;
        }

        GameObject root = new GameObject("GameFlowScreens");
        root.AddComponent<GameFlowScreens>();
    }

    private void Start()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        isPlaytestScene = activeScene.name == "PlaytestLevel";
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        stopwatchTimer = FindFirstObjectByType<StopwatchTimer>();

        if (isPlaytestScene)
        {
            EnemyHealth.ResetKillCount();
            hasStarted = true;
            ResumeGame();
            return;
        }

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
            deathTimeSurvived = stopwatchTimer != null ? stopwatchTimer.elapsedTime : Time.timeSinceLevelLoad;
            robotsKilledAtDeath = EnemyHealth.TotalRobotsKilled;

            if (stopwatchTimer != null)
            {
                stopwatchTimer.PauseTimer();
            }

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

        if (DrawRightSideButton("Begin", 22f, 220f, 48f))
        {
            hasStarted = true;
            ResumeGame();
        }
    }

    private void DrawGameOverScreen()
    {
        DrawOverlay();
        DrawCenteredLabel("GAME OVER", -35f, 44);
        DrawCenteredLabel($"Time Survived: {FormatTime(deathTimeSurvived)}", 10f, 28);
        DrawCenteredLabel($"Robots Killed: {robotsKilledAtDeath}", 48f, 28);

        if (DrawRightSideButton("Restart", 98f, 220f, 48f))
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

    private static bool DrawRightSideButton(string text, float yOffset, float width, float height)
    {
        float rightPadding = 48f;
        Rect rect = new Rect(Screen.width - width - rightPadding, Screen.height * 0.5f + yOffset, width, height);
        return GUI.Button(rect, text);
    }

    private static string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
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
