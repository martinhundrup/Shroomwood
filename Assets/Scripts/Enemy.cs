using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    #region EVENTS

    // called when the enemy dies.
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath;

    #endregion

    #region ATTRIBUTES    

    // The hitbox to spawn when overlapping with the player.
    [SerializeField] private GameObject hitbox;

    // Ref to the active spawned hitbox.
    private GameObject spawnedHitbox;

    #endregion

    #region UNITY CALLBACKS

    // Called when object is loaded.
    protected void Awake()
    {
        spawnedHitbox = Instantiate(this.hitbox);
        spawnedHitbox.transform.position = transform.position;
    }

    // Called once a frame.
    protected void Update()
    {
        spawnedHitbox.transform.position = transform.position;
    }

    // Called when this object is destroyed.
    protected void OnDestroy()
    {
        if (this.OnDeath != null)
        {
            this.OnDeath();
        }

        Destroy(this.spawnedHitbox);
    }

    #endregion
}
