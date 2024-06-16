using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponModifiers
{
    private float damageModifier;
            
    private float attackDurationModifier;

    private float attackCooldownModifier;

    public float DamageModifier
    {
        get { return this.damageModifier; }
    }

    public float AttackDurationModifier
    {
        get { return this.attackDurationModifier; }
    }

    public float AttackCooldownModifier
    {
        get { return this.attackCooldownModifier; }
    }

    public WeaponModifiers()
    {
        this.attackCooldownModifier = 1f;
        this.damageModifier = 1f;
        this.attackDurationModifier = 1f;
    }

    public WeaponModifiers(int _seed)
    {
        this.attackCooldownModifier = UnityEngine.Random.value;
        this.damageModifier = UnityEngine.Random.value;
        this.attackDurationModifier = UnityEngine.Random.value;
    }

    // deep copy constructor
    public WeaponModifiers(WeaponModifiers _other)
    {
        this.damageModifier = _other.damageModifier;
        this.attackDurationModifier = _other.attackDurationModifier;
        this.attackCooldownModifier = _other.attackCooldownModifier;
    }
}
