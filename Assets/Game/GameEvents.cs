using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Transform> OnPlayerEnterRoom;

    public static void PlayerEnterRoom(Transform roomBounder)
    {
        OnPlayerEnterRoom?.Invoke(roomBounder);
    }
}
