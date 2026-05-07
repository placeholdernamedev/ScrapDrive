using UnityEngine;
using UnityEngine.SceneManagement;

public class CarHealth : MonoBehaviour, IDamageable, IHealable
{
    public float maxHealth = 200f;
    [SerializeField] private float iFrameDuration = 1f; // in seconds

    public float currentHealth;
    public AudioSource audioDamageSource;
    public AudioClip damageAudio;
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

        audioDamageSource.PlayOneShot(damageAudio);

        if (currentHealth <= 0f)
        {
            DestroyCar();
            return;
        }

        StartCoroutine(IFrameRoutine()); // start I frames
    }

    public void Heal(float amount)
    {
        if (isDestroyed || amount <= 0f) return;

        float missingHealth = Mathf.Max(0f, maxHealth - currentHealth);
        float healAmount = Mathf.Min(amount, missingHealth);
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
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
            vehicleInteraction.ForceExitIfInVehicle();
            vehicleInteraction.enabled = false;
        }

        if (SceneManager.GetActiveScene().name == "PlaytestLevel")
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
            {
                PlayerHealth playerHealth = playerGo.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ForceKillForGameOver();
                }
            }
        }

        Destroy(gameObject, 3f);
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}