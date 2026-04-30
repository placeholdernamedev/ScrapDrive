using UnityEngine;

// hides the MeshRenderer on this object once the game starts.
// stays visible in the editor for placement, invisible at runtime.
public class HideOnPlay : MonoBehaviour
{
    void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
    }
}
