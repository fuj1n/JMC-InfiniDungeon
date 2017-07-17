using System;
using UnityEngine;

public class DungeonGateway : MonoBehaviour
{
    [NonSerialized]
    public Transform player;
    [NonSerialized]
    public Transform gatewayDestination;

    private bool isPlayerInRange = false;

    private void OnMouseDown()
    {
        if (isPlayerInRange)
        {
            player.transform.position = gatewayDestination.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
