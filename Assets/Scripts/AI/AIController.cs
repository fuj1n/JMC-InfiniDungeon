using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AIController : MonoBehaviour
{
    public float minimumPlayerProximity = 50F;

    private Queue<AIAction> actions = new Queue<AIAction>();
    private Queue<Func<bool>> awaits = new Queue<Func<bool>>();
    private Dictionary<string, AIComponent> components = new Dictionary<string, AIComponent>();

    private bool inRangeToPlayer = false;

    private void Start()
    {
        foreach (AIComponent component in GetComponents<AIComponent>())
            if (components.ContainsKey(component.GetId()))
                throw new ArgumentException("Duplicate ID. ID of '" + component.GetType().Name + "' matches ID of '" + components[component.GetId()].GetType().Name + "' on " + name);
            else
                components[component.GetId()] = component;

        foreach (AIComponent component in components.Values)
            component.ControllerReady();
    }

    public AIComponent GetAIComponent(string id)
    {
        if (components.ContainsKey(id))
            return components[id];

        return null;
    }

    public void AddAwait(Func<bool> func)
    {
        awaits.Enqueue(func);
    }

    public Guid AddAction(AIAction action)
    {
        if (action.id != Guid.Empty)
            throw new ArgumentException("The action provided to AddAction has already been used elsewhere.");

        action.id = Guid.NewGuid();
        action.SetController(this);

        actions.Enqueue(action);
        if (actions.Count == 1)
            action.OnStart();

        return action.id;
    }

    public void DiscardAction()
    {
        if (actions.Count > 0)
        {
            AIAction action = actions.Dequeue();
            action.OnDequeued();
            foreach (AIComponent component in components.Values)
                component.ActionComplete(action.id);

            awaits.Clear();

            if (actions.Count > 0)
                actions.Peek().OnStart();
        }
    }

    public Guid PeekAction()
    {
        if (actions.Count > 0)
            return actions.Peek().id;
        else
            return Guid.Empty;
    }

    private void Update()
    {
        if (!inRangeToPlayer)
            return;

        while (awaits.Count > 0)
            if (awaits.Peek()())
            {
                awaits.Dequeue();
            }
            else
                return;

        //if (awaits.Count > 0)
        //    return;

        if (actions.Count > 0)
        {
            AIAction currentAction = actions.Peek();

            if (currentAction.IsDone())
            {
                DiscardAction();

                return;
            }

            currentAction.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        bool foundInRange = (from p in GameObject.FindGameObjectsWithTag("Player")
                             where Vector3.Distance(transform.position, p.transform.position) <= minimumPlayerProximity
                             select p).Any();

        if (foundInRange != inRangeToPlayer)
        {
            inRangeToPlayer = foundInRange;

            foreach (AIComponent component in components.Values)
                component.stopped = !inRangeToPlayer;
        }
    }

    private void FixedUpdate()
    {
        if (awaits.Count > 0)
            return;

        if (actions.Count > 0)
            actions.Peek().OnPhysicsUpdate();
    }
}
