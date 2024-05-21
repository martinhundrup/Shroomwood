using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    #region ATTRIBUTES

    // The damage this object deals.
    [SerializeField] private int damage;

    // The force of which objects are pushed back.
    [SerializeField] private float knockbackForce;

    // The amount of time objects are pushed back for (tenths of seconds).
    [SerializeField] private float stunTime;

    // Tags of objects to avoid collision.
    [SerializeField] private string _tag;

    #endregion

    #region PROPERTIES

    // Gets or sets the damage this object deals.
    public int Damage
    {
        get { return this.damage; }
        set { this.damage = value; }
    }

    // Gets the knockback force.
    public float KnockbackForce
    {
        get { return this.knockbackForce; }
    }

    // Gets the stun time.
    public float StunTime
    {
        get { return this.stunTime; }
    }

    // Gets or sets the _tag value.
    public string Tag
    {
        get { return _tag; }
        set { this._tag = value; }
    }

    #endregion

    #region METHODS

    // Starts the destroy timer.
    public void StartTimer(float _time)
    {
        StartCoroutine(Decay(_time));
    }

    // Waits an amount of time then destroys this game object.
    private IEnumerator Decay(float _time)
    {
        yield return new WaitForSeconds(_time);

        Destroy(this.gameObject);
    }

    #endregion
}