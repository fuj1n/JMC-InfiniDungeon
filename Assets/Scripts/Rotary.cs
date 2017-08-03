using UnityEngine;

public class Rotary : MonoBehaviour
{
    public float sensitivity = .4F;

    private Vector3 mouseStart;
    private bool isRotating;

    void Update()
    {
        if (isRotating)
        {
            Vector3 mouseOffset = (Input.mousePosition - mouseStart);
            transform.Rotate(0F, -(mouseOffset.x + mouseOffset.y) * sensitivity, 0F);
            mouseStart = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        isRotating = true;
        mouseStart = Input.mousePosition;
    }

    void OnMouseUp()
    {
        isRotating = false;
    }

}