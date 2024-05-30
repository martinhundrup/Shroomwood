using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounder : MonoBehaviour
{
    // The event type to be called when a player overlaps with this object.
    public delegate void OnPlayerEnterAction();

    // The event to be called.
    public event OnPlayerEnterAction OnPlayerEnter;

    // Called when an object collides with this one.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (OnPlayerEnter != null)
                this.OnPlayerEnter();
        }
    }
}
