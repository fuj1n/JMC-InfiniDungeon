﻿using UnityEngine;

public class DamageScaler : MonoBehaviour
{
    private float baseDamage;
    private float baseLife;
    private AITargetAttacker attacker;
    private EntityLiving living;

    private void Awake()
    {
        attacker = GetComponent<AITargetAttacker>();
        living = GetComponent<EntityLiving>();
        baseDamage = attacker.damage;
        baseLife = living.maxLife;
    }

    private void Update()
    {
        if (PlayerData.Instance == null)
            return;

        attacker.damage = baseDamage * (PlayerData.Instance.level * .1F + 1);
        living.maxLife = baseLife * (PlayerData.Instance.level * .7F + 1);
    }
}
