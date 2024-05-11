using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    #region ATTRIBUTES

    // The value to multiply the movement vector by; the speed of the player.
    [SerializeField] private float movementSpeed;

    #endregion

    #region PROPERTIES
    
    // Gets or sets the movement speed.
    public float MovementSpeed
    {
        get { return this.movementSpeed; }
        set { this.movementSpeed = value; }
    }

    #endregion
}
