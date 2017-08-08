using System;
using UnityEngine;

[RequireComponent(typeof(AIController))]
public abstract class AIComponent : MonoBehaviour
{
    [NonSerialized]
    public bool stopped = true;

    protected AIController controller;

    private void Awake()
    {
        controller = GetComponent<AIController>();
    }

    public virtual void ControllerReady() { }
    public virtual void ActionComplete(Guid id) { }

    public abstract string GetId();
}
