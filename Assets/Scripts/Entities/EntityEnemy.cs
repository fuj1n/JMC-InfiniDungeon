using System;

public abstract class EntityEnemy : EntityLiving, IComparable<EntityEnemy>
{
    public int weight;

    private float id;

    private void Awake()
    {
        id = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    public int CompareTo(EntityEnemy other)
    {
        return other == null || other.id > id ? 1 : other.id < id ? -1 : 0;
    }
}
