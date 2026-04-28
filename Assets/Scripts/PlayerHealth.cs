using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public bool IsAlive => currentHealth > 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died");
        var controller = GetComponent<PlayerMovement>();
        var vehicleInteraction = GetComponent<VehicleInteraction>();

        if (controller != null)
        {
            controller.enabled = false;
        }
        if (vehicleInteraction != null)
        {
            vehicleInteraction.enabled = false;
        }

        // add effects later
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}