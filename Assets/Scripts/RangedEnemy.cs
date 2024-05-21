using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    // Reference to player.
    private PlayerController player;

    // The projectile to shoot.
    [SerializeField] private GameObject projectile;

    // The distance at which the enemy can detect the player.
    [SerializeField] private float sightDistance;

    // The rate at which the enemy can fire.
    [SerializeField] private float fireRate;

    // The enemy layer to ignore hitting other enemies.
    [SerializeField] private LayerMask[] layerMasks;

    // The layer masks to be used by raycasts.
    private int masks = 0;

    // Is the enemy currently locked onto the player?
    bool isShooting = false;

    // Called when the object is created.
    new private void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>();

        // init masks
        for (int i = 0; i < layerMasks.Length; i++)
        {
            masks = masks | layerMasks[i].value;
        }
    }

    // Called once a frame.
    private void Update()
    {
        FindTarget();
    }

    // Determines what the enemies next target is.
    private void FindTarget()
    {

        if (!this.isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    // Checks if the enemy has LOS to the player.
    private Vector2 CheckLOS()
    {
        Vector2 dir = player.transform.position - this.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, sightDistance, ~masks);

        return (hit && hit.collider.CompareTag("Player")) ? hit.transform.position : Vector2.zero;
    }

    private IEnumerator Shoot()
    {
        this.isShooting = true;
        var hit = CheckLOS();

        if(hit != Vector2.zero)
        {
            GameObject _p = Instantiate(projectile);
            _p.transform.position = this.transform.position;
            _p.GetComponent<Projectile>().Fire((Vector3) hit - this.transform.position);
        }

        yield return new WaitForSeconds(fireRate);

        this.isShooting = false;
    }
}
