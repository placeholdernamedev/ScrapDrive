using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverRuntime : MonoBehaviour
{
    public string restartSceneName = "Level1";
    public float fadeInDuration = 1.5f;

    private static GameOverRuntime instance;
    private static bool isShowing;

    private float deathTime;
    private int robotsKilled;

    private GameObject canvasRoot;
    private Image bgImage;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI killsText;
    private Image buttonImage;
    private TextMeshProUGUI buttonLabel;

    public static void Show()
    {
        if (isShowing)
        {
            return;
        }

        if (instance == null)
        {
            var go = new GameObject("GameOverRuntime");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<GameOverRuntime>();
        }

        instance.CaptureStats();
        instance.BuildUI();
    }

    private void CaptureStats()
    {
        StopwatchTimer timer = FindFirstObjectByType<StopwatchTimer>();
        if (timer != null)
        {
            deathTime = timer.elapsedTime;
            timer.PauseTimer();
        }
        else
        {
            deathTime = Time.timeSinceLevelLoad;
        }

        robotsKilled = EnemyHealth.TotalRobotsKilled;
    }

    private void BuildUI()
    {
        if (isShowing)
        {
            return;
        }

        isShowing = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        var canvasGO = new GameObject("GameOverCanvas");
        canvasRoot = canvasGO;
        DontDestroyOnLoad(canvasGO);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9998;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0f);
        var bgRT = bgImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        var titleGO = new GameObject("GameOverText");
        titleGO.transform.SetParent(canvasGO.transform, false);
        titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "GAME OVER";
        titleText.color = new Color(1f, 1f, 1f, 0f);
        titleText.fontSize = 140f;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        var trt = titleText.rectTransform;
        trt.anchorMin = new Vector2(0f, 0.62f);
        trt.anchorMax = new Vector2(1f, 0.62f);
        trt.pivot = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(0, 200);
        trt.anchoredPosition = Vector2.zero;

        var timeGO = new GameObject("TimeText");
        timeGO.transform.SetParent(canvasGO.transform, false);
        timeText = timeGO.AddComponent<TextMeshProUGUI>();
        timeText.text = $"Time Survived: {FormatTime(deathTime)}";
        timeText.color = new Color(1f, 1f, 1f, 0f);
        timeText.fontSize = 44f;
        timeText.alignment = TextAlignmentOptions.Center;
        var timeRT = timeText.rectTransform;
        timeRT.anchorMin = new Vector2(0f, 0.5f);
        timeRT.anchorMax = new Vector2(1f, 0.5f);
        timeRT.pivot = new Vector2(0.5f, 0.5f);
        timeRT.sizeDelta = new Vector2(0, 80);
        timeRT.anchoredPosition = new Vector2(0f, -10f);

        var killsGO = new GameObject("KillsText");
        killsGO.transform.SetParent(canvasGO.transform, false);
        killsText = killsGO.AddComponent<TextMeshProUGUI>();
        killsText.text = $"Robots Killed: {robotsKilled}";
        killsText.color = new Color(1f, 1f, 1f, 0f);
        killsText.fontSize = 40f;
        killsText.alignment = TextAlignmentOptions.Center;
        var killsRT = killsText.rectTransform;
        killsRT.anchorMin = new Vector2(0f, 0.46f);
        killsRT.anchorMax = new Vector2(1f, 0.46f);
        killsRT.pivot = new Vector2(0.5f, 0.5f);
        killsRT.sizeDelta = new Vector2(0, 70);
        killsRT.anchoredPosition = new Vector2(0f, -8f);

        var btnGO = new GameObject("RestartButton");
        btnGO.transform.SetParent(canvasGO.transform, false);
        buttonImage = btnGO.AddComponent<Image>();
        buttonImage.color = new Color(0.15f, 0.15f, 0.2f, 0f);
        var btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = buttonImage;
        var btnRT = buttonImage.rectTransform;
        btnRT.anchorMin = new Vector2(0.5f, 0.34f);
        btnRT.anchorMax = new Vector2(0.5f, 0.34f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.sizeDelta = new Vector2(360, 90);
        btnRT.anchoredPosition = Vector2.zero;

        var btnTextGO = new GameObject("Label");
        btnTextGO.transform.SetParent(btnGO.transform, false);
        buttonLabel = btnTextGO.AddComponent<TextMeshProUGUI>();
        buttonLabel.text = "Restart";
        buttonLabel.color = new Color(1f, 1f, 1f, 0f);
        buttonLabel.fontSize = 48f;
        buttonLabel.alignment = TextAlignmentOptions.Center;
        buttonLabel.fontStyle = FontStyles.Bold;
        var btnTextRT = buttonLabel.rectTransform;
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        btn.onClick.AddListener(OnRestartClicked);

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeInDuration);

            if (bgImage != null)
            {
                bgImage.color = new Color(0f, 0f, 0f, k * 0.78f);
            }

            if (titleText != null)
            {
                titleText.color = new Color(1f, 1f, 1f, k);
            }

            if (timeText != null)
            {
                timeText.color = new Color(1f, 1f, 1f, k);
            }

            if (killsText != null)
            {
                killsText.color = new Color(1f, 1f, 1f, k);
            }

            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.15f, 0.15f, 0.2f, k * 0.95f);
            }

            if (buttonLabel != null)
            {
                buttonLabel.color = new Color(1f, 1f, 1f, k);
            }

            yield return null;
        }
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        TurretBoostRuntime.ResetBoost();
        EnemyHealth.ResetKillCount();
        isShowing = false;

        if (canvasRoot != null)
        {
            Destroy(canvasRoot);
            canvasRoot = null;
        }

        if (instance == this)
        {
            instance = null;
            Destroy(gameObject);
        }

        if (!string.IsNullOrEmpty(restartSceneName))
        {
            SceneManager.LoadScene(restartSceneName);
        }
    }

    private static string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}

