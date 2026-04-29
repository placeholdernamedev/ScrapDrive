using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarHealthUI : MonoBehaviour
{
    public CarHealth health;
    public VehicleInteraction vehicleInteraction;
    public RectTransform Background;
    public RectTransform fillBar;

    public TextMeshProUGUI healthText;

    float fullWidth;

    void Start()
    {
        if (vehicleInteraction == null && health != null)
        {
            vehicleInteraction = health.GetComponent<VehicleInteraction>();
        }

        fullWidth = fillBar.sizeDelta.x;
        RefreshVisibility();
    }

    void Update()
    {
        if (health == null) return;

        RefreshVisibility();

        float percent = health.currentHealth / health.maxHealth;

        fillBar.sizeDelta = new Vector2(fullWidth * percent, fillBar.sizeDelta.y);

        healthText.text = "Car Health: " + Mathf.RoundToInt(health.currentHealth) + " / " + health.maxHealth;
    }

    void RefreshVisibility()
    {
        bool show = vehicleInteraction == null || vehicleInteraction.InVehicle;

        if (fillBar != null)
        {
            fillBar.gameObject.SetActive(show);
        }

        if (healthText != null)
        {
            healthText.gameObject.SetActive(show);
        }

        if (Background != null)
        {
            Background.gameObject.SetActive(show);
        }
    }
}
