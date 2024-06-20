using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] protected float jumpInterval; // the base time between jumps.
    [SerializeField] protected float intervalMargin; // the +/- time the interval is randomized to.
    [SerializeField, Range(0f, 1f)] protected float speedDecay;
    private bool isJumping;


    private void OnEnable()
    {
        StartCoroutine(JumpTimer());
    }

    // Called every physics frame.
    private void FixedUpdate()
    {
        if (!isInHitstun)
        {
            rb.velocity = rb.velocity * speedDecay;
        }
    }

    // Continuously jump at semi random intervals.
    private IEnumerator JumpTimer()
    {
        while (true)
        {
            var dir = FindDirection();

            yield return new WaitForSeconds(jumpInterval + Random.Range(-intervalMargin, intervalMargin));

            rb.AddForce(dir * speed * 10);
        }
    }

    // chooses a random direction to move in, 50% to target player
    private Vector2 FindDirection()
    {
        if (Random.Range(0,2) == 0)
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        else
            return (player.transform.position - transform.position).normalized;
    }
}
