public class SpellHeal : SpellBase
{
    private float healingPotential;

    public SpellHeal(int levelRequirement, float castTime, float cooldown) : base(levelRequirement, castTime, cooldown, false, TargetType.SELF, 0F)
    {
    }

    public SpellHeal SetHealingPotential(float percent)
    {
        healingPotential = percent;

        return this;
    }

    public override void Cast(PlayerControllerBase controller)
    {
        controller.Heal(controller.playerData.GetMaxLife() * healingPotential);
    }

    public override bool VerifyCanCastSpell(PlayerControllerBase controller, bool hypothetical = false)
    {
        if (!base.VerifyCanCastSpell(controller, hypothetical))
            return false;
        if (controller.life >= controller.playerData.GetMaxLife())
        {
            if (!hypothetical)
                UIHelper.Alert("alerts.player.spell.maxhealth", "InGameError");
            return false;
        }

        return true;
    }
}
