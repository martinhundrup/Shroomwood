using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ItemData
{
    // The amount of damage the weapon deals.
    [SerializeField] private float damage;

    // The time between attacks.
    [SerializeField] private float cooldown;

    // The time the hitbox is active.
    [SerializeField] private float duration;

    // The hitbox of the weapon.
    [SerializeField] private GameObject hitbox;

    [SerializeField] AnimatorController weaponAnimator;
    [SerializeField] AnimatorController effectAnimator;

    public AnimatorController WeaponAnimator
    {
        get { return this.weaponAnimator; }
    }

    public AnimatorController EffectAnimator
    {
        get { return this.effectAnimator; }
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

    // Gets the hitbox object.
    public GameObject Hitbox
    {
        get { return this.hitbox; } 
    }
}
