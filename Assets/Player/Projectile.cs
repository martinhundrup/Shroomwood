using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region ATTRIBUTES

    // Ref to the projectile's rigidbody component.
    private Rigidbody2D rigidBody;

    // The speed at which the projecile moves.
    [SerializeField] private float speed;

    // The tag attached to the owner of the shot.
    [SerializeField] string _tag;

    #endregion

    #region PROPERTIES

    // Gets or sets the speed value.
    public float Speed
    {
        get { return this.speed; }
        set { this.speed = value; }
    }

    // Gets or sets the _tag value.
    public string Tag
    {
        get { return _tag; }
        set {  _tag = value; }
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
        if (!collision.CompareTag(_tag) && !collision.CompareTag("ItemDrop") && !collision.GetComponent<Hitbox>())
        {
            Destroy(this.gameObject);
        }
    }

    #endregion

    #region METHODS

    // Starts the movement of the object.
    public void Fire(Vector2 _dir)
    {
        rigidBody.velocity = (_dir.normalized * speed);
    }

    #endregion
}
