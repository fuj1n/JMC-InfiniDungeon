using UnityEngine;

public class EntityItem : Entity
{
    public ItemStack stack;
    private Sprite sprite;

    private SpriteRenderer render;

    public override string GetName()
    {
        return stack.item.GetNameString(stack);
    }

    private void Start()
    {
        if (stack == null)
            return;

        sprite = Resources.Load<Sprite>("Icons/" + stack.item.GetIcon(stack));

        render = gameObject.AddComponent<SpriteRenderer>();
        render.sprite = sprite;

        transform.localEulerAngles = new Vector3(90F, transform.localEulerAngles.y, 0F);
        transform.localScale = new Vector3(.2F, .2F, .2F);

        // Move object down to ground
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 5F))
            transform.Translate(0F, -hit.distance + .1F, 0F, Space.World);

        gameObject.AddComponent<BoxCollider>().isTrigger = true;

        OnSpawn();

        if (tooltipText)
        {
            tooltipText.characterSize = 1F;
            tooltipText.transform.localPosition = new Vector3(0F, 0F, -5F);
        }
    }

    private void OnMouseDown()
    {
        PickUp();
    }

    private void PickUp()
    {
        PlayerControllerBase inst = PlayerControllerBase.GetActiveInstance();
        if (!inst || Vector3.Distance(inst.transform.position, transform.position) > 5F)
            return;

        PlayerData data = PlayerData.Instance;

        if (data != null)
        {
            if (data.inventory.PlaceStack(stack))
                Destroy(gameObject);
        }
    }
}
