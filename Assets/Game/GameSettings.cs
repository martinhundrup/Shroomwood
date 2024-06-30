using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [SerializeField] Vector2Int roomSize;
    [SerializeField] private int roomBorder;

    #region PROPERTIES

    public Vector2Int RoomSize
    {
        get { return roomSize; }
    }
    public int RoomBorder
    {
        get { return roomBorder; }
    }
    #endregion
}
