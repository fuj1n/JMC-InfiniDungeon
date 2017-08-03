using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControllerBase : EntityLiving
{
    public static readonly string[] keyLabels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };
    public static readonly KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals };

    private static PlayerControllerBase activeInstance;

    protected List<SpellBase> spells = new List<SpellBase>();
    private Dictionary<SpellBase, float> cooldowns = new Dictionary<SpellBase, float>();

    private SpellBase casting;
    private float progress;

    public float Progress
    {
        get
        {
            if (casting != null)
                return progress / casting.castTime;
            else
                return -1F;
        }
    }

    [HideInInspector]
    public PlayerData playerData = PlayerData.Instance;

    public SpellBase[] Spells
    {
        get
        {
            return spells.ToArray();
        }
    }

    protected void Awake()
    {
        activeInstance = this;

        maxLife = playerData.GetMaxLife();

        FillSpells();
    }

    protected override void Update()
    {
        base.Update();

        foreach (KeyValuePair<SpellBase, float> kvp in cooldowns)
        {
            if (kvp.Value > .0F)
                cooldowns[kvp.Key] = kvp.Value - Time.deltaTime;
        }

        if (casting != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                casting = null;
                return;
            }

            if (progress >= casting.castTime)
            {
                if (casting.VerifyCanCastSpell(this))
                {
                    cooldowns[casting] = casting.cooldown;
                    casting.Cast(this);
                }
                casting = null;
            }
            else
            {
                progress += Time.deltaTime;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TargetTracker.target = null;
            return;
        }

        for (int i = 0; i < spells.Count && i < spellKeys.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]))
            {
                if (cooldowns.ContainsKey(spells[i]) && cooldowns[spells[i]] > 0)
                {
                    UIHelper.Alert("alerts.player.spell.cooldown", "InGameError");
                    continue;
                }
                if (!spells[i].VerifyCanCastSpell(this))
                    continue;
                progress = 0;
                casting = spells[i];
                return;
            }
        }
    }

    protected abstract void FillSpells();

    public override bool IsTooltipVisible()
    {
        return false;
    }

    public override string GetName()
    {
        return playerData.name;
    }

    public static PlayerControllerBase GetActiveInstance()
    {
        return activeInstance;
    }

    public bool IsInCooldown(SpellBase spell)
    {
        return cooldowns.ContainsKey(spell) && cooldowns[spell] > .0F;
    }
}
