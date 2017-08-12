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
