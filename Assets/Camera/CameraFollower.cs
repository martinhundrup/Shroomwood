using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);   
    [SerializeField, Range(0, 1)] private float smoothness;

    private Vector3 zeroVector = Vector3.zero;  
    private Vector3 targetPos;

    private Vector3 maxPositions;
    private Vector3 minPositions;


    // Called when object is created.
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        target = player.transform;
        GameEvents.OnPlayerEnterRoom += ChangeBounds;
    }

    // Called once a frame. Varies with framerate.    
    private void Update()
    {
        if (target != null && transform.position != target.position) // only moves the object if it's not at the target already
        {
            UpdatePosition();
        }
    }


    // Updates the camera's position towards the target.
    private void UpdatePosition()
    {
        // apply any offset
        targetPos = target.position + offset;
        if (targetPos.x < minPositions.x)
        {
            targetPos = new Vector2(minPositions.x, targetPos.y);
        }
        else if (targetPos.x > maxPositions.x)
        {
            targetPos = new Vector2(maxPositions.x, targetPos.y);
        }

        if (targetPos.y < minPositions.y)
        {
            targetPos = new Vector2(targetPos.x, minPositions.y);
        }
        else if (targetPos.y > maxPositions.y)
        {
            targetPos = new Vector2(targetPos.x, maxPositions.y);
        }

        targetPos = new Vector3(targetPos.x, targetPos.y, offset.z);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPos, ref zeroVector, smoothness);
        transform.position = smoothedPosition;
    }

    // Called when player enters a new room
    public void ChangeBounds(Transform _bounder)
    {
        float x = _bounder.position.x;
        float y = _bounder.position.y;
        float width = _bounder.localScale.x;
        float height = _bounder.localScale.y;

        float max_x = x - (21 / 2) + (DataDictionary.GameSettings.RoomWidth) / 2;
        float min_x = x + (21 / 2) - (DataDictionary.GameSettings.RoomWidth) / 2;
        float max_y = y - (13 / 2) + (DataDictionary.GameSettings.RoomHeight) / 2;
        float min_y = y + (13 / 2) - (DataDictionary.GameSettings.RoomHeight) / 2;

        if (max_x <= min_x)
        {
            max_x = min_x = x;
        }
        if (max_y <= min_y)
        {
            max_y = min_y = y;
        }

        this.maxPositions = new Vector2(max_x, max_y);
        this.minPositions = new Vector2(min_x, min_y);
    }
}