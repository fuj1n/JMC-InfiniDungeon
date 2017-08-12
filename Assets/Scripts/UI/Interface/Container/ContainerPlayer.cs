public class ContainerPlayer : Container
{
    public int startX = 64;
    public int startY = 50;
    public int padding = 8;

    public override void InitializeSlots()
    {
        IInventory armorInv = PlayerData.Instance.armor;

        CreateSlot(armorInv, 0, 347, 915); // Head
        CreateSlot(armorInv, 1, 384, 841); // Chest
        CreateSlot(armorInv, 2, 310, 841); // Arms
        CreateSlot(armorInv, 3, 384, 767); // Legs
        CreateSlot(armorInv, 4, 421, 915); // Neck
        CreateSlot(armorInv, 5, 458, 841); // Finger
        CreateSlot(armorInv, 6, 384, 619); // Hand

        for (int y = 0; y < 6; y++)
            for (int x = 0; x < 10; x++)
                CreateSlot(PlayerData.Instance.inventory, y * 10 + x, startX + x * Slot.SLOT_SIZE + x * padding, startY + y * Slot.SLOT_SIZE + y * padding);
    }
}
