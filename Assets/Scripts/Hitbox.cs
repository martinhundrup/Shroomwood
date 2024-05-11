using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    #region ATTRIBUTES

    // The damage this object deals.
    [SerializeField] private int damage;

    #endregion

    #region PROPERTIES

    // Gets or sets the damage this object deals.
    public int Damage
    {
        get { return this.damage; }
        set { this.damage = value; }
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