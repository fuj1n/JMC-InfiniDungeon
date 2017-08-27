using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropGenerator
{
    private static Dictionary<string, string[]> icons;
    private static string[] names;

    private static System.Random rand;

    public static ItemStack Generate()
    {
        LoadTables();

        ItemStack stack = new ItemStack(Item.CUSTOM_EQUIPPABLE, 1);

        ItemCustomEquippable.Data data = new ItemCustomEquippable.Data();
        stack.data = data;

        data.slotType = rand.NextFrom(Enum.GetValues(typeof(ItemEquippable.SlotType)) as ItemEquippable.SlotType[]);
        data.icon = "Items/" + rand.NextFrom(icons[data.slotType.ToString()]);

        PlayerData pd = PlayerData.Instance;
        int level = pd != null ? pd.level : 1;

        data.statChanges.intelligence = rand.Next(0, Mathf.RoundToInt(2 * (level * .2F + 1)));
        data.statChanges.strength = rand.Next(0, Mathf.RoundToInt(2 * (level * .2F + 1)));
        data.statChanges.dexterity = rand.Next(0, Mathf.RoundToInt(2 * (level * .2F + 1)));
        data.statChanges.vitality = rand.Next(0, Mathf.RoundToInt(2 * (level * .15F + 1)));

        data.name = "item.type." + data.slotType.ToString() + " " + rand.NextFrom(names);

        return stack;
    }

    private static void LoadTables()
    {
        if (rand == null)
            rand = new System.Random();
        if (icons == null)
            icons = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(Resources.Load<TextAsset>("Icons/Items/rng").text);
        if (names == null)
            names = Resources.Load<TextAsset>("Icons/Items/names").text.Split('\n').Select(r => r.Trim()).ToArray();
    }
}
