using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Title overlay for the first final level: full-screen art plus a Start Game control
/// styled like <see cref="WinSequence"/> (same canvas setup, button colors, fade-in).
/// </summary>
public class TitleScreen : MonoBehaviour
{
    public const string TargetSceneName = "Level1";
    private const string BackgroundResourcePath = "TitleScreen/TitleScreenBackground";

    public float fadeInDuration = 1.5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (SceneManager.GetActiveScene().name != TargetSceneName)
            return;
        if (FindFirstObjectByType<TitleScreen>() != null)
            return;

        var go = new GameObject(nameof(TitleScreen));
        go.AddComponent<TitleScreen>();
    }

    private void Awake()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var canvasGO = new GameObject("TitleCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        var bg = new GameObject("BackgroundArt");
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImg = bg.AddComponent<Image>();
        var art = Resources.Load<Sprite>(BackgroundResourcePath);
        if (art != null)
        {
            bgImg.sprite = art;
            bgImg.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            Debug.LogWarning($"[TitleScreen] Missing sprite at Resources/{BackgroundResourcePath}. Using solid backdrop.");
            bgImg.color = new Color(0f, 0f, 0f, 0f);
        }

        bgImg.preserveAspect = false;
        bgImg.raycastTarget = false;
        var bgRT = bgImg.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        var dim = new GameObject("DimOverlay");
        dim.transform.SetParent(canvasGO.transform, false);
        var dimImg = dim.AddComponent<Image>();
        dimImg.color = new Color(0f, 0f, 0f, 0f);
        dimImg.raycastTarget = false;
        var dimRT = dimImg.rectTransform;
        dimRT.anchorMin = Vector2.zero;
        dimRT.anchorMax = Vector2.one;
        dimRT.offsetMin = Vector2.zero;
        dimRT.offsetMax = Vector2.zero;

        var btnGO = new GameObject("StartGameButton");
        btnGO.transform.SetParent(canvasGO.transform, false);
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.15f, 0.2f, 0f);
        var btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var btnRT = btnImg.rectTransform;
        btnRT.anchorMin = new Vector2(0.5f, 0.35f);
        btnRT.anchorMax = new Vector2(0.5f, 0.35f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.sizeDelta = new Vector2(360, 90);
        btnRT.anchoredPosition = Vector2.zero;

        var btnTextGO = new GameObject("Label");
        btnTextGO.transform.SetParent(btnGO.transform, false);
        var btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnText.text = "Start Game";
        btnText.color = new Color(1f, 1f, 1f, 0f);
        btnText.fontSize = 48f;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontStyle = FontStyles.Bold;
        var btnTextRT = btnText.rectTransform;
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        btn.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(gameObject);
        });

        StartCoroutine(FadeInUI(bgImg, dimImg, btnImg, btnText));
    }

    private IEnumerator FadeInUI(Image art, Image dim, Image btnBg, TextMeshProUGUI btnLabel)
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeInDuration);
            if (art.sprite != null)
                art.color = new Color(1f, 1f, 1f, k);
            else
                art.color = new Color(0f, 0f, 0f, k * 0.7f);
            dim.color = new Color(0f, 0f, 0f, k * 0.15f);
            btnBg.color = new Color(0.15f, 0.15f, 0.2f, k * 0.95f);
            btnLabel.color = new Color(1f, 1f, 1f, k);
            yield return null;
        }
    }
}
