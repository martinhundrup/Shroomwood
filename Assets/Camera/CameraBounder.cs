using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounder : MonoBehaviour
{
    private BoxCollider2D col;

    private void OnEnable()
    {
        col = GetComponent<BoxCollider2D>();

        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
    }

    // Called when an object collides with this one.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEvents.PlayerEnterRoom(transform);
        }
    }
}