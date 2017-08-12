using System;

public class InventoryArmor : InventoryBasic {
    public InventoryArmor() : base("inventory.armor", Enum.GetValues(typeof(ItemEquippable.SlotType)).Length)
    {
    }

    public override bool CanPlaceStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot))
            return false;

        return i.item is ItemEquippable && slot == (int)((ItemEquippable)i.item).GetSlotType(i);
    }
}
