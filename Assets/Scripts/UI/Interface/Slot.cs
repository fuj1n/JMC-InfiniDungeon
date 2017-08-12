using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const int SLOT_SIZE = 64;

    public bool isDummy = false;

    [Header("Slot")]
    [ConditionalHide("isDummy", true, true)]
    public Color color = new Color(1F, 1F, 1F, .5F);
    [ConditionalHide("isDummy", true, true)]
    public Color hoverColor = new Color(1F, 1F, 1F, .75F);

    [Header("Item")]
    [ConditionalHide("isDummy", true, true)]
    public Color multiplier = new Color(1F, 1F, 1F, 1F);
    [ConditionalHide("isDummy", true, true)]
    public Color hoverMultiplier = new Color(1F, 1F, 1F, .75F);


    private IInventory inv;
    private int slot;
    private int x, y;

    private Image background;
    private Image foreground;

    private Text count;

    private Slot cursorSlot;

    private bool isDirty = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDummy)
            return;


        ItemStack invStack = GetSlotContents();
        ItemStack cursorStack = cursorSlot.GetSlotContents();

        if (!inv.CanTakeStack(slot) || (cursorStack != null && !inv.CanPlaceStack(cursorStack, slot)))
            return;

        cursorSlot.SetSlotContents(invStack);
        SetSlotContents(cursorStack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDummy)
            return;

        background.color = hoverColor;
        foreground.color = hoverMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDummy)
            return;

        background.color = color;
        foreground.color = multiplier;
    }

    public void SetProperties(IInventory inv, int slot, int x, int y)
    {
        this.inv = inv;
        this.slot = slot;
        this.x = x;
        this.y = y;
    }

    public bool SetSlotContents(ItemStack stack)
    {
        isDirty = true;

        if (stack == null && inv.CanTakeStack(slot))
            return inv.TakeStack(slot) != null;
        else if (inv.CanPlaceStack(stack, slot))
            return inv.PlaceStack(stack, slot);

        return false;
    }

    public ItemStack GetSlotContents()
    {
        return inv.GetStackInSlot(slot);
    }

    private void Awake()
    {
        cursorSlot = InterfaceController.GetInstance().GetCursorSlot();

        RectTransform rect = (transform as RectTransform);

        rect.localScale = new Vector3(1, 1, 1);

        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = rect.anchorMin;

        rect.sizeDelta = new Vector2(SLOT_SIZE, SLOT_SIZE);

        RectTransform fg = new GameObject("item").AddComponent<RectTransform>();
        fg.SetParent(transform, false);

        fg.localScale = new Vector3(1, 1, 1);

        fg.anchorMin = new Vector2(0, 0);
        fg.anchorMax = new Vector2(1, 1);

        fg.anchoredPosition = new Vector2(0, 0);
        fg.sizeDelta = new Vector2(0, 0);

        foreground = fg.gameObject.AddComponent<Image>();
        foreground.raycastTarget = false;

        GameObject numberBackground = new GameObject("numberBackground");
        numberBackground.transform.SetParent(fg, false);

        Image numberBg = numberBackground.AddComponent<Image>();
        numberBg.color = new Color(0F, 0F, 0F, .75F);
        numberBg.raycastTarget = false;

        RectTransform bgRect = numberBackground.GetComponent<RectTransform>();

        bgRect.anchorMin = new Vector2(0F, 0F);
        bgRect.anchorMax = bgRect.anchorMin;
        bgRect.pivot = new Vector2(0F, 0F);
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30F);
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SLOT_SIZE);

        GameObject num = new GameObject("number");
        count = num.AddComponent<Text>();
        count.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        count.fontSize = 36;
        count.alignment = TextAnchor.MiddleCenter;
        count.transform.SetParent(numberBackground.transform, false);
        count.raycastTarget = false;

        RefreshSlot();

        if (isDummy)
            return;

        rect.anchoredPosition = new Vector2(x, y);

        background = gameObject.AddComponent<Image>();
        background.color = color;
    }

    private void Update()
    {
        if (isDirty)
            RefreshSlot();
    }

    private void RefreshSlot()
    {
        isDirty = false;

        ItemStack ist = GetSlotContents();
        foreground.gameObject.SetActive(ist != null);

        if (ist == null)
            return;

        foreground.sprite = Resources.Load<Sprite>("Icons/" + ist.item.GetIcon(ist));
        count.text = ist.amount.ToString();
    }
}
