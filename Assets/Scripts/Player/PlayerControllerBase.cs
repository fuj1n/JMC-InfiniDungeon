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

    [NonSerialized]
    public GameObject castEffect;

    private static PlayerControllerBase activeInstance;

    protected List<SpellBase> spells = new List<SpellBase>();
    private Dictionary<SpellBase, float> cooldowns = new Dictionary<SpellBase, float>();

    private SpellBase casting;
    private float progress;

    private float regenSpeed = 2F;
    private float regenPotency = 0.015F;

    private List<EntityLiving> enemyTabCache;

    private float farthestSpell = 0F;
    private Func<GameObject, bool> reasonableTargetFunc;

    private Puppeteer anim;

    private Transform castingParticle;

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

    protected virtual void Awake()
    {
        anim = transform.GetComponent<Puppeteer>();

        faction = TargetableFaction.PLAYER;

        reasonableTargetFunc = e => Vector3.Distance(e.transform.position, transform.position) < farthestSpell && Vector3.Angle(e.transform.position - transform.position, transform.forward) <= 90;

        if (playerData == null)
            return;

        activeInstance = this;

        FillSpells();
        spells.ForEach(b =>
        {
            if (b.range > farthestSpell)
                farthestSpell = b.range;
        });

        StartCoroutine(DoRegeneration());

        castingParticle = transform.FindRecursively("CastingParticle");
    }

    protected override void Start()
    {
        base.Start();

        PauseController.canPauseFunc = () =>
        {
            InterfaceController ic = InterfaceController.GetInstance();
            if (ic && (ic.IsInterfaceOpen(InterfaceController.Side.LEFT) || ic.IsInterfaceOpen(InterfaceController.Side.RIGHT)))
                return false;

            if (casting != null)
                return false;

            if (TargetTracker.target != null)
                return false;

            return true;
        };
    }

    protected override void Update()
    {
        base.Update();

        maxLife = playerData.GetMaxLife();

        if (playerData.name.StartsWith("#") && Input.GetKeyDown(KeyCode.L))
            playerData.GrantExperience(playerData.ExperienceToNextLevel - playerData.Experience);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!TryAndSwitchTarget())
            {
                enemyTabCache = (from e in GameObject.FindGameObjectsWithTag("Enemy")
                                 where reasonableTargetFunc(e) && e.GetComponent<EntityLiving>()
                                 orderby Vector3.Distance(e.transform.position, transform.position) ascending
                                 select e.GetComponent<EntityLiving>()).ToList();

                TryAndSwitchTarget();
            }
        }

        SpellBase[] keys = cooldowns.Keys.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            SpellBase key = keys[i];

            if (cooldowns[key] > .0F)
                cooldowns[key] -= Time.deltaTime;
        }

        if (casting != null)
        {
            if (progress >= casting.castTime)
            {
                if (casting.VerifyCanCastSpell(this))
                {
                    anim.Pause(false);

                    if (!string.IsNullOrEmpty(casting.animation) && !casting.animateBeforeCast && anim.IsPlaying())
                        return;

                    cooldowns[casting] = casting.cooldown;
                    casting.Cast(this);
                }

                anim.Stop();

                if (castingParticle)
                {
                    castEffect = null;

                    foreach (Transform t in castingParticle)
                        Destroy(t.gameObject);
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

                if (!string.IsNullOrEmpty(casting.animation))
                {
                    anim.Play(Resources.Load<TextAsset>("Poses/" + casting.animation));
                    if (!casting.animateBeforeCast)
                        anim.Pause(true);
                }

                if (castingParticle && !string.IsNullOrEmpty(casting.particle))
                    castEffect = Instantiate(Resources.Load<GameObject>("Particles/" + casting.particle), castingParticle);

                return;
            }
        }
    }

    protected virtual void LateUpdate()
    {
        // TODO: This works, so make cast animations based off it
        //if (casting != null)
        //transform.FindRecursively("hips").Translate(0F, .25F, 0F);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InterfaceController ic = InterfaceController.GetInstance();

            if (ic && (ic.IsInterfaceOpen(InterfaceController.Side.LEFT) || ic.IsInterfaceOpen(InterfaceController.Side.RIGHT)))
            {
                ic.CloseInterface(InterfaceController.Side.LEFT);
                ic.CloseInterface(InterfaceController.Side.RIGHT);
            }
            else if (casting != null)
            {
                casting = null;
                return;
            }
            else
            {
                TargetTracker.target = null;
                return;
            }
        }
    }

    protected virtual IEnumerator DoRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenSpeed);
            Heal(maxLife * regenPotency);
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

    public override bool OnKill()
    {
        PlayerData.Instance.SaveData();

        SceneManager.LoadScene("Scenes/MainMenu"); // TODO death screen?

        return false;
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

    private bool TryAndSwitchTarget()
    {
        while (enemyTabCache != null && enemyTabCache.Count > 0)
        {
            bool madeSwitch = false;
            if (enemyTabCache[0] && reasonableTargetFunc(enemyTabCache[0].gameObject))
            {
                TargetTracker.target = enemyTabCache[0];
                madeSwitch = true;
            }

            enemyTabCache.RemoveAt(0);

            if (madeSwitch)
                return true;
        }

        return false;
    }
}
