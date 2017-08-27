using System;
using UnityEngine;

public class EntityEnemy : EntityLiving, IComparable<EntityEnemy>
{
    [Header("Generator")]
    public int weight;

    [Header("Entity")]
    public new string name;

    private float id;

    private void Awake()
    {
        faction = TargetableFaction.ENEMY;

        id = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    public int CompareTo(EntityEnemy other)
    {
        return other == null || other.id > id ? 1 : other.id < id ? -1 : 0;
    }

    public override string GetName()
    {
        return "entity." + name + ".name";
    }

    public override bool IsTooltipVisible()
    {
        return base.IsTooltipVisible() || TargetTracker.target == this;
    }

    public override bool OnKill()
    {
        DoDrops();

        return base.OnKill();
    }

    public virtual void DoDrops()
    {
        if (random.NextBool(25))
        {
            GameObject itemDrop = new GameObject("itemDrop");
            itemDrop.transform.position = transform.position;

            EntityItem item = itemDrop.AddComponent<EntityItem>();
            item.stack = DropGenerator.Generate();
        }
    }
}
