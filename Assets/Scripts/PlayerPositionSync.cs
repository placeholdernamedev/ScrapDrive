using UnityEngine;

// keeps the player transform's position synced to this object's position
// while the player gameobject is inactive (i.e. while in the car).
// this way enemy AI tracking the player transform actually chases the car.
public class PlayerPositionSync : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null && !player.gameObject.activeSelf)
        {
            player.position = transform.position;
        }
    }
}
