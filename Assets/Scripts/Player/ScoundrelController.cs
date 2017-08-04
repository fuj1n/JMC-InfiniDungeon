public class ScoundrelController : PlayerControllerBase
{
    protected override void FillSpells()
    {
        // TODO: a ranged dot?

        spells.Add(new SpellAttack(1, 0F, .2F, range: 5F).SetDamage(5F).SetPrimaryStat(PlayerData.Stats.Stat.DEXTERITY).SetIcon("Weapon_03"));
        spells.Add(new SpellAttack(5, 1F, 0F).SetDamage(10F).SetPrimaryStat(PlayerData.Stats.Stat.DEXTERITY).SetIcon("Weapon_11"));
        spells.Add(new SpellAttack(10, 3F, 7F).SetDamage(20F).SetPrimaryStat(PlayerData.Stats.Stat.DEXTERITY).SetIcon("Weapon_05"));
        spells.Add(new SpellHeal(7, 2.5F, 30F).SetHealingPotential(.5F).SetIcon("Variation B/Weapon_12"));
    }
}
