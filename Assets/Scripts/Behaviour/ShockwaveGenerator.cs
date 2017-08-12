using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShockwaveGenerator : MonoBehaviour {
    public GameObject shockwave;
    public float shockwaveTime;

    private bool isGrounded = true;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void LateUpdate()
    {
        /*Ray r = new Ray();
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, col.bounds.extents.y + .1F, out hit)){

        }*/
    }
}
