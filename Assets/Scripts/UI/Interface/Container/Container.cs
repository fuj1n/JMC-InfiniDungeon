using System.Collections.Generic;
using UnityEngine;

public abstract class Container
{
    protected Interface parent;

    protected RectTransform slotsParent;
    protected List<Slot> slots = new List<Slot>();

    public abstract void InitializeSlots();

    protected void CreateSlot(IInventory inv, int id, int x, int y, string label = "")
    {
        GameObject go = new GameObject("slot " + inv.GetName() + " " + id);
        go.AddComponent<RectTransform>().SetParent(slotsParent);

        Slot slot = go.AddComponent<Slot>();

        slot.SetProperties(inv, id, x, y);
        slot.label = label;
        slots.Add(slot);
    }

    public virtual void Open(Interface parent)
    {
        this.parent = parent;

        slotsParent = new GameObject("Slots").AddComponent<RectTransform>();
        slotsParent.SetParent(parent.transform, false);

        slotsParent.anchorMin = new Vector2(0, 0);
        slotsParent.anchorMax = new Vector2(1, 1);
        slotsParent.localScale = new Vector3(1, 1, 1);

        slotsParent.sizeDelta = new Vector2(0, 0);
        slotsParent.anchoredPosition = new Vector2(0, 0);


        InitializeSlots();
    }
    public virtual void Close() { }
}
