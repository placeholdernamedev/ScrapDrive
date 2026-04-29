using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelBarUI : MonoBehaviour
{
    public FuelSystem fuelSystem;
    public RectTransform fillBar;

    public TextMeshProUGUI fuelText;

    float fullWidth;

    void Start()
    {
        fullWidth = fillBar.sizeDelta.x;
    }

    void Update()
    {
        if (fuelSystem == null) return;

        float percent = fuelSystem.currentFuel / fuelSystem.maxFuel;

        fillBar.sizeDelta = new Vector2(fullWidth * percent, fillBar.sizeDelta.y);

        fuelText.text = "Fuel: " + Mathf.RoundToInt(fuelSystem.currentFuel) + " / " + fuelSystem.maxFuel;
    }
}