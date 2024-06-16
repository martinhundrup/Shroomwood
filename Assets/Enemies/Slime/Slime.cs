using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Slime : MonoBehaviour
{
    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

    [SerializeField] protected float jumpInterval; // the base time between jumps.
    [SerializeField] protected float intervalMargin; // the +/- time the interval is randomized to.
    [SerializeField, Range(0f, 1f)] protected float speedDecay;
    private bool isJumping;

    // Reference to this objects enemy component.
    private Enemy enemy;

    // Ran when the object is created.
    private void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        StartCoroutine(JumpTimer());
    }

    // Called every physics frame.
    private void FixedUpdate()
    {
        if (!this.enemy.IsInHitstun)
        {
            rigidBody.velocity = rigidBody.velocity * speedDecay;
        }
        else
        {
            Debug.Log("open for knockback");
        }
    }

    // Continuously jump at semi random intervals.
    private IEnumerator JumpTimer()
    {
        while (true)
        {            
            var dir = FindDirection();

            yield return new WaitForSeconds(jumpInterval + Random.Range(-intervalMargin, intervalMargin));

            rigidBody.AddForce(dir * enemy.Speed / 100);
        }
    }

    // chooses a random direction to move in
    private Vector2 FindDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
