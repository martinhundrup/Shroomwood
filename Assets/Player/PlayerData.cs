using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void HealthChangedAction();

    // The event called when this object collides with a hitbox.
    public event HealthChangedAction OnHealthChanged;

    #endregion

    #region ATTRIBUTES

    // The value to multiply the movement vector by; the speed of the player.
    [SerializeField] private float movementSpeed;

    // The maximum amount of health the player can have.
    [SerializeField] private int maxHealth;

    // The current health of the player.
    [SerializeField] private int currentHealth;

    // The amount of time the player is invulnerable after taking damage.
    [SerializeField] private float damageBoostDuration;

    #endregion

    #region PROPERTIES
    
    // Gets or sets the movement speed.
    public float MovementSpeed
    {
        get { return this.movementSpeed; }
        set { this.movementSpeed = value; }
    }

    // Gets or sets the current health value.
    public int CurrentHealth
    {
        get { return this.currentHealth; }
        set 
        { 
            this.currentHealth = value;
            this.OnHealthChanged();

            // ensure player can not have more health than their max
            if (this.currentHealth > maxHealth)
                this.currentHealth = maxHealth;
        }
    }

    // Gets or sets the damage boost duration.
    public float DamageBoostDuration
    {
        get { return this.damageBoostDuration; }
        set { this.damageBoostDuration = value; }
    }

    // Gets or sets the max health value.
    public int MaxHealth
    {
        get { return this.maxHealth; }
        set { this.maxHealth = value; }
    }

    #endregion

    #region METHODS

    public void ResetHealth()
    {
        this.currentHealth = this.maxHealth;
    }

    #endregion
}
