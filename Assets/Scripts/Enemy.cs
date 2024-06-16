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

    // Ref to the this object's rigidbody 2D component.
    protected Rigidbody2D rigidBody;

    protected SpriteRenderer spriteRenderer;

    // The speed at which the enemy moves
    [SerializeField] protected float speed;

    [SerializeField] protected bool hasHitstun = false;

    [SerializeField] protected bool isInHitstun = false;

    // Gets the speed attribute.
    public float Speed
    {
        get { return this.speed; }
    }

    // Gets the has histstun attribute.
    public bool HasHitstun
    {
        get { return this.hasHitstun; }
    }

    // Gets the isInHitstun state
    public bool IsInHitstun
    {
        get { return this.isInHitstun;}
    }

    #endregion

    #region UNITY CALLBACKS

    // Called when object is loaded.
    protected void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (this.hitbox != null)
        {
            spawnedHitbox = Instantiate(this.hitbox);
            spawnedHitbox.transform.position = transform.position;
        }
    }

    // Called once a physics frame.
    protected void FixedUpdate()
    {
        if (this.spawnedHitbox != null)
        {
            spawnedHitbox.transform.position = transform.position;
        }
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

    // Called when a trigger collides with this object.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if we collided with a valid hitbox
        var _hitbox = collision.GetComponent<Hitbox>();
        if (_hitbox && !CompareTag(_hitbox.Tag))
        {
            StartCoroutine(HitStun(_hitbox, (Vector2)collision.gameObject.transform.position));
            StopCoroutine(Blink());
            StartCoroutine(Blink());
        }
    } 

    // Temporarily pauses the enemy when hit.
    private IEnumerator HitStun(Hitbox _hitbox, Vector2 _position_of_collision)
    {
        this.isInHitstun = true;
        var s = this.speed;
        this.speed = 0;
        this.rigidBody.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.01f); // add delay for knockback force

        Debug.Log("add knockback force");
        this.rigidBody.AddForce(_hitbox.KnockbackForce / 10 * ((Vector2)this.transform.position - _position_of_collision).normalized);

        yield return new WaitForSeconds(_hitbox.StunTime);

        this.speed = s;
        this.isInHitstun = false;
    }

    // Blinks the player's prite renderer until they are no longer invulnerable.
    private IEnumerator Blink()
    {
        int _count = 3;

        while (_count > 0)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            _count--;
        }

        spriteRenderer.enabled = true;
        yield return null;
    }
}
