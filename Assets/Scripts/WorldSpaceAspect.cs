using UnityEngine;

public class WorldSpaceAspect : MonoBehaviour {
    private void Start()
    {
        Canvas c = GetComponent<Canvas>();
        RectTransform rect = c.GetComponent<RectTransform>();

        Bounds bounds = c.worldCamera.OrthographicBounds();

        rect.anchoredPosition = new Vector3(bounds.center.x, bounds.center.y, 0);
        rect.sizeDelta = bounds.size;
    }
}
