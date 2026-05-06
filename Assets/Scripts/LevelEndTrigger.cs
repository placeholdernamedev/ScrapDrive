using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelEndTrigger : MonoBehaviour
{
    public GameObject completeUI;

    [Header("Scene Transition")]
    public string nextSceneName = "";
    public string nextLevelDisplayName = "";
    public float fadeInTime = 1.2f;
    public float holdTime = 1.5f;
    public float fadeOutTime = 1.0f;

    private bool fired = false;

    void Awake()
    {
        if (completeUI != null) completeUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag("Car")) return;

        fired = true;

        if (completeUI != null) completeUI.SetActive(true);

        if (!string.IsNullOrEmpty(nextSceneName))
            SpawnFader();
    }

    void SpawnFader()
    {
        var canvasGO = new GameObject("LevelEndFadeCanvas");
        DontDestroyOnLoad(canvasGO);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);
        img.raycastTarget = false;
        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var txtGO = new GameObject("LevelNameText");
        txtGO.transform.SetParent(canvasGO.transform, false);
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = nextLevelDisplayName;
        tmp.color = new Color(1f, 1f, 1f, 0f);
        tmp.fontSize = 92f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
        var trt = tmp.rectTransform;
        trt.anchorMin = new Vector2(0f, 0.5f);
        trt.anchorMax = new Vector2(1f, 0.5f);
        trt.pivot = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(0, 220);
        trt.anchoredPosition = Vector2.zero;

        var runner = canvasGO.AddComponent<LevelEndFadeRunner>();
        runner.fadeImage = img;
        runner.nameText = tmp;
        runner.targetScene = nextSceneName;
        runner.fadeInTime = fadeInTime;
        runner.holdTime = holdTime;
        runner.fadeOutTime = fadeOutTime;
    }
}

public class LevelEndFadeRunner : MonoBehaviour
{
    [HideInInspector] public Image fadeImage;
    [HideInInspector] public TextMeshProUGUI nameText;
    [HideInInspector] public string targetScene;
    [HideInInspector] public float fadeInTime = 1.2f;
    [HideInInspector] public float holdTime = 1.5f;
    [HideInInspector] public float fadeOutTime = 1.0f;

    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        for (float t = 0f; t < fadeInTime; t += Time.unscaledDeltaTime)
        {
            float a = Mathf.Clamp01(t / fadeInTime);
            if (fadeImage != null) fadeImage.color = new Color(0f, 0f, 0f, a);
            if (nameText != null)  nameText.color  = new Color(1f, 1f, 1f, a);
            yield return null;
        }
        if (fadeImage != null) fadeImage.color = Color.black;
        if (nameText != null)  nameText.color  = Color.white;

        yield return new WaitForSecondsRealtime(holdTime);

        var op = SceneManager.LoadSceneAsync(targetScene);
        while (op != null && !op.isDone) yield return null;

        yield return null;

        for (float t = 0f; t < fadeOutTime; t += Time.unscaledDeltaTime)
        {
            float a = 1f - Mathf.Clamp01(t / fadeOutTime);
            if (fadeImage != null) fadeImage.color = new Color(0f, 0f, 0f, a);
            if (nameText != null)  nameText.color  = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
