public class SpellAttack : SpellBase
{
    public float damage = 5F;
    public PlayerData.Stats.Stat primaryStat;

    public SpellAttack(int levelRequirement, float castTime, float cooldown, bool requiresLineOfSight = true, TargetType spellTarget = TargetType.SELECTED, float range = 25) : base(levelRequirement, castTime, cooldown, requiresLineOfSight, spellTarget, range)
    { }

    public SpellAttack SetDamage(float damage)
    {
        this.damage = damage;

        return this;
    }

    public SpellAttack SetPrimaryStat(PlayerData.Stats.Stat primaryStat)
    {
        this.primaryStat = primaryStat;

        return this;
    }

    public override void Cast(PlayerControllerBase controller)
    {
        TargetTracker.target.Damage(controller, damage * controller.playerData.GetDamageMultiplierForStat(primaryStat));
    }
}
