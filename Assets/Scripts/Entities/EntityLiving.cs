using System;
using System.Collections.Generic;

public abstract class EntityLiving : Entity
{
    [NonSerialized]
    public float life;
    public float maxLife;

    public int xpReward = 1;

    protected virtual void Start()
    {
        life = maxLife;
    }

    public override void Damage(Targetable source, float rawDamage)
    {
        base.Damage(source, rawDamage);

        life -= rawDamage;

        if (life <= 0 && OnKill())
            Destroy(gameObject);
    }

    public override void GetTooltip(List<string> tooltip)
    {
        base.GetTooltip(tooltip);

        tooltip.Add(FormatCodes.RED + FormatCodes.ITALIC + "entity.label.health: " + life + FormatCodes.ITALIC_E + FormatCodes.COL_E);

        if (TargetTracker.target == this)
            tooltip.Add(FormatCodes.GOLD + "entity.label.selected" + FormatCodes.COL_E);
    }

    public virtual bool OnKill()
    {
        PlayerData.Instance.GrantExperience(xpReward);

        return true;
    }

    private void OnMouseDown()
    {
        TargetTracker.target = this;
    }
}
