using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string hitboxTag; // breakable objects with a matching tag are friendly
    [SerializeField] private float damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float stunTime; // amount of time to pause target for

    #region PROPERTIES

    public float Damage
    {
        get { return damage; }
    }

    public string HitboxTag
    {
        get { return hitboxTag; }
    }

    public float KnockbackForce
    {
        get { return this.knockbackForce; }
    }

    public float StunTime
    {
        get { return this.stunTime; }
    }
    #endregion
}
