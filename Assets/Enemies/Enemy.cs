using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    protected PlayerController player;
    protected Rigidbody2D rb;
    

    [SerializeField] protected float speed;
    [SerializeField] protected bool hasHitstun = false;
    protected bool isInHitstun = false;

    protected SpriteRenderer sr;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        player = FindObjectOfType<PlayerController>();

        
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // check if we collided with a valid hitbox
        var _hitbox = collision.GetComponent<Hitbox>();
        if (_hitbox && !CompareTag(_hitbox.HitboxTag))
        {
            Debug.Log("enemy hit");
            StartCoroutine(HitStun(_hitbox));
        }
    }


    // Temporarily pauses the enemy when hit.
    protected IEnumerator HitStun(Hitbox _hitbox)
    {
        this.isInHitstun = true;
        var s = this.speed;
        this.speed = 0;
        this.rb.velocity = Vector2.zero;

        yield return new WaitForFixedUpdate(); // add delay for knockback force

        this.rb.AddForce(_hitbox.KnockbackForce * 50 * (this.transform.position - player.transform.position).normalized);

        yield return new WaitForSeconds(_hitbox.StunTime);

        this.speed = s;
        this.isInHitstun = false;
    }
}
