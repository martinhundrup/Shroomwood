using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region COMPONENTS

    // Holds the reference to the player's Rigidbody2D component.
    private Rigidbody2D rigidBody;

    // Holds the reference to the player's Sprite Renderer component.
    private SpriteRenderer spriteRenderer;

    // Holds the reference to the player's Animator component.
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

    // Denotes whether the player is currently rolling or not.
    [SerializeField] private bool isRolling = false;

    // The time in between weapon blinks
    [SerializeField] float blinkTime;

    // Whether or not the playe is immune to taking damage.
    private bool isInvulnerable = false;

    private WeaponModifiers weaponModifiers = new WeaponModifiers();
    [SerializeField] private WeaponData weaponData;

    // the current animation being played.
    [SerializeField] private string current_anim = string.Empty;
    [SerializeField] private string current_weapon_anim = string.Empty;
    [SerializeField] private string current_effect_anim = string.Empty;

    // idles
    private const string IDLE_F = "Idle_Front";
    private const string IDLE_B = "Idle_Back";
    private const string IDLE_S = "Idle_Side";
    private const string IDLE_QF = "Idle_QuarterFront";
    private const string IDLE_QB = "Idle_QuarterBack";

    // running
    private const string RUN_F = "Run_Front";
    private const string RUN_B = "Run_Back";
    private const string RUN_S = "Run_Side";
    private const string RUN_QF = "Run_QuarterFront";
    private const string RUN_QB = "Run_QuarterBack";

    // attacking
    private const string ATTACK_F = "Attack_Front";
    private const string ATTACK_B = "Attack_Back";
    private const string ATTACK_S = "Attack_Side";
    private const string ATTACK_QF = "Attack_QuarterFront";
    private const string ATTACK_QB = "Attack_QuarterBack";

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRolling = true;
            //TODO: implement rolling
            // should be similar to attacking
            // player can roll if not attacking
            // add force to rigidbody
            // make invulnerable
            // start roll timer
            // make not rolling
        }
    }

    // Called when this collides with a trigger collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitbox = collision.GetComponent<Hitbox>();
       
        if (!isInvulnerable && hitbox && !CompareTag(hitbox.Tag)) // player collided with an enemy hitbox
        {
            Debug.Log(collision.gameObject);
            this.playerData.CurrentHealth -= hitbox.Damage;
            StartCoroutine(MakeInvulnerable(this.playerData.DamageBoostDuration));
        }
    }

    // Called every frame that the player collides with a trigger.
    private void OnTriggerStay2D(Collider2D collision)
    {
        var hitbox = collision.GetComponent<Hitbox>();
        if (!isInvulnerable && hitbox && !CompareTag(hitbox.Tag)) // player collided with an enemy hitbox
        {
            Debug.Log("player hit");
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
                hitbox.GetComponent<Hitbox>().InitValues(this.weaponData);
                hitbox.GetComponent<Hitbox>().StartTimer();

                // rotate to the correct direction;
                hitbox.transform.position = this.transform.position + 
                    (DirToVect(this.direction).normalized * weaponData.HitBoxDistanceOffset);
                hitbox.transform.rotation = DirToQuaternion(this.direction);
                
                isAttacking = true;
                this.isAttackCooldownDone = false;
                this.rigidBody.velocity = Vector2.zero;
                StartCoroutine(AttackDuration());
            }
            else if (Input.GetButtonDown("Ranged"))
            {
                GameObject proj = Instantiate(this.projectile.gameObject);
                proj.transform.position = this.transform.position;
                Projectile p = proj.GetComponent<Projectile>();

                p.Fire(DirToVect(this.direction));
            }
        }
    }

    // Responsible for figuring out what animation to play
    public void Animate()
    {
        // the anim to play
        string _anim = string.Empty;

        if (isRunning)
        {
            _Enable_Animator(weaponAnimator, false);
            _Enable_Animator(attackEffectAnimator, false);

            switch (this.direction)
            {
                case Direction.Left:
                    _anim = RUN_S; break;
                case Direction.Right:
                    _anim = RUN_S; break;
                case Direction.Up:
                    _anim = RUN_B; break;
                case Direction.DownLeft:
                    _anim = RUN_QF; break;
                case Direction.DownRight:
                    _anim = RUN_QF; break;
                case Direction.UpLeft:
                    _anim = RUN_QB; break;
                case Direction.UpRight:
                    _anim = RUN_QB; break;
                default:
                    _anim = RUN_F; break;
            }
        }
        else if (isAttacking)
        {

            _Enable_Animator(weaponAnimator, true);
            _Enable_Animator(attackEffectAnimator, true);

            string _dir = string.Empty;

            // animate shroomie
            switch (this.direction)
            {
                case Direction.Left:
                    _anim = ATTACK_S;
                    _dir = "_Side";
                    break;
                case Direction.Right:
                    _anim = ATTACK_S;
                    _dir = "_Side";
                    break;
                case Direction.Up:
                    _anim = ATTACK_B;
                    _dir = "_Back";
                    break;
                case Direction.DownLeft:
                    _anim = ATTACK_QF;
                    _dir = "_QuarterFront";
                    break;
                case Direction.DownRight:
                    _anim = ATTACK_QF;
                    _dir = "_QuarterFront";
                    break;
                case Direction.UpLeft:
                    _anim = ATTACK_QB;
                    _dir = "_QuarterBack";
                    break;
                case Direction.UpRight:
                    _anim = ATTACK_QB;
                    _dir = "_QuarterBack";
                    break;
                default:
                    _anim = ATTACK_F;
                    _dir = "_Front";
                    break;
            }

            string _weapon = weaponData.ItemName + _dir;

            // animate weapon
            if (current_weapon_anim != _weapon)
            {
                this.weaponAnimator.Play(_weapon);
                this.current_weapon_anim = _weapon;
            }

            string _effect = weaponData.EffectName + _dir;

            // animate effect
            if (current_effect_anim != _effect)
            {
                this.attackEffectAnimator.Play(_effect);
                this.current_effect_anim = _effect;
            }
        }
        else if (isRolling)
        {

        }
        else // idle
        {

            _Enable_Animator(weaponAnimator, false);
            _Enable_Animator(attackEffectAnimator, false);

            switch (this.direction)
            {
                case Direction.Left:
                    _anim = IDLE_S; break;
                case Direction.Right:
                    _anim = IDLE_S; break;
                case Direction.Up:
                    _anim = IDLE_B; break;
                case Direction.DownLeft:
                    _anim = IDLE_QF; break;
                case Direction.DownRight:
                    _anim = IDLE_QF; break;
                case Direction.UpLeft:
                    _anim = IDLE_QB; break;
                case Direction.UpRight:
                    _anim = IDLE_QB; break;
                default:
                    _anim = IDLE_F; break;
            }
        }

        if (current_anim != _anim)
        {
            this.animator.Play(_anim);
            this.current_anim = _anim;
        }

        // enables or disables the animator and sprite renderer.
        void _Enable_Animator(Animator _anim, bool _bool)
        {
            _anim.GetComponent<SpriteRenderer>().enabled = _bool;
            _anim.GetComponent<SpriteRenderer>().flipX = this.spriteRenderer.flipX;

            if (!_bool)
                _anim.Play("null");

            this.current_weapon_anim = string.Empty;
            this.current_effect_anim = string.Empty;
        }
    }

    // Called by inventory manager when a new weapon is picked up.
    public void EquipWeapon(WeaponDrop _weapon)
    {
        this.weaponData = (WeaponData)_weapon.ItemData;
        this.weaponModifiers = _weapon.Modifiers;
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
    }

    // Makes the player invulnerable for a parameterized amount of time.
    private IEnumerator MakeInvulnerable(float _time)
    {
        isInvulnerable = true;
        StartCoroutine(Blink());                                
        yield return new WaitForSeconds(_time);
        isInvulnerable = false;
    }

    // Blinks the player's prite renderer until they are no longer invulnerable.
    private IEnumerator Blink()
    {       
        while (isInvulnerable)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkTime);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkTime);
        }
        
        spriteRenderer.enabled = true;
        yield return null;
    }

    #endregion

    #region UTILITY

    // Finds the direction something is facing based on x and y coords.
    private Direction FindDir(float _x, float _y)
    {

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

    private Quaternion DirToQuaternion(Direction _dir)
    {
        var _vect = DirToVect(_dir).normalized;

        float angle = Mathf.Atan2(_vect.y, _vect.x) * Mathf.Rad2Deg + 270f;

        return Quaternion.Euler(0, 0, angle);
    }

    #endregion
}
