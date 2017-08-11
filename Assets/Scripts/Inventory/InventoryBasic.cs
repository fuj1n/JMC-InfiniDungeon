﻿public class InventoryBasic : IInventory
{
    private ItemStack[] inventory;
    private string name;

    public InventoryBasic(string name, int size)
    {
        this.name = name;
        inventory = new ItemStack[size];
    }

    public int GetSize()
    {
        return inventory.Length;
    }

    public ItemStack GetStackInSlot(int slot)
    {
        if (!TestBounds(slot))
            return null;

        ItemStack i = inventory[slot];

        if (i == null)
            return null;

        return i;
    }

    public bool CanTakeStack(int slot)
    {
        if (!TestBounds(slot))
            return false;

        return inventory[slot] != null;
    }

    public ItemStack TakeStack(int slot)
    {
        if (!TestBounds(slot))
            return null;

        if (!CanTakeStack(slot))
            return null;

        ItemStack i = inventory[slot];

        if (i == null)
            return null;

        inventory[slot] = null;
        return i;
    }

    public void SetStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot))
            return;

        inventory[slot] = i;
    }

    public bool PlaceStack(ItemStack i)
    {
        for (int slot = 0; slot < GetSize(); slot++)
            if (PlaceStack(i, slot))
                return true;

        return false;
    }

    public bool PlaceStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot) || i == null)
            return false;

        if (inventory[slot] == null)
        {
            inventory[slot] = i;
            return true;
        }

        if (inventory[slot].IsEqual(i) && inventory[slot].amount + i.amount <= inventory[slot].GetMaxStackSize())
        {
            inventory[slot].amount += i.amount;
            return true;
        }

        return false;
    }

    public bool TestBounds(int slot)
    {
        return slot >= 0 && slot < GetSize();
    }

    public string GetName()
    {
        return name;
    }
}
