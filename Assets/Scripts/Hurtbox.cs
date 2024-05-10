using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ** DEPRECATED ** //

public class Hurtbox : MonoBehaviour
{
    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void HurtAction(int damage);

    // The event called when this object collides with a hitbox.
    public event HurtAction OnHurt;

    #endregion

    #region UNITY CALLBACKS

    // The method called when another object with a 2D collider overlaps with this object's collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // a hitbox has collided with this
        if (collision.gameObject.GetComponent<Hitbox>() != null)
        {
            OnHurt(collision.gameObject.GetComponent<Hitbox>().Damage);
        }
    }

    #endregion

    #region METHODS

    // The method called when another object with a 2D collider overlaps with this object's collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // a hitbox has collided with this
        if (collision.gameObject.GetComponent<Hitbox>() != null)
        {
            OnHurt(collision.gameObject.GetComponent<Hitbox>().Damage);
        }
    }

    #endregion
}