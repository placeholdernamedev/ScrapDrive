using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    [SerializeField] private float iFrameDuration = 0.1f;

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
            DestroyEnemy();
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

    private void DestroyEnemy()
    {
        isDestroyed = true;

        Debug.Log("Enemy Destroyed");

        var ai = GetComponent<MeleeEnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        Destroy(gameObject, 0.5f);
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}
