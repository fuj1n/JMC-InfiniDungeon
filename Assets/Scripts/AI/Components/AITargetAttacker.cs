using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AITargetInfo))]
public class AITargetAttacker : AIComponent
{
    public float damage = 25F;

    public float range = 0.5F;
    public float cooldown = 2F;

    public float speed = .5F;
    public float stepTime = 0F;

    public Vector3 forwardOffset = Vector3.zero;

    private AITargetInfo targetInfo;

    private Guid movingAction;
    private Guid attackingAction;

    private float currentCooldown = .0F;

    public override void ControllerReady()
    {
        base.ControllerReady();

        targetInfo = (AITargetInfo)controller.GetAIComponent(AITargetInfo.ID);
    }

    private void Update()
    {
        if (stopped)
            return;

        if (!targetInfo.target || attackingAction != Guid.Empty)
        {
            currentCooldown = .0F;
            return;
        }

        transform.LookAt(new Vector3(targetInfo.target.transform.position.x, transform.position.y, targetInfo.target.transform.position.z));

        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            return;
        }

        if (movingAction != Guid.Empty)
            return;

        if (Vector3.Distance(transform.position, targetInfo.target.transform.position) > range)
        {
            movingAction = controller.AddAction(new ActionMoveTowardsTarget(range, forwardOffset, targetInfo.target.transform, speed, stepTime));
        }
        else
        {
            attackingAction = controller.AddAction(new ActionCastAttack(targetInfo.target, 0F, damage, cooldown));
        }
    }

    public override void ActionComplete(Guid id)
    {
        base.ActionComplete(id);

        if (id == movingAction)
            movingAction = Guid.Empty;
        else if (id == attackingAction)
            attackingAction = Guid.Empty;
    }

    public override string GetId()
    {
        return "component.attack";
    }
}
