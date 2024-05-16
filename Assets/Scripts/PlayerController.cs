using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void EnteredRoomAction(Transform _room);

    // The event called when this object collides with a hitbox.
    public event EnteredRoomAction OnEnterRoom;

    #endregion

    #region COMPONENTS

    // Holds the reference to the player's Rigidbody2D component.
    private Rigidbody2D rigidBody;

    // Holds the reference to the player's Sprite Renderer component.
    private SpriteRenderer spriteRenderer;

    // Holds the reference to the player's Running Animator component
    private Animator animator;

    #endregion

    #region ATTRIBUTES

    // Contains the scriptable object that contains the player's stats.
    [SerializeField] private PlayerData playerData;

    // The player's melee hitbox.
    [SerializeField] private GameObject meleeHitbox;

    // The player's ranged attack projectile.
    [SerializeField] private GameObject projectile;

    // The last direction the player was moving/facing.
    [SerializeField] private Direction direction;

    // The time needed to wait between attacks.
    [SerializeField] private float attackCooldown;

    // Denotes whether the player is currently attackig or not.
    bool isAttacking = false;

    // Denotes whether the player is currently running or not.
    [SerializeField] bool isRunning = false;

    // Controls the running animations.
    [SerializeField] private AnimatorController runController;

    // Controls the idle animations.
    [SerializeField] private AnimatorController idleController;

    #endregion

    #region PROPERTIES    

    // Gets the current direction of the player.
    public Direction Direction
    {
        get { return this.direction; }
    }

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
        Animate();
    }

    // Called when this collides with a trigger collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraBounder"))
        {
            this.OnEnterRoom(collision.gameObject.transform);
        }
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
            isRunning = true;

            // get the direction the player is facing only when moving
            this.direction = FindDir(x, y);
        }
        else
        {
            isRunning = false;
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
        // don't attack again if already attacking
        if (isAttacking)
        {
            return;
        }
        else
        {
            // melee attack
            if (Input.GetButtonDown("Melee"))
            {
                GameObject hitbox = Instantiate(this.meleeHitbox.gameObject);
                hitbox.GetComponent<Hitbox>().StartTimer(0.2f);

                hitbox.transform.position = DirToVect(this.direction) + this.transform.position;
                _Attack();
            }
            else if (Input.GetButtonDown("Ranged"))
            {
                GameObject proj = Instantiate(this.projectile.gameObject);
                proj.transform.position = this.transform.position;
                Projectile p = proj.GetComponent<Projectile>();

                p.Dir = DirToVect(this.direction);
                p.Fire();
            }

            void _Attack()
            {
                isAttacking = true;
                float t = this.playerData.MovementSpeed;
                this.playerData.MovementSpeed = 0;
                StartCoroutine(AttackCooldown(this.attackCooldown, t));
            }
        }
    }

    // Responsible for figuring out what animation to play
    public void Animate()
    {
        ResetAnimations();

        if (!isRunning)
        {
            animator.runtimeAnimatorController = idleController;
        }
        else
        {
            animator.runtimeAnimatorController = runController;
        }

        // player faces sideways when moving diagonal
        switch (this.direction)
        {
            case Direction.Left:
                this.animator.SetBool("Side", true); break;
            case Direction.Right:
                this.animator.SetBool("Side", true); break;
            case Direction.Up:
                this.animator.SetBool("Back", true); break;
            case Direction.DownLeft:
                this.animator.SetBool("Side", true); break;
            case Direction.DownRight:
                this.animator.SetBool("Side", true); break;
            case Direction.UpLeft:
                this.animator.SetBool("Side", true); break;
            case Direction.UpRight:
                this.animator.SetBool("Side", true); break;
            default:
                this.animator.SetBool("Front", true); break;
        }
    }

    // Waits an amount of time before setting the 'isAttacking' bool to false.
    private IEnumerator AttackCooldown(float _waitTime, float _speed)
    {
        yield return new WaitForSeconds(_waitTime);

        isAttacking = false;
        this.playerData.MovementSpeed = _speed;
    }

    #endregion

    #region UTILITY

    // Finds the direction something is facing based on x and y coords.
    private Direction FindDir(float _x, float _y)
    {

        //Debug.Log("x: " + _x + "\ny: " + _y);

        if (_x == 0 && _y > 0) // up
        {
            return Direction.Up;
        }
        else if (_x == 0 && _y < 0) // down
        {
            return Direction.Down;
        }
        else if (_x < 0 && _y == 0) // left
        {
            return Direction.Left;
        }
        else if (_x > 0 && _y == 0) // right
        {
            return Direction.Right;
        }
        else if (_x < 0 && _y > 0) // up left
        {
            return Direction.UpLeft;
        }
        else if (_x > 0 && _y > 0) // up right
        {
            return Direction.UpRight;
        }
        else if (_x < 0 && _y < 0) // down left
        {
            return Direction.DownLeft;
        }
        else // down right
        {
            return Direction.DownRight;
        }
    }

    // Converts a direction enum to a normalized vector 3.
    private Vector3 DirToVect(Direction _dir)
    {
        switch (_dir)
        {
            case Direction.Left:
                return new Vector3(-1, 0, 0); 
            case Direction.Right:
                return new Vector3(1, 0, 0); 
            case Direction.Up:
                return new Vector3(0, 1, 0); 
            case Direction.UpLeft:
                return new Vector3(-1, 1, 0).normalized; 
            case Direction.UpRight:
                return new Vector3(1, 1, 0).normalized; 
            case Direction.DownLeft:
                return new Vector3(-1, -1, 0).normalized; 
            case Direction.DownRight:
                return new Vector3(1, -1, 0).normalized; 
            default:
                return new Vector3(0, -1, 0); 
        }
    }

    // Cancels all animations.
    private void ResetAnimations()
    {
        this.animator.SetBool("Front", false);
        this.animator.SetBool("Back", false);
        this.animator.SetBool("Side", false);
    }

    #endregion
}
