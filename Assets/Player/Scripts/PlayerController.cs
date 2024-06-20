using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private SwingEffect swingEffect;
    private WeaponSprite weaponSprite;
    private Vector2 facingDirection; // the direction of the cursor relative to shroomie

    private PlayerStats playerStats;


    private void Awake()
    {
        playerStats = DataDictionary.PlayerStats;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        swingEffect = GetComponentInChildren<SwingEffect>();
        weaponSprite = GetComponentInChildren<WeaponSprite>();
    }

    // get input for movement
    private void Update()
    {
        Movement();
        FindFacingDirection();        
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            swingEffect.Swing(0.15f);
            weaponSprite.Attack(0.15f);
        }
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        var movementVect = new Vector2(x, y).normalized * playerStats.PlayerSpeed;

        if (movementVect.magnitude > 0.01)
        {
            animator.Play("run");
        }
        else
        {
            animator.Play("idle");
        }

        rb.velocity = movementVect;
    }

    private void FindFacingDirection()
    {
        var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        facingDirection = newPos.normalized;

        bool flipX = true;

        // set sprite flipped on x or not depending on facing direction
        if (facingDirection.x <= 0) flipX = false;

        sr.flipX = flipX;
        weaponSprite.SpriteFlipX(flipX);
    }
}
