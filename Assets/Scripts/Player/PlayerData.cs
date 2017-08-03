using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerData
{
    public const string DATA_LOCATION = "Data/Players/";

    public const int BASE_XP = 32;

    public static PlayerData Instance { get; set; }

    public readonly string name;

    [JsonIgnore]
    public string Snowflake
    {
        get
        {
            return snowflake;
        }
    }
    private string snowflake = Guid.NewGuid().ToString("N");

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
    [JsonProperty]
    private int experience = 0;
    [JsonIgnore]
    public int Experience
    {
        get
        {
            return experience;
        }
    }

    [JsonIgnore]
    public int ExperienceToNextLevel
    {
        get
        {
            return BASE_XP * (int)Mathf.Pow(level, 2);
        }
    }

    [JsonIgnore]
    public string ClassName
    {
        get
        {
            return "player.class." + playerClass.ToString();
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

        public float GetStat(Stat s)
        {
            switch (s)
            {
                case Stat.STRENGTH:
                    return strength;
                case Stat.DEXTERITY:
                    return dexterity;
                case Stat.VITALITY:
                    return vitality;
                case Stat.INTELLIGENCE:
                    return intelligence;
            }

            return 0f;
        }

        public enum Stat
        {
            STRENGTH, DEXTERITY, VITALITY, INTELLIGENCE
        }
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

    public float GetDamageMultiplierForStat(Stats.Stat stat)
    {
        return 1F; // TODO damage calculation
    }

    public float GetMaxLife()
    {
        return 250 * (CalculateStats().vitality * 0.5F);
    }

    public void GrantExperience(int xp)
    {
        experience += xp;

        bool isLevelUp = false;

        while (experience >= ExperienceToNextLevel)
        {
            isLevelUp = true;

            experience -= ExperienceToNextLevel;
            level++;
        }

        if (isLevelUp)
            UIHelper.Alert("Level " + level, "LevelUp");
    }

    public void SaveData()
    {
        if (name.StartsWith("#"))
            return;

        if (!Directory.Exists(DATA_LOCATION))
            Directory.CreateDirectory(DATA_LOCATION);

        slotsData = new Dictionary<int, string>();

        for (int i = 0; i < inventory.GetSize(); i++)
            if (inventory.GetStackInSlot(i) != null)
                slotsData[i] = inventory.GetStackInSlot(i).ToJSON();

        File.WriteAllText(DATA_LOCATION + snowflake + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));

        slotsData = null;
    }

    public static PlayerData ReadData(string location)
    {
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(location));

        if (data.slotsData != null)
            foreach (KeyValuePair<int, string> kvp in data.slotsData)
                data.inventory.PlaceStack(ItemStack.FromJSON(kvp.Value), kvp.Key);

        data.snowflake = Path.GetFileNameWithoutExtension(location);

        data.slotsData = null;

        return data;
    }

    public void DeleteSave()
    {
        string file = DATA_LOCATION + snowflake + ".json";

        if (File.Exists(file))
            File.Delete(file);
    }

    public static PlayerData[] ReadAllData()
    {
        if (!Directory.Exists(DATA_LOCATION))
            return new PlayerData[0];

        List<PlayerData> pd = new List<PlayerData>();

        foreach (string f in Directory.GetFiles(DATA_LOCATION))
        {
            PlayerData data = ReadData(f);
            if (data != null)
                pd.Add(data);
        }

        return pd.OrderBy(d => d.name).ToArray();
    }
}
