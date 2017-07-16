using UnityEngine;

public class PlayerSpawn : MonoBehaviour {

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1F, 0F, 1F));
    }
}
