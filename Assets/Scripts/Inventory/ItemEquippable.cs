public class ItemEquippable : Item
{
    private SlotType slot;
    private PlayerData.Stats statChanges;

    public ItemEquippable(int id, SlotType slot, PlayerData.Stats statChanges = default(PlayerData.Stats)) : base(id)
    {
        this.slot = slot;
        this.statChanges = statChanges;
    }

    public virtual SlotType GetSlotType(ItemStack stack)
    {
        return slot;
    }

    public virtual PlayerData.Stats GetStatChanges(ItemStack stack)
    {
        return statChanges;
    }

    public override string GetTooltip(ItemStack stack)
    {
        string tooltip = base.GetTooltip(stack);

        PlayerData.Stats statChanges = GetStatChanges(stack);

        if (statChanges.Sum() > 0F)
        {
            tooltip += "\n\n" + FormatCodes.GOLD + "Stat Changes:" + FormatCodes.COL_E;

            if (statChanges.vitality > 0F)
                tooltip += "\nplayer.stats.vit.name: " + statChanges.vitality.ToString("F1");
            if (statChanges.dexterity > 0F)
                tooltip += "\nplayer.stats.dex.name: " + statChanges.dexterity.ToString("F1");
            if (statChanges.intelligence > 0F)
                tooltip += "\nplayer.stats.int.name: " + statChanges.intelligence.ToString("F1");
            if (statChanges.strength > 0F)
                tooltip += "\nplayer.stats.str.name: " + statChanges.strength.ToString("F1");
        }

        return tooltip.Trim();
    }

    public enum SlotType
    {
        HEAD = 0,
        CHEST = 1,
        ARMS = 2,
        LEGS = 3,
        NECK = 4,
        FINGER = 5,
        HAND = 6
    }
}
