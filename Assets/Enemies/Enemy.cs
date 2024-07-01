using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : Breakable
{
    protected PlayerController player;
    protected Rigidbody2D rb;
    

    [SerializeField] protected float speed;
    [SerializeField] protected bool hasHitstun = false;
    [SerializeField] protected float contactDamage; // damage dealt to player on contact
    protected bool isInHitstun = false;
    protected bool isInvulnerable = false;
    protected Breakable breakable;
    protected SpriteRenderer sr;

    public float ContactDamage
    {
        get { return contactDamage; }
    }

    new protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        player = FindObjectOfType<PlayerController>();
        breakable = GetComponent<Breakable>();

        base.Awake();
    }

    protected override void TakeDamage(Hitbox _hitbox)
    {
        if (!isInHitstun && !isInvulnerable) // extra check for enemies
        {
            StartCoroutine(HitStun(_hitbox));
            StartCoroutine(MakeInvulnerable());
            base.TakeDamage(_hitbox);
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

    // makes enemy unable to take more damage when hit
    protected IEnumerator MakeInvulnerable()
    {
        isInvulnerable = true;
        GetComponent<Blink>().StartBlinking();

        yield return new WaitForSeconds(0.4f);

        GetComponent<Blink>().StopBlinking();
        isInvulnerable = false;
    }
}
