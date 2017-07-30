using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityLiving : Entity
{
    public float life;
    public float invulnerabilityTime = 1F;

    protected bool isInvulnerable = false;

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

        tooltip.Add(FormatCodes.RESET + FormatCodes.DARK_GREY + FormatCodes.ITALIC + "Health: " + life);
    }

    public virtual bool OnKill()
    {
        return true;
    }
}
