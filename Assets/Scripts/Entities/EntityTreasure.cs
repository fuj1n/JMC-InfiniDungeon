using UnityEngine;

public class EntityTreasure : Entity {
    public int min = 1;
    public int max = 3;

    public override string GetName()
    {
        return "entity.treasure.name";
    }

    private void OnMouseDown()
    {
        int drops = random.Next(min, max + 1);

        for(int i = 0; i < drops; i++)
        {
            GameObject itemDrop = new GameObject("itemDrop");
            itemDrop.transform.position = transform.position + Vector3.up * .5F + Vector3.forward * (random.Next(-5, 5) * .05F) + Vector3.left * (random.Next(-5, 5) * .05F);
            itemDrop.transform.localEulerAngles = new Vector3(0F, random.Next(-90, 90), 0);

            EntityItem item = itemDrop.AddComponent<EntityItem>();
            item.stack = DropGenerator.Generate();
        }

        Destroy(gameObject);
    }
}
