public class ItemCustomEquippable : ItemEquippable
{
    public ItemCustomEquippable(int id) : base(id, SlotType.ARMS)
    { }

    public override SlotType GetSlotType(ItemStack stack)
    {
        if (stack.data is Data)
            return ((Data)stack.data).slotType;

        return base.GetSlotType(stack);
    }

    public override PlayerData.Stats GetStatChanges(ItemStack stack)
    {
        if (stack.data is Data)
            return ((Data)stack.data).statChanges;

        return base.GetStatChanges(stack);
    }

    public override string GetNameString(ItemStack i)
    {
        if (i.data is Data)
            return ((Data)i.data).name;

        return base.GetNameString(i);
    }

    public override string GetIcon(ItemStack stack)
    {
        if (stack.data is Data)
            return ((Data)stack.data).icon;

        return base.GetIcon(stack);
    }

    public class Data
    {
        public string name;
        public string icon;
        public SlotType slotType;
        public PlayerData.Stats statChanges;
    }
}
