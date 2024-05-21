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

    // Called when a trigger collides with this object.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if we collided with a valid hitbox
        var hitbox = collision.GetComponent<Hitbox>();
        if (hitbox && !CompareTag(hitbox.Tag))
        {
            StartCoroutine(HitStun(hitbox.StunTime));
        }
    }

    #endregion

    #region METHODS

    // Temporarily pauses the enemy when hit.
    private IEnumerator HitStun(float _time)
    {
        var s = this.speed;
        this.speed = 0;

        yield return new WaitForSeconds(_time);

        this.speed = s;
    }

    #endregion
}
