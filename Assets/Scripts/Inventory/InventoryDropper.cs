using System;
using UnityEngine;

public class InventoryDropper : InventoryBasic
{
    private System.Random random = new System.Random();

    public InventoryDropper() : base("dropper", 1)
    {
    }

    public override void SetStack(ItemStack i, int slot)
    {
        PlayerControllerBase pc = PlayerControllerBase.GetActiveInstance();

        if (!pc)
            return;

        GameObject drop = new GameObject("drop");
        drop.AddComponent<EntityItem>().stack = i;
        drop.transform.position = pc.transform.position + Vector3.up * .5F;
        drop.transform.localEulerAngles = new Vector3(0F, random.Next(-90, 90), 0);
    }

    public override bool PlaceStack(ItemStack i)
    {
        SetStack(i, 0);

        return true;
    }
}
