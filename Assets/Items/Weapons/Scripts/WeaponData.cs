using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ItemData
{
    // The name of the weapon effect
    [SerializeField] private string effectName;

    // The amount of damage the weapon deals.
    [SerializeField] private float damage;

    // The time between attacks.
    [SerializeField] private float cooldown;

    // The time the hitbox is active.
    [SerializeField] private float duration;

    // The force applied to enemies that get knocked back.
    [SerializeField] private float knockback;

    // The amount of time enemies are prevented from moving.
    [SerializeField] private float stunTime;

    // The size modifier of the hitbox.
    [SerializeField] private float sizeModifier;

    // The hitbox of the weapon.
    [SerializeField] private GameObject hitbox;

    [SerializeField] private float hitboxDistanceOffset;

  

    // Gets the effect name.
    public string EffectName
    {
        get { return this.effectName; }
    }

    // Gets the damage value.
    public float Damage
    {
        get { return this.damage; }
    }

    // Gets the cooldown value.
    public float Cooldown
    {
        get { return this.cooldown; } 
    }

    // Gets the attack duration value.
    public float Duration
    {
        get { return this.duration; }
    }

    // Gets the knockback force.
    public float Knockback
    {
        get { return this.knockback; }
    }

    public float StunTime
    {
        get { return this.stunTime; }
    }

    // Gets the hitbox object.
    public GameObject Hitbox
    {
        get { return this.hitbox; } 
    }

    // Gets the size modifier.
    public float SizeModifier
    {
        get { return this.sizeModifier; }
    }

    public float HitBoxDistanceOffset
    {
        get { return this.hitboxDistanceOffset; }
    }
}
