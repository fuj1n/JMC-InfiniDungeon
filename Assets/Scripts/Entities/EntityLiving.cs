using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityLiving : Entity
{
    [NonSerialized]
    public float life;
    public float maxLife;
    public float invulnerabilityTime = 1F;

    protected bool isInvulnerable = false;

    protected virtual void Start()
    {
        life = maxLife;
    }

    public virtual void Damage(float rawDamage)
    {
        if (isInvulnerable)
            return;

        life -= rawDamage;

        StartCoroutine(InvulnerabilityTimer());

        if (life <= 0 && OnKill())
            Destroy(gameObject);
    }

    protected virtual IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false;
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
        return true;
    }

    private void OnMouseDown()
    {
        TargetTracker.target = this;
    }
}
