using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerMaxHealth;
    [SerializeField] private float playerHealth; // current health

    #region PROPERTIES

    public float PlayerSpeed
    {
        get { return playerSpeed; }
    }

    public float PlayerMaxHealth
    {
        get { return playerMaxHealth; }
    }
    public float PlayerHealth
    {
        get { return playerHealth; }
        set 
        { 
            playerHealth = value; 
            playerHealth = Mathf.Clamp(playerHealth, 0f, playerMaxHealth);
        }
    }

    #endregion
}
