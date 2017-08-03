using System;
using System.Collections.Generic;
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

    public override void GetTooltip(List<string> tooltip)
    {
        base.GetTooltip(tooltip);

        if (TargetTracker.target == this)
            tooltip.Add(FormatCodes.GOLD + "Selected" + FormatCodes.COL_E);
    }

    public override bool IsTooltipVisible()
    {
        return base.IsTooltipVisible() || TargetTracker.target == this;
    }
}
