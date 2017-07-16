using System.Collections.Generic;

public abstract class EntityLiving : Entity
{
    public float life;

    public virtual void Damage(float rawDamage)
    {
        life -= rawDamage;
    }

    public override void GetTooltip(List<string> tooltip)
    {
        base.GetTooltip(tooltip);

        tooltip.Add(FormatCodes.RESET + FormatCodes.DARK_GREY + FormatCodes.ITALIC + "Health: " + life);
    }
}
