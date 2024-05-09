using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region COMPONENTS

    // -- COMPONENTS -- //

    /// Holds the reference to the player's Rigidbody2D component.
    private Rigidbody2D rigidBody;

    /// Holds the reference to the player's Sprite Renderer component.
    private SpriteRenderer spriteRenderer;

    /// Holds the reference to the player's Animator component
    private Animator animator;

    #endregion

    #region STATS

    // -- STATS -- //

    /// The value to multiply the movement vector by; the speed of the player.
    [SerializeField] private float movementSpeed;

    #endregion

    #region PROPERTIES

    // -- PROPERTIES -- //

    /// Gets or sets the movement speed of the player.
    public float MovementSpeed
    {
        get { return this.movementSpeed; }
        set {  this.movementSpeed = value; }
    }

    #endregion

    #region UNITY CALLBACKS

    /// Called once at beginning of scene.
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    /// Called once a frame. Varies with framerate.
    private void Update()
    {
        Movement();
    }

    #endregion

    #region METHODS

    /// Handles movement related input and rigidbody forces responsible for movement
    private void Movement()
    {

        // find the horizontal movement vector
        float x = Input.GetAxis("Horizontal");

        // find the vertical movement vector
        float y = Input.GetAxis("Vertical");

        // update animation state to running if moving in either direction
        if (Mathf.Abs(x) != 0 || Mathf.Abs(y) != 0)
        {
            animator.SetBool("Running", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idle", true);
        }

        // adjust the flipX attribute of the sprite appropriately
        // we don't use an else so the player won't flip back to default when negative movement stops
        if (x < 0)
            spriteRenderer.flipX = true;
        else if (x > 0)
            spriteRenderer.flipX = false;

        // create the input based movement vector; this will be normalized to achieve the final movement vector
        Vector2 movement_vector = new Vector2(x, y);
        movement_vector.Normalize(); // this normalizes (makes the magnitude 1)

        // set the rigidbody velocity to the movement vector
        // velocity is normalized with framerate by default
        rigidBody.velocity = movement_vector * movementSpeed;
    }

    #endregion
}
