using System;

public class ContainerPlayer : Container
{
    public int startX = 64;
    public int startY = 50;
    public int padding = 8;

    public override void InitializeSlots()
    {
        for (int y = 0; y < 6; y++)
            for (int x = 0; x < 10; x++)
                CreateSlot(PlayerData.Instance.inventory, y * 10 + x, startX + x * Slot.SLOT_SIZE + x * padding, startY + y * Slot.SLOT_SIZE + y * padding);
    }
}
