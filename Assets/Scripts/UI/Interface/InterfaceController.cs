using UnityEngine;

public class InterfaceController : MonoBehaviour
{
    private static InterfaceController instance;

    private Interface leftOpen;
    private Interface rightOpen;

    private Slot cursorSlot;

    private RectTransform leftInterface;
    private RectTransform rightInterface;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        leftInterface = transform.Find("left").GetComponent<RectTransform>();
        rightInterface = transform.Find("right").GetComponent<RectTransform>();

        cursorSlot = transform.Find("cursorSlot").GetComponent<Slot>();
        cursorSlot.SetProperties(new InventoryBasic("cursor.inventory", 1), 0, 0, 0);

        UpdateState();
    }

    public Slot GetCursorSlot()
    {
        return cursorSlot;
    }

    public void OpenInterface(Interface i)
    {
        switch (i.side)
        {
            case Side.LEFT:
                OpenInterface(i, leftOpen, leftInterface, out leftOpen);
                break;
            case Side.RIGHT:
                OpenInterface(i, rightOpen, rightInterface, out rightOpen);
                break;
        }

        UpdateState();
    }

    private void OpenInterface(Interface i, Interface current, RectTransform side, out Interface output)
    {
        bool closeOnly = (current && current.id == i.id);

        CloseInterface(i.side);

        if (closeOnly)
        {
            output = null;
            return;
        }

        output = Instantiate(i, side);
        output.Open();

        UpdateState();
    }

    public void CloseInterface(Side side)
    {
        switch (side)
        {
            case Side.LEFT:
                CloseInterface(leftOpen, out leftOpen);
                break;
            case Side.RIGHT:
                CloseInterface(rightOpen, out rightOpen);
                break;
        }

        UpdateState();
    }

    private void CloseInterface(Interface face, out Interface outFace)
    {
        if (face)
        {
            face.Close();
            Destroy(face);
        }

        outFace = null;
    }

    public bool IsInterfaceOpen(Side side)
    {
        return (side == Side.LEFT && leftOpen) || (side == Side.RIGHT && rightOpen);
    }

    private void UpdateState()
    {
        leftInterface.gameObject.SetActive(leftOpen);
        rightInterface.gameObject.SetActive(rightOpen);
    }

    public static InterfaceController GetInstance()
    {
        return instance;
    }

    public enum Side
    {
        LEFT,
        RIGHT
    }
}
