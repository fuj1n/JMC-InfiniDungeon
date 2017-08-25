public class MageController : PlayerControllerBase
{
    protected override void FillSpells()
    {
        spells.Add(new SpellAttack(1, 2F, 0F).SetDamage(10F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE).SetIcon("Weapon_19").SetAnimation("Cast"));
        spells.Add(new SpellAttack(5, 0F, 2F).SetDamage(5F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE).SetIcon("Weapon_20").SetAnimation("Cast"));
        spells.Add(new SpellAttack(10, 5F, 0F).SetDamage(30F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE).SetIcon("Weapon_17").SetAnimation("Cast"));
        spells.Add(new SpellHeal(3, 5F, 30F).SetHealingPotential(.5F).SetIcon("Weapon_16"));
    }
}
