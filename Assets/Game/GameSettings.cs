using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;
    [SerializeField] private int roomBorder;

    #region PROPERTIES

    public int RoomWidth
    {
        get { return roomWidth; }
    }
    public int RoomHeight
    {
        get { return roomHeight; }
    }
    public int RoomBorder
    {
        get { return roomBorder; }
    }
    #endregion
}
