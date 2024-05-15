using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region ATTRIBUTES

    // Ref to the projectile's rigidbody component.
    private Rigidbody2D rigidBody;

    // The direction the projectile should move in.
    private Vector2 dir = new Vector2(1f, 1f);

    // The speed at which the projecile moves.
    [SerializeField] private float speed;

    #endregion

    #region PROPERTIES

    // Gets or sets the dir value.
    public Vector2 Dir
    {
        get { return this.dir; }
        set { this.dir = value.normalized; }
    }

    // Gets or sets the speed value.
    public float Speed
    {
        get { return this.speed; }
        set { this.speed = value; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Called when object created.
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Called when a trigger collision occurs.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // destroy this if it collides with anything other than the player
        if (!collision.CompareTag("Player") && !collision.CompareTag("ItemDrop"))
        {
            Destroy(this.gameObject);
        }
    }

    #endregion

    #region METHODS

    // Starts the movement of the object.
    public void Fire()
    {
        rigidBody.velocity = (dir.normalized * speed);
    }

    #endregion
}
