using UnityEngine;

public class CarHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 200f;
    [SerializeField] private float iFrameDuration = 1f; // in seconds

    public float currentHealth;
    private bool isDestroyed = false;
    private bool isInvincible = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed || isInvincible) return; // checks if invincible or dead

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            DestroyCar();
            return;
        }

        StartCoroutine(IFrameRoutine()); // start I frames
    }

    private System.Collections.IEnumerator IFrameRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(iFrameDuration);

        isInvincible = false;
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

        Destroy(gameObject, 3f);
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}