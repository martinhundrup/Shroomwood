using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    #region ATTRIBUTES

    // Ref to the player.
    private PlayerController player;

    // The transform of the game object the camera will target.
    [SerializeField] private Transform target;

    // The offset at which we look at the target.
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    
    // The damping value to follow the target with. Smaller values result in closer following.    
    [SerializeField] private float smoothness;
    
    // The 3D zero vector as a variable.    
    private Vector3 zeroVector = Vector3.zero;
    
    // The final position to which the camera moves after the offset.    
    private Vector3 targetPos;

    // The max coords the camera can move to.
    [SerializeField] private Vector3 maxPositions;

    // The min coords the camera can move to.
    [SerializeField] private Vector3 minPositions;

    #endregion

    #region UNITY CALLBACKS

    // Called when object is created.
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        player.OnEnterRoom += this.ChangeBounds;
    }

    // Called once a frame. Varies with framerate.    
    private void Update()
    {
        if (transform.position != target.position) // only moves the object if it's not at the target already
        {
            UpdatePosition();
        }
    }

    #endregion

    #region METHODS

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

        // ensures z value is 0
        targetPos = new Vector3(targetPos.x, targetPos.y, offset.z);

        // smooth the position to a new variable
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPos, ref zeroVector, smoothness);

        // set our position to the smoothed position
        transform.position = smoothedPosition;
    }

    // Called when player enters a new room
    private void ChangeBounds(Transform _bounder)
    {

        Debug.Log("change bounds");
        float x = _bounder.position.x;
        float y = _bounder.position.y;
        float width = _bounder.localScale.x;
        float height = _bounder.localScale.y;

        float max_x = x - 8 + width / 2;
        float min_x = x + 8 - width / 2;
        float max_y = y - 4 + height / 2;
        float min_y = y + 4 - height / 2;

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

    #endregion
}