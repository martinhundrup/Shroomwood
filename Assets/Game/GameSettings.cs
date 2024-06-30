using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [SerializeField] Vector2Int roomSize;
    [SerializeField] private int roomBorder;
    [SerializeField] private int gameLevel; // the current level the player is on

    #region PROPERTIES

    public Vector2Int RoomSize
    {
        get { return roomSize; }
    }
    public int RoomBorder
    {
        get { return roomBorder; }
    }
    public int GameLevel
    {
        get { return gameLevel; }
    }
    #endregion
}
