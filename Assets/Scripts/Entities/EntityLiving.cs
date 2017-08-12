using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityLiving : Entity
{
    [NonSerialized]
    public float life;
    public float maxLife;

    public int xpReward = 1;

    protected virtual void Start()
    {
        life = 1F;
    }

    public override void Damage(Targetable source, float rawDamage)
    {
        base.Damage(source, rawDamage);

        life -= rawDamage / maxLife;

        if (life <= 0 && OnKill())
            Destroy(gameObject);
    }

    public void Heal(float health)
    {
        life = Mathf.Clamp(life + health / maxLife, 0F, 1F);
    }

    public override void GetTooltip(List<string> tooltip)
    {
        base.GetTooltip(tooltip);

        tooltip.Add(FormatCodes.RED + FormatCodes.ITALIC + "entity.label.health: " + (int)(life * maxLife) + FormatCodes.ITALIC_E + FormatCodes.COL_E);

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
