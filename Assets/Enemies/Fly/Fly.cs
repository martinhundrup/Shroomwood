using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy))]
public class Fly : MonoBehaviour
{
    // The speed at which the enemy moves
    [SerializeField] protected float speed;

    [SerializeField] protected Vector2 dir;

    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

    // Gets the speed attribute.
    public float Speed
    {
        get { return this.speed; }
    }

    // Ran when the object is created.
    private void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        //rigidBody.velocity = dir * speed;
    }

    // Called every physics update frame.
    private void FixedUpdate()
    {
        rigidBody.velocity = dir * speed;

    }

    // Called when a trigger collides with this object.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if we collided with a valid hitbox
        var _hitbox = collision.GetComponent<Hitbox>();
        if (_hitbox && !CompareTag(_hitbox.Tag))
        {
            StartCoroutine(HitStun(_hitbox.StunTime));
        }
    }

    // Called when a collider contacts this object.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Wall")) return;

        Debug.Log(collision.contacts[0].point.x);

        float _x = collision.contacts[0].point.x - this.transform.position.x;
        float _y = collision.contacts[0].point.y - this.transform.position.y;

        if (Mathf.Abs(_x) > Mathf.Abs(_y)) // change x dir
        {
            dir = dir * new Vector3(-1f, 1f, 0f);
        }
        else // change y dir
        {
            dir = dir * new Vector3(1f, -1f, 0f);
        }

        rigidBody.velocity = dir * speed;
    }

    // Temporarily pauses the enemy when hit.
    private IEnumerator HitStun(float _time)
    {
        var s = this.speed;
        this.speed = 0;

        yield return new WaitForSeconds(_time);

        this.speed = s;
        rigidBody.velocity = dir * speed;
    }
}
