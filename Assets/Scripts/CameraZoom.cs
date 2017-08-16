using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float maxZoom = 0F;
    public float minZoom = -3F;

    public float speed = 2F;

    private float currentZoom;

    private void Awake()
    {
        currentZoom = transform.localPosition.z;
    }

    private void Update()
    {
        float mouseMovement = Input.GetAxis("Mouse ScrollWheel");
        currentZoom = Mathf.Clamp(currentZoom + mouseMovement, minZoom, maxZoom);

        Vector3 lp = transform.localPosition;

        float allowedZoom = currentZoom;
        RaycastHit hit;

        if (Physics.Raycast(new Ray(transform.parent.TransformPoint(new Vector3(lp.x, lp.y, 0)), -transform.parent.forward), out hit, Mathf.Abs(allowedZoom)))
            allowedZoom = minZoom < 0 ? -hit.distance + .2F : hit.distance + .2F;

        if ((minZoom < 0 && lp.z < allowedZoom) || (minZoom >= 0 && lp.z > allowedZoom))
            transform.localPosition = new Vector3(lp.x, lp.y, allowedZoom);
        else
            transform.localPosition = Vector3.Lerp(lp, new Vector3(lp.x, lp.y, allowedZoom), Time.deltaTime * speed);
    }
}
