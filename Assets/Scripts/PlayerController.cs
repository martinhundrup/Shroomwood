using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region COMPONENTS

    // Holds the reference to the player's Rigidbody2D component.
    private Rigidbody2D rigidBody;

    // Holds the reference to the player's Sprite Renderer component.
    private SpriteRenderer spriteRenderer;

    // Holds the reference to the player's Running Animator component.
    private Animator animator;

    // Holds reference to the animator for the weapon animation.
    [SerializeField] Animator weaponAnimator;

    // Holds reference to the animator for the weapon attack effect.
    [SerializeField] Animator attackEffectAnimator;

    #endregion

    #region ATTRIBUTES

    // Contains the scriptable object that contains the player's stats.
    [SerializeField] private PlayerData playerData;

    // The player's ranged attack projectile.
    [SerializeField] private GameObject projectile;

    // The last direction the player was moving/facing.
    [SerializeField] private Direction direction;

    // Denotes whether the player is currently attackig or not.
    [SerializeField] private bool isAttacking = false;

    // Whether the attack cooldown amount of time has been waited since last attack.
    [SerializeField] private bool isAttackCooldownDone = true;

    // Denotes whether the player is currently running or not.
    [SerializeField] bool isRunning = false;

    // Controls the running animations.
    [SerializeField] private AnimatorController runController;

    // Controls the idle animations.
    [SerializeField] private AnimatorController idleController;

    // Controls the attack animations.
    [SerializeField] private AnimatorController attackController;

    // Whether or not the playe is immune to taking damage.
    private bool isInvulnerable = false;

    private WeaponModifiers weaponModifiers = new WeaponModifiers();
    [SerializeField] private WeaponData weaponData;

    #endregion

    #region PROPERTIES    

    // Gets the current direction of the player.
    public Direction Direction
    {
        get { return this.direction; }
    }

    #endregion

    #region UNITY CALLBACKS



    // Called once when object is created.
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        this.isAttackCooldownDone = true;
    }

    // Called once a frame. Varies with framerate.
    private void Update()
    {
        if (!isAttacking)
            Movement();
        Attack();
        Animate();



        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"weapon duration modifier: {this.weaponModifiers.AttackDurationModifier}");
        }
    }

    // Called when this collides with a trigger collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitbox = collision.GetComponent<Hitbox>();
        var weapon = collision.GetComponent<Weapon>();

        if (!isInvulnerable && hitbox && !CompareTag(hitbox.Tag)) // player collided with an enemy hitbox
        {
            this.playerData.CurrentHealth -= hitbox.Damage;
            StartCoroutine(MakeInvulnerable(this.playerData.DamageBoostDuration));
        }


        else if (weapon)
        {
            this.weaponData = weapon.WeaponData;
            this.weaponModifiers = weapon.Modifiers;
            this.weaponAnimator.runtimeAnimatorController = this.weaponData.WeaponAnimator;
            this.attackEffectAnimator.runtimeAnimatorController = this.weaponData.EffectAnimator;
        }
    }

    // Called every frame that the player collides with a trigger.
    private void OnTriggerStay2D(Collider2D collision)
    {
        var hitbox = collision.GetComponent<Hitbox>();
        if (!isInvulnerable && hitbox && !CompareTag(hitbox.Tag)) // player collided with an enemy hitbox
        {
            this.playerData.CurrentHealth -= hitbox.Damage;
            StartCoroutine(MakeInvulnerable(this.playerData.DamageBoostDuration));
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
        else {
            isRunning = false;
        }

        // adjust the flipX attribute of the sprite appropriately
        // we don't use an else so the player won't flip back to default when negative movement stops
        if (x < 0)
            spriteRenderer.flipX = true;
        else if (x > 0)
            spriteRenderer.flipX = false;

        // don't flip when facing up or down
        if (this.direction == Direction.Up || this.direction == Direction.Down)
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
            isRunning = false;
            return;
        }
        else if (this.isAttackCooldownDone && this.weaponData != null) // only allow attacking if a weapon has been collected
        {


            // melee attack
            if (Input.GetButtonDown("Melee"))
            {
                GameObject hitbox = Instantiate(this.weaponData.Hitbox.gameObject);
                hitbox.GetComponent<Hitbox>().StartTimer(0.2f);

                hitbox.transform.position = DirToVect(this.direction) * 0.6f + this.transform.position + new Vector3(0f, -0.1f, 0f);
                _Attack();
            }
            else if (Input.GetButtonDown("Ranged"))
            {
                GameObject proj = Instantiate(this.projectile.gameObject);
                proj.transform.position = this.transform.position;
                Projectile p = proj.GetComponent<Projectile>();

                p.Fire(DirToVect(this.direction));
            }

            void _Attack()
            {
                isAttacking = true;
                this.isAttackCooldownDone = false;

                // enable the attacking animators
                _AnimateAttack(this.weaponAnimator);
                _AnimateAttack(this.attackEffectAnimator);


                this.rigidBody.velocity = Vector2.zero;
                StartCoroutine(AttackDuration());


                void _AnimateAttack(Animator _anim)
                {
                    //_anim.enabled = true;
                    _anim.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    _anim.gameObject.GetComponent<SpriteRenderer>().flipX = this.spriteRenderer.flipX;

                    // reset any bools
                    _anim.SetBool("Front", false);
                    _anim.SetBool("Back", false);
                    _anim.SetBool("Side", false);
                    _anim.SetBool("QuarterFront", false);
                    _anim.SetBool("QuarterBack", false);
                    _anim.SetBool("Idle", false);

                    switch (this.direction)
                    {
                        case Direction.Left:
                            _anim.SetBool("Side", true); break;
                        case Direction.Right:
                            _anim.SetBool("Side", true); break;
                        case Direction.Up:
                            _anim.SetBool("Back", true); break;
                        case Direction.DownLeft:
                            _anim.SetBool("QuarterFront", true); break;
                        case Direction.DownRight:
                            _anim.SetBool("QuarterFront", true); break;
                        case Direction.UpLeft:
                            _anim.SetBool("QuarterBack", true); break;
                        case Direction.UpRight:
                            _anim.SetBool("QuarterBack", true); break;
                        default:
                            _anim.SetBool("Front", true); break;
                    }

                }
            }

        }
        else // not attacking
        {
            // disable weapon animations

            if (this.weaponAnimator)
            {
                //this.weaponAnimator.enabled = false;
                this.weaponAnimator.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }

            if (this.attackEffectAnimator)
            {
                //this.attackEffectAnimator.enabled = false;
                this.attackEffectAnimator.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Responsible for figuring out what animation to play
    public void Animate()
    {
        ResetAnimations();

        if (isRunning)
        {
            animator.runtimeAnimatorController = runController;
            
        }
        else if (isAttacking)
        {
            animator.runtimeAnimatorController = attackController;
        }
        else
        {
            animator.runtimeAnimatorController = idleController;
        }

        switch (this.direction)
        {
            case Direction.Left:
                this.animator.SetBool("Side", true); break;
            case Direction.Right:
                this.animator.SetBool("Side", true); break;
            case Direction.Up:
                this.animator.SetBool("Back", true); break;
            case Direction.DownLeft:
                this.animator.SetBool("QuarterFront", true); break;
            case Direction.DownRight:
                this.animator.SetBool("QuarterFront", true); break;
            case Direction.UpLeft:
                this.animator.SetBool("QuarterBack", true); break;
            case Direction.UpRight:
                this.animator.SetBool("QuarterBack", true); break;
            default:
                this.animator.SetBool("Front", true); break;
        }
    }

    // Waits an amount of time before setting the 'isAttacking' bool to false.
    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(this.weaponData.Duration * this.weaponModifiers.AttackDurationModifier);

        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    // Waits for the attack cooldown duration.
    private IEnumerator AttackCooldown()
    {
        isAttackCooldownDone = false;

        

        yield return new WaitForSeconds(this.weaponData.Cooldown * this.weaponModifiers.AttackCooldownModifier);

        isAttackCooldownDone = true;

        // reset attack animators
        this.weaponAnimator.SetBool("Idle", true);
        this.weaponAnimator.SetBool("Front", false);
        this.weaponAnimator.SetBool("Side", false);
        this.weaponAnimator.SetBool("Back", false);
        this.weaponAnimator.SetBool("QuarterFront", false);
        this.weaponAnimator.SetBool("QuarterBack", false);

        this.attackEffectAnimator.SetBool("Idle", true);
        this.attackEffectAnimator.SetBool("Front", false);
        this.attackEffectAnimator.SetBool("Side", false);
        this.attackEffectAnimator.SetBool("Back", false);
        this.attackEffectAnimator.SetBool("QuarterFront", false);
        this.attackEffectAnimator.SetBool("QuarterBack", false);
    }

    // Makes the player invulnerable for a parameterized amount of time.
    private IEnumerator MakeInvulnerable(float _time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(_time);
        isInvulnerable = false;
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
        this.animator.SetBool("QuarterFront", false);
        this.animator.SetBool("QuarterBack", false);
    }

    #endregion
}
