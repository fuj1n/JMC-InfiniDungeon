using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AITargetInfo))]
public class AITargetFinder : AIComponent
{
    public bool targetOnProvoke = true;
    public bool provokeOverridesTarget = true;

    public float range = 15F;
    public float loseRange = 25F;

    public float scanInterval = 2F;
    public Targetable.TargetableFaction faction = Targetable.TargetableFaction.PLAYER;

    private AITargetInfo targetInfo;

    public override void ControllerReady()
    {
        base.ControllerReady();

        targetInfo = (AITargetInfo)controller.GetAIComponent(AITargetInfo.ID);

        if (targetOnProvoke)
        {
            Targetable comp = GetComponent<Targetable>();
            if (comp)
                comp.onDamaged += (Targetable provoker) =>
                {
                    if (provokeOverridesTarget || !targetInfo.target)
                        targetInfo.target = provoker;
                };
        }

        InvokeRepeating("TargetFinder", scanInterval, scanInterval);
    }

    private void TargetFinder()
    {
        if (stopped)
            return;

        if (!targetInfo.target)
        {
            Targetable newTarget = (from t in Targetable.cache
                                    where t.gameObject != gameObject && t.faction == faction && Vector3.Distance(transform.position, t.transform.position) <= range
                                    select t).OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();

            if (newTarget)
                targetInfo.target = newTarget;
        }
    }

    public override string GetId()
    {
        return "component.finder";
    }
}
