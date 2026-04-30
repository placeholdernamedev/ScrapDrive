using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsHintRuntime : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateControlsHint()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        bool supportedScene = activeScene.name == "Zak_Dev" || activeScene.name == "PlaytestLevel";
        if (!supportedScene)
        {
            return;
        }

        GameObject root = new GameObject("ControlsHintRuntime");
        root.AddComponent<ControlsHintRuntime>();
    }

    private void OnGUI()
    {
        const float width = 230f;
        const float height = 122f;
        const float rightPadding = 16f;
        const float bottomPadding = 16f;

        Rect panelRect = new Rect(Screen.width - width - rightPadding, Screen.height - height - bottomPadding, width, height);

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 13;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;

        GUIStyle lineStyle = new GUIStyle(GUI.skin.label);
        lineStyle.fontSize = 12;
        lineStyle.normal.textColor = Color.white;

        float textLeft = panelRect.x + 10f;
        float y = panelRect.y + 8f;

        GUI.Label(new Rect(textLeft, y, width - 20f, 20f), "Controls", headerStyle);
        y += 22f;

        GUI.Label(new Rect(textLeft, y, width - 20f, 18f), "WASD  - Move / Drive", lineStyle);
        y += 18f;
        GUI.Label(new Rect(textLeft, y, width - 20f, 18f), "Z     - Enter / Exit Car", lineStyle);
        y += 18f;
        GUI.Label(new Rect(textLeft, y, width - 20f, 18f), "X     - Refuel (at station)", lineStyle);
        y += 18f;
        GUI.Label(new Rect(textLeft, y, width - 20f, 18f), "LMB   - Shoot", lineStyle);
    }
}
