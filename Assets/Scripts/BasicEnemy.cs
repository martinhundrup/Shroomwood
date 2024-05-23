using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicEnemy : Enemy
{
    // Reference to player.
    private PlayerController player;

    // The distance at which the enemy can detect the player.
    [SerializeField] private float sightDistance;

    // The time the enemy waits when idle.
    [SerializeField] private float idleTime;

    // The distance the enemy can wonder while idle
    [SerializeField] private float idleDistance;

    // The enemy layer to ignore hitting other enemies.
    [SerializeField] private LayerMask[] layerMasks;

    // The layer masks to be used by raycasts.
    private int masks = 0;

    // Whether or not the enemy is currently seeking the player.
    private bool isAggro = false;

    // The current position to move towards.
    private Vector2 targetPos;


    private bool isIdling = false;

    // Called when the object is created.
    new private void Awake()
    {
        base.Awake();
        targetPos = transform.position;
        player = FindObjectOfType<PlayerController>();

        // init masks
        for (int i = 0; i < layerMasks.Length; i++)
        {
            masks = masks | layerMasks[i].value;
        }
    }

    // Called once a frame.
    new private void Update()
    {
        base.Update();
        FindTarget();
        Movement();
    }

    // Moves the enemy in the appropriate manner.
    private void Movement()
    {
        // if the enemy reaches it's target, it is no longer aggro'd and stops moving
        if (Vector2.Distance(this.transform.position, this.targetPos) < 0.1)
        {
            // ensure no crashes when overlapping with player
            if (Vector2.Distance(player.transform.position, this.targetPos) < 0.6)
                return;

            isAggro = false;
            this.rigidBody.velocity = Vector2.zero;
            
            if (!isIdling)
            {
                StartCoroutine(IdleMovement());
            }
        }
        else
        {
            this.rigidBody.velocity = ((Vector3)targetPos - this.transform.position).normalized * this.speed;
        }
    }

    // Determines what the enemies next target is.
    private void FindTarget()
    {
        // Sees or saw the player
        if (isAggro || Vector2.Distance(this.transform.position, player.transform.position) < sightDistance)
        {
            // if the enemy has LOS, aggro onto player if not already
            if (CheckLOS())
            {
                this.StopCoroutine(IdleMovement());
                this.isAggro = true;
                isIdling = false;
                this.targetPos = this.player.transform.position;
            }
        }
    }

    // Checks if the enemy has LOS to the player.
    private bool CheckLOS()
    {
        Vector2 dir = player.transform.position - this.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, sightDistance, ~masks);

        return hit && hit.collider.CompareTag("Player");
    }

    private IEnumerator IdleMovement()
    {
        this.isIdling = true;
        Vector2 r_dir = new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f).normalized;

        // check if direction is valid
        while (Physics2D.Raycast(this.transform.position, r_dir, idleDistance, ~masks))
        {
            r_dir = new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f).normalized;
        }
                

        // update target
        this.targetPos = this.transform.position + (Vector3)r_dir * idleDistance;

        yield return new WaitForSeconds(idleTime); // wait while enemy moves

        this.targetPos = this.transform.position;

        yield return new WaitForSeconds(idleTime); // wait while enemy stops

        this.isIdling = false;
    }
}
