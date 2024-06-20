using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    protected PlayerController player;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    [SerializeField] protected float speed;
    [SerializeField] protected bool hasHitstun = false;
    protected bool isInHitstun = false;

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
            StopCoroutine(Blink());
            StartCoroutine(Blink());
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

    // Blinks the sprite renderer until they are no longer invulnerable.
    protected IEnumerator Blink()
    {
        int _count = 3;

        while (_count > 0)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
            _count--;
        }

        sr.enabled = true;
        yield return null;
    }

}
