using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData
{
    public const int BASE_XP = 32;

    public static PlayerData Instance { get; set; }

    public readonly string name;

    [JsonIgnore]
    public IInventory inventory = new InventoryBasic(60);

    public readonly PlayerClass playerClass;
    public Stats rawStats;

    [JsonIgnore]
    public int UsedStatPoints
    {
        get
        {
            return rawStats.Sum();
        }
    }

    [JsonIgnore]
    public int UnusedStatPoints
    {
        get
        {
            return level - 1 - UsedStatPoints;
        }
    }

    public int level = 1;
    public int experience = 0;

    [JsonIgnore]
    public int ExperienceToNextLevel
    {
        get
        {
            return BASE_XP * (int)Mathf.Pow(level, level);
        }
    }

    [JsonProperty]
    private Dictionary<int, string> slotsData;

    public struct Stats
    {
        public float strength;
        public float dexterity;
        public float vitality;
        public float intelligence;

        [JsonIgnore]
        public float[] StatValues
        {
            get
            {
                return new float[] { strength, dexterity, vitality, intelligence };
            }
        }

        public static Stats operator +(Stats lhs, Stats rhs)
        {
            return new Stats() { strength = lhs.strength + rhs.strength, dexterity = lhs.dexterity + rhs.dexterity, vitality = lhs.vitality + rhs.vitality, intelligence = lhs.intelligence + rhs.intelligence };
        }

        public static Stats operator -(Stats lhs, Stats rhs)
        {
            return new Stats() { strength = lhs.strength - rhs.strength, dexterity = lhs.dexterity - rhs.dexterity, vitality = lhs.vitality - rhs.vitality, intelligence = lhs.intelligence - rhs.intelligence };
        }

        public int Sum()
        {
            return Mathf.RoundToInt(strength + dexterity + vitality + intelligence);
        }

        public static readonly string[] STAT_NAMES = { "str", "dex", "vit", "int" };
        public static readonly string[] STAT_COLORS = { FormatCodes.CYAN, FormatCodes.YELLOW, FormatCodes.RED, FormatCodes.MAGENTA };
    }

    public enum PlayerClass
    {
        WARRIOR = 0,
        MAGE = 1,
        SCOUNDREL = 2
    }

    public static readonly string[] CLASS_COLORS = { FormatCodes.CYAN, FormatCodes.MAGENTA, FormatCodes.YELLOW };

    public static readonly Dictionary<PlayerClass, Stats> baseStats = new Dictionary<PlayerClass, Stats>()
    {
        { PlayerClass.WARRIOR, new Stats(){ strength = 5, dexterity = 1, vitality = 10, intelligence = 4 } },
        { PlayerClass.MAGE, new Stats(){ strength = 1, dexterity = 4, vitality = 2, intelligence = 10 } },
        { PlayerClass.SCOUNDREL, new Stats(){ strength = 3, dexterity = 10, vitality = 3, intelligence = 5 } }
    };

    public static readonly Dictionary<PlayerClass, string> prefabs = new Dictionary<PlayerClass, string>()
    {
        { PlayerClass.WARRIOR, "Player/PlayerWarrior" },
        { PlayerClass.MAGE, "Player/PlayerMage" },
        { PlayerClass.SCOUNDREL, "Player/PlayerScoundrel" }
    };

    public PlayerData(string name, PlayerClass playerClass)
    {
        this.name = name;
        this.playerClass = playerClass;
    }

    public Stats CalculateStats()
    {
        return rawStats + baseStats[playerClass]; // TODO stats based on inv
    }

    public void SaveData(string location)
    {
        slotsData = new Dictionary<int, string>();

        for (int i = 0; i < inventory.GetSize(); i++)
            if (inventory.GetStackInSlot(i) != null)
                slotsData[i] = inventory.GetStackInSlot(i).ToJSON();

        File.WriteAllText(location, JsonConvert.SerializeObject(this));

        slotsData = null;
    }

    public static PlayerData ReadData(string location)
    {
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(location));

        if (data.slotsData != null)
            foreach (KeyValuePair<int, string> kvp in data.slotsData)
                data.inventory.PlaceStack(ItemStack.FromJSON(kvp.Value), kvp.Key);

        data.slotsData = null;

        return data;
    }
}
