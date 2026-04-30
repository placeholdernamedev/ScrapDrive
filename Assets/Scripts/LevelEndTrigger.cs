using UnityEngine;

// activates a UI element when the car enters this trigger.
// used for level end / win condition.
public class LevelEndTrigger : MonoBehaviour
{
    public GameObject completeUI;

    private bool fired = false;

    void Awake()
    {
        if (completeUI != null) completeUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag("Car")) return;
        if (completeUI == null) return;

        completeUI.SetActive(true);
        fired = true;
    }
}
