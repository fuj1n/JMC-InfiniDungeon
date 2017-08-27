using Newtonsoft.Json;
using System;

public sealed class ItemStack
{
    public readonly Item item;
    public int amount;

    [JsonIgnore]
    public object data;

    public ItemStack(Item item, int amount = 1)
    {
        if (item == null)
            throw new ArgumentNullException("ItemStacks of null are not allowed");

        this.item = item;
        this.amount = amount;

        item.OnItemStackConstructed(this);
    }

    public int GetMaxStackSize()
    {
        return item.GetMaxStackSize(this);
    }

    public bool IsEqual(ItemStack stack)
    {
        return item.IsEqual(this, stack);
    }

    public string ToJSON()
    {
        SavedItemStack sis = new SavedItemStack();
        sis.itemid = item.itemId;
        sis.amount = amount;

        if (data != null)
        {
            sis.data = JsonConvert.SerializeObject(data);
            sis.dataType = data.GetType();
        }

        return JsonConvert.SerializeObject(sis);
    }

    public static ItemStack FromJSON(string json)
    {
        SavedItemStack sis = JsonConvert.DeserializeObject<SavedItemStack>(json);
        ItemStack i = new ItemStack(Item.ITEMS[sis.itemid], sis.amount);

        if (sis.dataType != null)
            i.data = JsonConvert.DeserializeObject(sis.data, sis.dataType);

        return i;
    }

    private class SavedItemStack
    {
        public int itemid;
        public int amount;

        public string data;
        public Type dataType;
    }
}
