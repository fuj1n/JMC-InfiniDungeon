using UnityEngine;

public class StickToCursor : MonoBehaviour
{
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.position = Input.mousePosition;
    }
}
