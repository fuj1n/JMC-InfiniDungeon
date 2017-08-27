using System;

public class InventoryArmor : InventoryBasic
{
    public InventoryArmor() : base("inventory.armor", Enum.GetValues(typeof(ItemEquippable.SlotType)).Length)
    {
    }

    public override bool CanPlaceStack(ItemStack i, int slot)
    {
        return base.CanPlaceStack(i, slot) && i.item is ItemEquippable && slot == (int)((ItemEquippable)i.item).GetSlotType(i);
    }
}
