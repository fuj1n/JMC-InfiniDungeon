using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour
{
    public static readonly KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals };

    protected List<SpellBase> spells = new List<SpellBase>();
    private Dictionary<SpellBase, float> cooldowns = new Dictionary<SpellBase, float>();

    private SpellBase casting;
    private float progress;

    [HideInInspector]
    public PlayerData playerData = PlayerData.Instance;

    public SpellBase[] Spells
    {
        get
        {
            return spells.ToArray();
        }
    }

    void Start()
    {
        FillSpells();
    }

    void Update()
    {
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
}
