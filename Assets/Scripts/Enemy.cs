using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region ATTRIBUTES

    // The speed at which the enemy moves
    [SerializeField] private float speed;

    // Ref to the this object's rigidbody 2D component.
    private Rigidbody2D rigidBody;

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
    private void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hitbox hitbox = collision.GetComponent<Hitbox>();

        if (hitbox != null)
        {
            StartCoroutine(Knockback(hitbox.KnockbackForce, hitbox.StunTime, collision.transform.position));
        }
    }

    #endregion

    #region METHODS
    
    private IEnumerator Knockback(float _force, float _stunTime, Vector2 _other)
    {
        this.rigidBody.AddForce(_force / 100 * ((Vector2)this.transform.position - _other).normalized);

        yield return new WaitForSeconds(_stunTime / 10);

        this.rigidBody.velocity = Vector2.zero;
    }

    #endregion
}
