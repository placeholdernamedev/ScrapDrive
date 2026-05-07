using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurretBoostRuntime : MonoBehaviour
{
    private const float FlashThreshold = 10f;
    private static TurretBoostRuntime instance;

    public static bool IsActive => instance != null && instance.remainingTime > 0f;
    public static float RemainingTime => instance != null ? Mathf.Max(0f, instance.remainingTime) : 0f;

    private float remainingTime;
    private Canvas canvas;
    private Image iconContainer;
    private Image iconBase;
    private Image iconBarrel;
    private Image iconGlow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureExistsOnSceneLoad()
    {
        EnsureInstance();
    }

    public static void Activate(float durationSeconds)
    {
        if (durationSeconds <= 0f)
        {
            return;
        }

        EnsureInstance();
        instance.remainingTime = Mathf.Max(instance.remainingTime, durationSeconds);
        instance.SetIconVisible(true);
    }

    public static void ResetBoost()
    {
        if (instance == null)
        {
            return;
        }

        instance.remainingTime = 0f;
        instance.SetIconVisible(false);
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject runtime = new GameObject("TurretBoostRuntime");
        DontDestroyOnLoad(runtime);
        instance = runtime.AddComponent<TurretBoostRuntime>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetIconVisible(false);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Keep icon hidden when no boost is active after scene transitions.
        SetIconVisible(remainingTime > 0f);
    }

    private void Update()
    {
        if (remainingTime <= 0f)
        {
            if (iconContainer != null && iconContainer.gameObject.activeSelf)
            {
                SetIconVisible(false);
            }
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            SetIconVisible(false);
            return;
        }

        if (remainingTime <= FlashThreshold)
        {
            float pulse = Mathf.PingPong(Time.unscaledTime * 3.5f, 1f);
            float alpha = Mathf.Lerp(0.25f, 1f, pulse);
            SetIconAlpha(alpha);
            iconGlow.color = Color.Lerp(new Color(1f, 0.5f, 0.18f, alpha), new Color(1f, 0.1f, 0.1f, alpha), pulse);
        }
        else
        {
            SetIconAlpha(1f);
            iconGlow.color = new Color(0.25f, 0.9f, 1f, 0.7f);
        }
    }

    private void BuildUI()
    {
        GameObject canvasGo = new GameObject("TurretBoostCanvas");
        canvasGo.transform.SetParent(transform, false);
        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 8500;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject containerGo = new GameObject("TurretBoostIcon");
        containerGo.transform.SetParent(canvasGo.transform, false);
        iconContainer = containerGo.AddComponent<Image>();
        iconContainer.color = new Color(0.03f, 0.04f, 0.08f, 0.7f);
        RectTransform cRt = iconContainer.rectTransform;
        cRt.anchorMin = new Vector2(0.5f, 0f);
        cRt.anchorMax = new Vector2(0.5f, 0f);
        cRt.pivot = new Vector2(0.5f, 0.5f);
        cRt.anchoredPosition = new Vector2(0f, 66f);
        cRt.sizeDelta = new Vector2(74f, 74f);

        iconGlow = CreatePart("Glow", iconContainer.transform, new Color(0.25f, 0.9f, 1f, 0.7f), new Vector2(0f, 0f), new Vector2(68f, 68f));
        iconBase = CreatePart("Base", iconContainer.transform, new Color(0.8f, 0.86f, 0.95f, 1f), new Vector2(0f, -8f), new Vector2(24f, 24f));
        iconBarrel = CreatePart("Barrel", iconContainer.transform, new Color(0.9f, 0.95f, 1f, 1f), new Vector2(0f, 12f), new Vector2(10f, 34f));
    }

    private static Image CreatePart(string name, Transform parent, Color color, Vector2 anchoredPos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return image;
    }

    private void SetIconVisible(bool isVisible)
    {
        if (iconContainer != null)
        {
            iconContainer.gameObject.SetActive(isVisible);
        }
    }

    private void SetIconAlpha(float alpha)
    {
        Color c = iconContainer.color;
        iconContainer.color = new Color(c.r, c.g, c.b, 0.4f + alpha * 0.4f);

        Color b = iconBase.color;
        iconBase.color = new Color(b.r, b.g, b.b, alpha);

        Color br = iconBarrel.color;
        iconBarrel.color = new Color(br.r, br.g, br.b, alpha);
    }
}
