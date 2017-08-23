using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFader : MonoBehaviour
{
    public Transform trackedObject;
    public DistanceType distanceType;

    [ColorUsage(true)]
    public Color zeroColor = new Color(1F, 1F, 1F, 0F);
    [ColorUsage(true)]
    public Color oneColor = Color.white;

    public float zeroDistance = 5F;
    public float oneDistance = 10F;

    private Material mat;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetInt("_ZWrite", 1); // Enables depth filtering, fixes weird ordering effects of FADE shading mode
    }

    void Update()
    {
        float rawDistance = Vector3.Distance(transform.position, trackedObject.position);

        switch (distanceType)
        {
            case DistanceType.TRACK_X:
                rawDistance = trackedObject.localPosition.x;
                break;
            case DistanceType.TRACK_Y:
                rawDistance = trackedObject.localPosition.y;
                break;
            case DistanceType.TRACK_Z:
                rawDistance = trackedObject.localPosition.z;
                break;
        }

        float distance = Mathf.Clamp((rawDistance - zeroDistance) / (oneDistance - zeroDistance), 0, 1);

        mat.color = Color.Lerp(zeroColor, oneColor, distance);
    }

    public enum DistanceType
    {
        VECTOR_CALC,
        TRACK_X,
        TRACK_Y,
        TRACK_Z
    }
}
