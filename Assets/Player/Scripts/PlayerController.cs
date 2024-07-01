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
    private Blink blink;
    private bool isInvulnerable;

    private void Awake()
    {
        playerStats = DataDictionary.PlayerStats;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        blink = GetComponentInChildren<Blink>();
        animator = GetComponentInChildren<Animator>();
        swingEffect = GetComponentInChildren<SwingEffect>();
        weaponSprite = GetComponentInChildren<WeaponSprite>();

        // for now, heal player on awake
        playerStats.PlayerHealth = playerStats.PlayerMaxHealth;
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


    // detect collision from enemy bodies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null && !isInvulnerable)
        {
            playerStats.PlayerHealth -= enemy.ContactDamage;
            StartCoroutine(MakeInvulnerable());
            if (playerStats.PlayerHealth == 0)
            {
                Debug.LogError("Shroomie died!");
            }
        }        
    }

    private IEnumerator MakeInvulnerable()
    {
        isInvulnerable = true;
        blink.StartBlinking();

        yield return new WaitForSeconds(0.4f);

        isInvulnerable = false;
        blink.StopBlinking();
    }
}
