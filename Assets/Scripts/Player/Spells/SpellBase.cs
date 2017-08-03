using UnityEngine;

public abstract class SpellBase
{
    public readonly int levelRequirement;
    public readonly float castTime;
    public readonly float cooldown;
    public readonly bool requiresLineOfSight;
    public readonly TargetType spellTarget;
    public readonly float range;

    public SpellBase(int levelRequirement, float castTime, float cooldown, bool requiresLineOfSight = true, TargetType spellTarget = TargetType.SELECTED, float range = 25F)
    {
        this.levelRequirement = levelRequirement;
        this.castTime = castTime;
        this.cooldown = cooldown;
        this.requiresLineOfSight = requiresLineOfSight;
        this.spellTarget = spellTarget;
        this.range = range;

        if (this.spellTarget == TargetType.SELF)
            this.requiresLineOfSight = false;
    }

    public virtual bool VerifyCanCastSpell(PlayerControllerBase controller)
    {
        if (controller.playerData.level < levelRequirement)
            UIHelper.Alert("alerts.player.spell.lowlevel", "InGameError");
        else if (spellTarget == TargetType.SELECTED && !TargetTracker.target)
            UIHelper.Alert("alerts.player.spell.notarget", "InGameError");
        else if (requiresLineOfSight && Vector3.Angle(TargetTracker.target.transform.position - controller.transform.position, controller.transform.forward) > 135)
            UIHelper.Alert("alerts.player.spell.notfacing", "InGameError");
        else
            return true;

        return false;
    }

    public abstract void Cast(PlayerControllerBase controller);

    public enum TargetType
    {
        SELF,
        SELECTED
    }
}
