public class InventoryBasic : IInventory
{
    private ItemStack[] inventory;
    private string name;

    public InventoryBasic(string name, int size)
    {
        this.name = name;
        inventory = new ItemStack[size];
    }

    public virtual int GetSize()
    {
        return inventory.Length;
    }

    public virtual ItemStack GetStackInSlot(int slot)
    {
        if (!TestBounds(slot))
            return null;

        ItemStack i = inventory[slot];

        if (i == null)
            return null;

        return i;
    }

    public virtual bool CanTakeStack(int slot)
    {
        if (!TestBounds(slot))
            return false;

        return inventory[slot] != null;
    }

    public virtual bool CanPlaceStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot))
            return false;

        return true;
    }

    public virtual ItemStack TakeStack(int slot)
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

    public virtual void SetStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot))
            return;

        inventory[slot] = i;
    }

    public virtual bool PlaceStack(ItemStack i)
    {
        for (int slot = 0; slot < GetSize(); slot++)
            if (PlaceStack(i, slot))
                return true;

        return false;
    }

    public virtual bool PlaceStack(ItemStack i, int slot)
    {
        if (!TestBounds(slot) || i == null)
            return false;

        if (!CanPlaceStack(i, slot))
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

    public virtual bool TestBounds(int slot)
    {
        return slot >= 0 && slot < GetSize();
    }

    public virtual string GetName()
    {
        return name;
    }
}
