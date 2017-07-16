using System;
using UnityEngine;

public class DungeonTile : MonoBehaviour, IComparable<DungeonTile>
{
    public int weight;

    private float id;

    private void Awake()
    {
        id = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    public int CompareTo(DungeonTile other)
    {
        return other == null || other.id > id ? 1 : other.id < id ? -1 : 0;
    }
}
