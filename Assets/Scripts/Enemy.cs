using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region ATTRIBUTES

    // The speed at which the enemy moves
    [SerializeField] protected float speed;

    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

    #endregion

    #region PROPERTIES

    // Gets the speed attribute.
    public float Speed
    {
        get { return this.speed; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Called when object is loaded.
    protected void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region METHODS
    

    #endregion
}
