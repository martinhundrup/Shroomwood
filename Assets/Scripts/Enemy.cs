using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region EVENTS

    // called when the enemy dies.
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath;

    #endregion



    #region ATTRIBUTES

    // The speed at which the enemy moves
    [SerializeField] protected float speed;

    // The hitbox to spawn when overlapping with the player.
    [SerializeField] private GameObject hitbox;

    // Ref to the active spawned hitbox.
    private GameObject spawnedHitbox;

    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

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
    protected void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        spawnedHitbox = Instantiate(this.hitbox);
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


    #endregion

    #region METHODS

    // Temporarily pauses the enemy when hit.
    private IEnumerator HitStun(float _time)
    {
        var s = this.speed;
        this.speed = 0;

        yield return new WaitForSeconds(_time);

        this.speed = s;
    }

    #endregion
}
