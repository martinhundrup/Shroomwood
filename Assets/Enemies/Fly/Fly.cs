using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : Enemy
{
    [SerializeField] protected float jumpInterval; // the base time between jumps.
    [SerializeField] protected float intervalMargin; // the +/- time the interval is randomized to.
    [SerializeField] protected float moveTime; // the time the fly is moving for

    private void OnEnable()
    {
        StartCoroutine(JumpTimer());
    }

    // Continuously jump at semi random intervals.
    private IEnumerator JumpTimer()
    {
        while (true)
        {
            var dir = FindDirection();

            yield return new WaitForSeconds(jumpInterval + Random.Range(-intervalMargin, intervalMargin));
            if (dir.x < 0) sr.flipX = true;
            else sr.flipX = false;
            rb.velocity = (dir * speed);
            StartCoroutine(StopMovementTimer());
        }
    }

    private IEnumerator StopMovementTimer()
    {
        yield return new WaitForSeconds(moveTime);
        rb.velocity = Vector3.zero;
    }

    // chooses a random direction to move in, 50% to target player
    private Vector2 FindDirection()
    {
        if (Random.Range(0, 2) == 0)
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        else
            return (player.transform.position - transform.position).normalized;
    }
}
