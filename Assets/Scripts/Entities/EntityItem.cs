using UnityEngine;

public class EntityItem : Entity
{
    public ItemStack stack;
    private Sprite sprite;

    private new SpriteRenderer renderer;

    public override string GetName()
    {
        return stack.item.GetNameString(stack);
    }

    private void Awake()
    {
        stack = new ItemStack(Item.ITEM_TEST, 22);
        sprite = Resources.Load<Sprite>("Icons/" + stack.item.GetIcon(stack));

        renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        transform.Rotate(90F, 0F, 0F);
        transform.localScale = new Vector3(.2F, .2F, .2F);

        // Move object down to ground
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit))
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
        Destroy(gameObject);
    }
}
