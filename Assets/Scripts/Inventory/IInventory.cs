public interface IInventory
{
    int GetSize();
    ItemStack GetStackInSlot(int slot);
    bool CanTakeStack(int slot);
    bool CanPlaceStack(ItemStack i, int slot);
    ItemStack TakeStack(int slot);
    void SetStack(ItemStack i, int slot);
    bool PlaceStack(ItemStack i);
    bool PlaceStack(ItemStack i, int slot);
    string GetName();
}
