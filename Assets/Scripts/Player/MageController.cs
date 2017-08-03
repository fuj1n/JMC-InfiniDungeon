public class MageController : PlayerControllerBase {
    protected override void FillSpells()
    {
        spells.Add(new SpellAttack(1, 2F, 0F).SetDamage(10F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE));
        spells.Add(new SpellAttack(5, 0F, 2F).SetDamage(5F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE));
        spells.Add(new SpellAttack(10, 5F, 0F).SetDamage(30F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE));
        //spells.Add(new SpellAttack(1, 2F, 0F).SetDamage(25F).SetPrimaryStat(PlayerData.Stats.Stat.INTELLIGENCE));
    }
}
