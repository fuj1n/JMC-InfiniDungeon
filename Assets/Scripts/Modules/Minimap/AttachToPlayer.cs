using UnityEngine;

public class AttachToPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position;
    }

    private void Update()
    {
        transform.position = player.position + offset;
    }
}
