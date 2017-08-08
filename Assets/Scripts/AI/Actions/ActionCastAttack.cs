using System.Collections;
using UnityEngine;

public class ActionCastAttack : AIAction
{
    private Targetable target;

    private float castTime;
    private float cooldown;

    private float damage;

    private bool isDone = false;

    public ActionCastAttack(Targetable target, float castTime, float damage, float cooldown = 0F)
    {
        this.target = target;

        this.castTime = castTime;
        this.cooldown = cooldown;
        this.damage = damage;
    }

    public override bool IsDone()
    {
        return isDone;
    }

    public override void OnStart()
    {
        base.OnStart();

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        if (castTime > .0F)
            yield return new WaitForSeconds(castTime);

        target.Damage(owner.GetComponent<Targetable>(), damage);

        if (cooldown > .0F)
            yield return new WaitForSeconds(cooldown);

        isDone = true;
    }
}
