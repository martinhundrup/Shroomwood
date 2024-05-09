using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    #region ATRIBUTES

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
}