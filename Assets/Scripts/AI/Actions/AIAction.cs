using System;
using System.Collections;
using UnityEngine;

public abstract class AIAction
{
    public Guid id = Guid.Empty;

    private AIController controller;
    protected GameObject owner;

    public void SetController(AIController controller)
    {
        this.controller = controller;
        owner = controller.gameObject;
    }

    public virtual void OnStart() { }
    public virtual void OnDequeued() { }

    public abstract bool IsDone();

    public virtual void OnUpdate() { }
    public virtual void OnPhysicsUpdate() { }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return controller.StartCoroutine(enumerator);
    }

    public void Sleep(float seconds)
    {
        Sleep(() =>
        {
            return (seconds -= Time.deltaTime) <= 0;
        });
    }

    public void Sleep(Func<bool> condition)
    {
        controller.AddAwait(condition);
    }
}
