using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private float playerSpeed;

    #region PROPERTIES

    public float PlayerSpeed
    {
        get { return playerSpeed; }
    }

    #endregion
}
