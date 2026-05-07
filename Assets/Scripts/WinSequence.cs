using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WinSequence : MonoBehaviour
{
    public Transform dataCenter;
    public string newGameSceneName = "Level1";

    public float zoomOutDistance = 350f;
    public float zoomOutHeight = 180f;
    public float zoomDuration = 3.5f;
    public float explosionDuration = 6f;
    public float fadeInDuration = 1.5f;

    private bool fired = false;

    void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag("Car")) return;
        fired = true;
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        // Find camera FIRST so we can bail safely if missing
        var cam = Camera.main;
        if (cam == null)
        {
            var cams = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            if (cams.Length > 0) cam = cams[0];
        }
        if (cam == null)
        {
            Debug.LogError("[WinSequence] No camera in scene. Skipping cinematic, just showing UI.");
            ShowWinUI();
            yield break;
        }

        // Disable script behaviours on player and car (NOT colliders/renderers/cameras)
        var disabled = new List<Behaviour>();
        var player = GameObject.Find("Player");
        var car = GameObject.Find("Car With Turret");
        if (player != null)
        {
            foreach (var b in player.GetComponentsInChildren<MonoBehaviour>())
                if (b != null && b != this && b.enabled) { b.enabled = false; disabled.Add(b); }
        }
        if (car != null)
        {
            foreach (var b in car.GetComponentsInChildren<MonoBehaviour>())
                if (b != null && b != this && b.enabled) { b.enabled = false; disabled.Add(b); }
            var rb = car.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Disable ONLY MonoBehaviours on the camera (NOT the Camera component itself)
        // Camera component IS a Behaviour and disabling it stops rendering.
        foreach (var b in cam.GetComponents<MonoBehaviour>())
            if (b != null && b.enabled) { b.enabled = false; disabled.Add(b); }
        cam.transform.SetParent(null);

        Vector3 dcCenter = dataCenter != null ? dataCenter.position + new Vector3(0, 100f, 0) : transform.position;
        Vector3 backDir = (cam.transform.position - dcCenter);
        backDir.y = 0;
        if (backDir.sqrMagnitude < 0.01f) backDir = new Vector3(0, 0, -1);
        backDir.Normalize();

        Vector3 startPos = cam.transform.position;
        Vector3 endPos = dcCenter + backDir * zoomOutDistance + Vector3.up * zoomOutHeight;
        Quaternion startRot = cam.transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(dcCenter - endPos);

        StartCoroutine(ExplosionSequence(dcCenter));

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / zoomDuration);
            if (cam != null)
            {
                cam.transform.position = Vector3.Lerp(startPos, endPos, k);
                cam.transform.rotation = Quaternion.Slerp(startRot, endRot, k);
            }
            yield return null;
        }

        yield return new WaitForSeconds(Mathf.Max(0f, explosionDuration - zoomDuration));

        ShowWinUI();
    }

    IEnumerator ExplosionSequence(Vector3 dcCenter)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-130f, 130f),
                Random.Range(-100f, 110f),
                Random.Range(-70f, 70f));
            CreateFireball(dcCenter + offset, Random.Range(60f, 110f));
            yield return new WaitForSeconds(Random.Range(0.08f, 0.25f));
        }
    }

    void CreateFireball(Vector3 pos, float size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Fireball";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * (size * 0.4f);
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);

        var rend = go.GetComponent<Renderer>();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        var mat = new Material(shader);
        Color baseCol = new Color(1f, 0.55f, 0.1f);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseCol);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseCol);
        rend.material = mat;

        go.AddComponent<FireballAnimator>().Init(size, mat);
    }

    void ShowWinUI()
    {
        var canvasGO = new GameObject("WinCanvas");
        DontDestroyOnLoad(canvasGO);

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
            DontDestroyOnLoad(es);
        }

        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0f);
        var bgRT = bgImg.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        var titleGO = new GameObject("WinText");
        titleGO.transform.SetParent(canvasGO.transform, false);
        var tmp = titleGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "YOU WIN";
        tmp.color = new Color(1f, 1f, 1f, 0f);
        tmp.fontSize = 160f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        var trt = tmp.rectTransform;
        trt.anchorMin = new Vector2(0f, 0.55f);
        trt.anchorMax = new Vector2(1f, 0.55f);
        trt.pivot = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(0, 220);
        trt.anchoredPosition = Vector2.zero;

        var btnGO = new GameObject("NewGameButton");
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
        btnText.text = "New Game";
        btnText.color = new Color(1f, 1f, 1f, 0f);
        btnText.fontSize = 48f;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontStyle = FontStyles.Bold;
        var btnTextRT = btnText.rectTransform;
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        string sceneName = newGameSceneName;
        btn.onClick.AddListener(() => {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        });

        StartCoroutine(FadeInUI(bgImg, tmp, btnImg, btnText));
    }

    IEnumerator FadeInUI(Image bg, TextMeshProUGUI title, Image btnBg, TextMeshProUGUI btnLabel)
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeInDuration);
            bg.color = new Color(0f, 0f, 0f, k * 0.7f);
            title.color = new Color(1f, 1f, 1f, k);
            btnBg.color = new Color(0.15f, 0.15f, 0.2f, k * 0.95f);
            btnLabel.color = new Color(1f, 1f, 1f, k);
            yield return null;
        }
    }
}

public class FireballAnimator : MonoBehaviour
{
    private float maxSize;
    private Material mat;
    private float lifetime = 2.0f;
    private float t = 0f;

    public void Init(float size, Material m)
    {
        maxSize = size;
        mat = m;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / lifetime);
        float scaleFactor = Mathf.SmoothStep(0.4f, 2.5f, Mathf.Clamp01(k * 2f));
        transform.localScale = Vector3.one * (maxSize * scaleFactor);

        Color col;
        if (k < 0.3f) col = Color.Lerp(new Color(1f, 0.9f, 0.3f), new Color(1f, 0.55f, 0.1f), k / 0.3f);
        else if (k < 0.7f) col = Color.Lerp(new Color(1f, 0.55f, 0.1f), new Color(0.9f, 0.15f, 0.05f), (k - 0.3f) / 0.4f);
        else col = Color.Lerp(new Color(0.9f, 0.15f, 0.05f), new Color(0.15f, 0.05f, 0.05f), (k - 0.7f) / 0.3f);

        if (mat != null)
        {
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", col);
        }

        if (k >= 1f) Destroy(gameObject);
    }
}
