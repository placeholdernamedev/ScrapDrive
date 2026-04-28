using UnityEngine;

public class CarHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 200f;

    private float currentHealth;
    private bool isDestroyed = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            DestroyCar();
        }
    }

    private void DestroyCar()
    {
        isDestroyed = true;

        Debug.Log("Car Destroyed");

        var controller = GetComponent<CarMovement>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        var vehicleInteraction = GetComponent<VehicleInteraction>();
        if (vehicleInteraction != null)
        {
            vehicleInteraction.enabled = false;
        }

        Destroy(gameObject, 3f); // delay of 3 seconds before destruction.
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}