using UnityEngine;

public class SpellAttack : SpellBase
{
    public float damage = 5F;
    public float flySpeed = float.PositiveInfinity;

    public PlayerData.Stats.Stat primaryStat;

    public SpellAttack(int levelRequirement, float castTime, float cooldown, bool requiresLineOfSight = true, TargetType spellTarget = TargetType.SELECTED, float range = 25) : base(levelRequirement, castTime, cooldown, requiresLineOfSight, spellTarget, range)
    { }

    public SpellAttack SetDamage(float damage)
    {
        this.damage = damage;

        return this;
    }

    public SpellAttack SetPrimaryStat(PlayerData.Stats.Stat primaryStat)
    {
        this.primaryStat = primaryStat;

        return this;
    }

    public SpellAttack SetFlySpeed(float speed)
    {
        flySpeed = speed;

        return this;
    }

    public override void Cast(PlayerControllerBase controller)
    {
        float damage = this.damage * controller.playerData.GetDamageMultiplierForStat(primaryStat);
        if (float.IsPositiveInfinity(flySpeed))
        {
            TargetTracker.target.Damage(controller, damage);
            return;
        }

        GameObject effect = controller.castEffect;
        if (!effect)
        {
            effect = new GameObject("Projectile");
            effect.transform.position = controller.transform.position;
            effect.AddComponent<BoxCollider>().isTrigger = true;
        }

        effect.transform.SetParent(null, true);
        foreach (MonoBehaviour mb in effect.GetComponents<MonoBehaviour>())
            Object.Destroy(mb);

        Projectile proj = effect.AddComponent<Projectile>();
        proj.damage = damage;
        proj.speed = flySpeed;
        proj.source = controller;
        proj.target = TargetTracker.target;
    }
}
