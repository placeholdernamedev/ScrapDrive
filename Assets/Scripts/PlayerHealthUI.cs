using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth health;
    public RectTransform fillBar;

    public TextMeshProUGUI healthText;

    float fullWidth;

    void Start()
    {
        fullWidth = fillBar.sizeDelta.x;
    }

    void Update()
    {
        if (health == null) return;

        float percent = health.currentHealth / health.maxHealth;

        fillBar.sizeDelta = new Vector2(fullWidth * percent, fillBar.sizeDelta.y);

        healthText.text = "Player Health: " + Mathf.RoundToInt(health.currentHealth) + " / " + health.maxHealth;
    }
}