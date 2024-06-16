using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Fly : MonoBehaviour
{
    // The starting direction.
    [SerializeField] protected Vector2 dir;

    // Reference to this objects enemy component.
    private Enemy enemy;

    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

    // Ran when the object is created.
    private void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.enemy = GetComponent<Enemy>();
    }

    //private void OnEnable()
    //{
    //    rigidBody.AddForce(dir * enemy.Speed / 100);
    //}

    //Called every physics update frame.
    private void FixedUpdate()
    {
        if (!this.enemy.IsInHitstun)
            rigidBody.velocity = dir * enemy.Speed;
    }

    //Called when a collider contacts this object.
    private void OnCollisionEnter2D(Collision2D collision)
    {
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

        rigidBody.velocity = dir * this.enemy.Speed;
    }
}
