using UnityEngine;

// New script written for ScrapDrive.
// Drives the activation of an EnemySpawner GameObject when something enters the zone trigger.
// Doesn't touch any existing scripts.
public class EncounterZone : MonoBehaviour
{
    [Tooltip("The GameObject containing the EnemySpawner. Will be disabled in Awake and enabled on trigger.")]
    public GameObject spawnerObject;

    [Tooltip("Activate when something tagged 'Car' enters?")]
    public bool triggerOnCarEnter = true;

    [Tooltip("Activate when something tagged 'Player' enters?")]
    public bool triggerOnPlayerEnter = false;

    [Tooltip("If true, the zone only activates once. If false, leaves the spawner running indefinitely after first entry.")]
    public bool oneShot = false;

    [Tooltip("If oneShot, how long the spawner stays enabled before being disabled again. 0 = never auto-disable.")]
    public float oneShotDuration = 0f;

    private bool activated = false;

    void Awake()
    {
        if (spawnerObject != null) spawnerObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated && oneShot) return;

        bool match =
            (triggerOnCarEnter && other.CompareTag("Car")) ||
            (triggerOnPlayerEnter && other.CompareTag("Player"));

        if (!match) return;
        if (spawnerObject == null) return;

        spawnerObject.SetActive(true);
        activated = true;

        if (oneShot && oneShotDuration > 0f)
        {
            Invoke(nameof(DeactivateSpawner), oneShotDuration);
        }
    }

    void DeactivateSpawner()
    {
        if (spawnerObject != null) spawnerObject.SetActive(false);
    }
}
