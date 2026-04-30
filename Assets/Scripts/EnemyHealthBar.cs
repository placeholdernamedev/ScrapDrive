using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 2.5f, 0f);
    public Vector2 size = new Vector2(1f, 0.12f);
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);
    public Color fillColor = new Color(0.9f, 0.2f, 0.2f, 1f);
    public bool hideWhenFull = true;

    private EnemyHealth health;
    private Camera cam;
    private RectTransform fill;
    private GameObject canvasGO;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        cam = Camera.main;

        canvasGO = new GameObject("HealthBar", typeof(RectTransform), typeof(Canvas));
        canvasGO.transform.SetParent(transform, false);
        canvasGO.transform.localPosition = offset;

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 1;

        var canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = size;
        canvasRT.localScale = Vector3.one;

        var bgGO = new GameObject("BG", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        bgGO.GetComponent<Image>().color = backgroundColor;

        var fillGO = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillGO.transform.SetParent(canvasGO.transform, false);
        fill = fillGO.GetComponent<RectTransform>();
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = Vector2.one;
        fill.pivot = new Vector2(0f, 0.5f);
        float inset = size.y * 0.15f;
        fill.offsetMin = new Vector2(inset, inset);
        fill.offsetMax = new Vector2(-inset, -inset);
        fillGO.GetComponent<Image>().color = fillColor;
    }

    void LateUpdate()
    {
        if (health == null || canvasGO == null) return;
        if (cam == null) cam = Camera.main;

        float pct = Mathf.Clamp01(health.GetHealthPercent());

        if (hideWhenFull && pct >= 0.999f)
        {
            if (canvasGO.activeSelf) canvasGO.SetActive(false);
            return;
        }
        if (!canvasGO.activeSelf) canvasGO.SetActive(true);

        if (cam != null)
        {
            canvasGO.transform.rotation = Quaternion.LookRotation(canvasGO.transform.position - cam.transform.position);
        }

        if (fill != null)
        {
            fill.localScale = new Vector3(pct, 1f, 1f);
        }
    }
}
