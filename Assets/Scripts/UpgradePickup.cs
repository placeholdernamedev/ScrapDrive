using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public enum UpgradeType
    {
        CarHealthBoost,
        GasBoost,
        PlayerHealthBoost
    }

    public UpgradeType upgradeType;
    public float carHealthAmount = 40f;
    public float gasAmount = 50f;
    public float playerHealthAmount = 30f;

    private CarHealth carHealth;
    private FuelSystem fuelSystem;
    private PlayerHealth playerHealth;

    private void Start()
    {
        carHealth = FindFirstObjectByType<CarHealth>();
        fuelSystem = FindFirstObjectByType<FuelSystem>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        ApplyUpgrade();
        Destroy(gameObject);
    }

    private void ApplyUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.CarHealthBoost:
                if (carHealth != null)
                {
                    carHealth.Heal(carHealthAmount);
                }
                break;
            case UpgradeType.GasBoost:
                if (fuelSystem != null)
                {
                    fuelSystem.Refuel(gasAmount);
                }
                break;
            case UpgradeType.PlayerHealthBoost:
                if (playerHealth != null)
                {
                    playerHealth.Heal(playerHealthAmount);
                }
                break;
        }
    }
}
