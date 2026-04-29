using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    [SerializeField] private float iFrameDuration = 1f;

    public float currentHealth;
    private bool isInvincible = false; // starts false

    public bool IsAlive => currentHealth > 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive || isInvincible) return; // check if dead or invincible

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartCoroutine(IFrameRoutine()); // I Frames
    }

    private System.Collections.IEnumerator IFrameRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(iFrameDuration);

        isInvincible = false;
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