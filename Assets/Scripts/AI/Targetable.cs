using System;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public static readonly List<Targetable> cache = new List<Targetable>();

    public delegate void OnDamaged(Targetable damageSource);
    public event OnDamaged onDamaged;

    [NonSerialized]
    public TargetableFaction faction = TargetableFaction.NEUTRAL;

    private void OnEnable()
    {
        cache.Add(this);
    }

    private void OnDisable()
    {
        cache.Remove(this);
    }

    public virtual void Damage(Targetable source, float damage)
    {
        if (source && onDamaged != null)
            onDamaged(source);
    }

    public enum TargetableFaction
    {
        NEUTRAL,
        PLAYER,
        ENEMY
    }
}
