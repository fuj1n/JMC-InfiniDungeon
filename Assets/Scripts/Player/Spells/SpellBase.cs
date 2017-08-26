using UnityEngine;

public abstract class SpellBase
{
    public readonly int levelRequirement;
    public readonly float castTime;
    public readonly float cooldown;
    public readonly bool requiresLineOfSight;
    public readonly TargetType spellTarget;
    public readonly float range;

    public string animation;
    public bool animateBeforeCast = true;
    public string icon;
    public string particle;

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

    public virtual bool VerifyCanCastSpell(PlayerControllerBase controller, bool hypothetical = false)
    {
        if (!CompareLevelRequirement(controller))
        {
            if (!hypothetical)
                UIHelper.Alert("alerts.player.spell.lowlevel", "InGameError");
        }
        else if (spellTarget == TargetType.SELECTED && !TargetTracker.target)
        {
            if (!hypothetical)
                UIHelper.Alert("alerts.player.spell.notarget", "InGameError");
        }
        else if (requiresLineOfSight && Vector3.Angle(TargetTracker.target.transform.position - controller.transform.position, controller.transform.forward) > 90)
        {
            if (!hypothetical)
                UIHelper.Alert("alerts.player.spell.notfacing", "InGameError");
        }
        else if (spellTarget == TargetType.SELECTED && Vector3.Distance(TargetTracker.target.transform.position, controller.transform.position) > range)
        {
            if (!hypothetical)
                UIHelper.Alert("alerts.player.spell.outofrange", "InGameError");
        }
        else
            return true;

        return false;
    }

    public virtual bool CompareLevelRequirement(PlayerControllerBase controller)
    {
        return controller.playerData.level >= levelRequirement;
    }

    public abstract void Cast(PlayerControllerBase controller);

    public SpellBase SetIcon(string icon)
    {
        this.icon = icon;

        return this;
    }

    public SpellBase SetAnimation(string animation, bool animateBeforeCast = true)
    {
        this.animation = animation;
        this.animateBeforeCast = animateBeforeCast;

        return this;
    }

    public SpellBase SetParticle(string particle)
    {
        this.particle = particle;

        return this;
    }

    public enum TargetType
    {
        SELF,
        SELECTED
    }
}
