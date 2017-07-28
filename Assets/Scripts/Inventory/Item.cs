using System;

public class Item
{
    public static readonly Item[] ITEMS = new Item[256];

    private string unlocalizedName = "undefined";
    private int maxStackSize = 64;

    public readonly int itemId;

    public Item(int id)
    {
        if (id < 0 || id >= ITEMS.Length)
            throw new ArgumentException("Item ID " + id + " is out of range of the items array, the largest possible ID is " + ITEMS.Length);
        if (ITEMS[id] != null)
            throw new ArgumentException("Item ID " + id + " already taken by " + ITEMS[id] + " when trying to add " + this + ".");

        itemId = id;
        ITEMS[id] = this;
    }

    public int GetMaxStackSize()
    {
        return maxStackSize;
    }

    public virtual int GetMaxStackSize(ItemStack i)
    {
        return GetMaxStackSize();
    }

    public Item SetMaxStackSize(int maxStackSize)
    {
        this.maxStackSize = maxStackSize;
        return this;
    }

    public virtual string GetNameString(ItemStack i)
    {
        return "item." + GetUnlocalizedName(i) + ".name";
    }

    public virtual string GetUnlocalizedName(ItemStack i)
    {
        return unlocalizedName;
    }

    public Item SetUnlocalizedName(string name)
    {
        unlocalizedName = name;
        return this;
    }
}
