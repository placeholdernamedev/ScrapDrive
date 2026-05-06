using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkerUI : MonoBehaviour
{
    public Transform target;
    public Image icon;
    public TMP_Text distanceText;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        // Always show (even behind walls)
        transform.position = screenPos;

        float distance = Vector3.Distance(cam.transform.position, target.position);
        distanceText.text = Mathf.RoundToInt(distance) + "m";

        // Optional: hide if behind camera
        if (screenPos.z < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}