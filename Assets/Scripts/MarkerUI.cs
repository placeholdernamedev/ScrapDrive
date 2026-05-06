using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkerUI : MonoBehaviour
{
    public Transform target;
    public Image icon;
    public TMP_Text distanceText;

    private Camera cam;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            return;
        }

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        transform.position = screenPos;

        float distance = Vector3.Distance(cam.transform.position, target.position);
        if (distanceText != null)
        {
            distanceText.text = Mathf.RoundToInt(distance) + "m";
        }

        bool behindCamera = screenPos.z < 0f;
        if (behindCamera)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
