using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 offset;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + offset, new Vector3(2, 0, 2));
    }
}
