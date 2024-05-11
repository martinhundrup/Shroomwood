using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region COMPONENTS

    // Holds the reference to the player's Rigidbody2D component.
    private Rigidbody2D rigidBody;

    // Holds the reference to the player's Sprite Renderer component.
    private SpriteRenderer spriteRenderer;

    // Holds the reference to the player's Animator component
    private Animator animator;

    #endregion

    #region ATTRIBUTES

    // Contains the scriptable object that contains the player's stats.
    [SerializeField] private PlayerData playerData;

    // The player's melee hitbox.
    [SerializeField] private GameObject meleeHitbox;

    // The last direction the player was moving/facing.
    private Direction direction;

    #endregion

    #region PROPERTIES    

    #endregion

    #region UNITY CALLBACKS

    // Called once at beginning of scene.
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    // Called once a frame. Varies with framerate.
    private void Update()
    {
        Movement();
        Attack();
    }

    #endregion

    #region METHODS

    // Handles movement related input and rigidbody forces responsible for movement
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

            // get the direction the player is facing only when moving
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // facing left or right
                if (x > 0) this.direction = Direction.Right;
                else this.direction = Direction.Left;
            }
            else
            {
                // facing up or down
                if (y > 0) this.direction = Direction.Up;
                else this.direction = Direction.Down;
            }
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
        rigidBody.velocity = movement_vector * this.playerData.MovementSpeed;
    }

    // Handles the input and logistics of the attack
    private void Attack()
    {
        // melee attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject hitbox = Instantiate(this.meleeHitbox.gameObject, transform);
            hitbox.GetComponent<Hitbox>().StartTimer(0.2f);
            
            switch (this.direction)
            {
                case Direction.Left:
                    hitbox.transform.position = new Vector3 (-1, 0, 0) + this.transform.position; break;
                case Direction.Right:
                    hitbox.transform.position = new Vector3(1, 0, 0) + this.transform.position; break;
                case Direction.Up:
                    hitbox.transform.position = new Vector3(0, 1, 0) + this.transform.position; break;
                default:
                    hitbox.transform.position = new Vector3(0, -1, 0) + this.transform.position; break;
            }
        }
    }

    #endregion
}
