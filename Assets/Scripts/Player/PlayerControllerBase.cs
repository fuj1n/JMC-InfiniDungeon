using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PlayerControllerBase : EntityLiving
{
    public static readonly string[] keyLabels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };
    public static readonly KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals };

    private static PlayerControllerBase activeInstance;

    protected List<SpellBase> spells = new List<SpellBase>();
    private Dictionary<SpellBase, float> cooldowns = new Dictionary<SpellBase, float>();

    private SpellBase casting;
    private float progress;

    private float regenSpeed = 1.3F;
    private float regenPotency = 0.05F;

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

    [NonSerialized]
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
        type = TargetableType.PLAYER;

        if (playerData == null)
            return;

        activeInstance = this;

        maxLife = playerData.GetMaxLife();

        FillSpells();

        StartCoroutine(DoRegeneration());
    }

    protected override void Update()
    {
        base.Update();

        if (playerData.name.StartsWith("#") && Input.GetKeyDown(KeyCode.L))
            playerData.GrantExperience(playerData.ExperienceToNextLevel - playerData.Experience);

        SpellBase[] keys = cooldowns.Keys.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            SpellBase key = keys[i];

            if (cooldowns[key] > .0F)
                cooldowns[key] -= Time.deltaTime;
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

    protected virtual IEnumerator DoRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenSpeed);
            Heal(playerData.GetMaxLife() * regenPotency);
        }
    }

    protected abstract void FillSpells();

    public void Heal(float health)
    {
        life = Mathf.Clamp(life + health, 0F, playerData.GetMaxLife());
    }

    public override bool IsTooltipVisible()
    {
        return false;
    }

    public override string GetName()
    {
        return playerData.name;
    }

    public override bool OnKill()
    {
        SceneManager.LoadScene("Scenes/MainMenu"); // TODO death screen

        return true;
    }

    public static PlayerControllerBase GetActiveInstance()
    {
        return activeInstance;
    }

    public bool IsInCooldown(SpellBase spell)
    {
        return cooldowns.ContainsKey(spell) && cooldowns[spell] > .0F;
    }

    public float GetCooldown(SpellBase spell)
    {
        return IsInCooldown(spell) ? cooldowns[spell] : -1F;
    }
}
