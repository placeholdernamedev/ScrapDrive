using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    public float maxHealth = 100f;
    [SerializeField] private float iFrameDuration = 1f;

    public float currentHealth;
    public AudioSource audioDamageSource;
    public AudioClip damageAudio;
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

        audioDamageSource.PlayOneShot(damageAudio);
        
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
        if (!IsAlive || amount <= 0f) return;

        float missingHealth = Mathf.Max(0f, maxHealth - currentHealth);
        float healAmount = Mathf.Min(amount, missingHealth);
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void ForceKillForGameOver()
    {
        StopAllCoroutines();
        isInvincible = false;
        currentHealth = 0f;
        Die();
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

        GameOverRuntime.Show();
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}