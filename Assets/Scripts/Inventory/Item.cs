using System;

public class Item
{
    public static readonly Item[] ITEMS = new Item[256];

    public static readonly Item CUSTOM_EQUIPPABLE = new ItemCustomEquippable(0).SetUnlocalizedName("custom");

    private string unlocalizedName = "undefined";
    private int maxStackSize = 64;

    public readonly int itemId;

    private string icon;

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

    public virtual string GetTooltip(ItemStack stack)
    {
        return "<b>" + stack.item.GetNameString(stack) + "</b>";
    }

    public Item SetIcon(string icon)
    {
        this.icon = icon;

        return this;
    }

    public virtual string GetIcon(ItemStack stack)
    {
        return icon;
    }

    public virtual void OnItemStackConstructed(ItemStack stack) { }

    public virtual bool IsEqual(ItemStack l, ItemStack r)
    {
        return l.item == r.item && l.data == r.data;
    }
}
