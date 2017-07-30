using System;
using UnityEngine;

public class EntityEnemy : EntityLiving, IComparable<EntityEnemy>
{
    public static EntityEnemy selectedEnemy;

    private static int test = 0;

    [Header("Generator")]
    public int weight;

    [Header("Entity")]
    public string name;

    private float id;

    private TextMesh statusText;

    private void Awake()
    {
        id = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
    }

    private void Start()
    {
        if (test == 0)
        {
            test++;
            return;
        }
        GameObject go = new GameObject("text");
        go.transform.SetParent(transform, false);
        go.transform.Translate(0F, 1F, 0F);
        statusText = go.AddComponent<TextMesh>();
        statusText.anchor = TextAnchor.MiddleCenter;
        statusText.text = "Health: ";
        statusText.characterSize = .25F;
    }

    private void Update()
    {
        statusText.text = "Health: " + life + (selectedEnemy == this ? "\nSelected" : "");
    }

    public int CompareTo(EntityEnemy other)
    {
        return other == null || other.id > id ? 1 : other.id < id ? -1 : 0;
    }

    public override string GetName()
    {
        return "entity." + name + ".name";
    }

    private void OnMouseDown()
    {
        selectedEnemy = this;
    }
}
