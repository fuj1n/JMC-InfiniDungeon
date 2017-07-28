public interface IInventory
{
    int GetSize();
    ItemStack GetStackInSlot(int slot);
    bool CanTakeStack(int slot);
    ItemStack TakeStack(int slot);
    bool PlaceStack(ItemStack i);
    bool PlaceStack(ItemStack i, int slot);
    string GetName();
}
