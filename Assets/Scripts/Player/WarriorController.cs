public class WarriorController : PlayerControllerBase
{
    protected override void FillSpells()
    {
        spells.Add(new SpellAttack(1, 0F, .2F, range: 5F).SetDamage(7F).SetPrimaryStat(PlayerData.Stats.Stat.STRENGTH).SetIcon("Weapon_08"));
        spells.Add(new SpellAttack(3, 0F, 2F, range: 5F).SetDamage(15F).SetPrimaryStat(PlayerData.Stats.Stat.STRENGTH).SetIcon("Weapon_25"));
        spells.Add(new SpellAttack(10, 2F, 5F, range: 5F).SetDamage(30F).SetPrimaryStat(PlayerData.Stats.Stat.STRENGTH).SetIcon("Weapon_23"));
        spells.Add(new SpellHeal(5, 0F, 60F).SetHealingPotential(1F).SetIcon("Variation B/Weapon_08"));
    }
}
