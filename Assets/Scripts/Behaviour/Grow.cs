using UnityEngine;

public class Grow : MonoBehaviour {
    public Vector3 maxSize;
    public float time;

    private float currentTime;
    private Vector3 startSize;

    private void Awake()
    {
        currentTime = 0F;
        startSize = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(startSize, maxSize, currentTime / time);

        currentTime += Time.deltaTime;
    }
}
