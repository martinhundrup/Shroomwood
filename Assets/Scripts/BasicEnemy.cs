using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    // Reference to player.
    private PlayerController player;

    // The distance at which the enemy can detect the player.
    [SerializeField] private float sightDistance;

    // The enemy layer to ignore hitting other enemies.
    [SerializeField] private LayerMask layerMask;

    // Whether or not the enemy is currently seeking the player.
    private bool isAggro = false;

    // Called when the object is created.
    new private void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>();
    }

    // Called once a frame.
    private void Update()
    {
        FindPlayer();
        Movement();
    }

    // Moves the enemy in the appropriate manner.
    private void Movement()
    {
        if (isAggro)
        {
            this.rigidBody.velocity = (player.transform.position - this.transform.position).normalized * this.speed;
        }
        else
        {
            this.rigidBody.velocity = Vector2.zero;
        }
    }

    // Determines if the enemy can see the player.
    private void FindPlayer()
    {
        if (Vector2.Distance(this.transform.position, player.transform.position) < sightDistance)
        {
            Vector2 dir = player.transform.position - this.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, sightDistance, ~layerMask.value);

            if (hit && hit.collider.CompareTag("Player"))
            {
                isAggro = true;
            }
            else
            {
                isAggro = false;
            }

        }
        else
        {
            isAggro = false;
        }
    }
}
