using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    #region ATRIBUTES

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

    #endregion

    #region UNITY CALLBACKS

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

        // smooth the position to a new variable
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPos, ref zeroVector, smoothness);

        // set our position to the smoothed position
        transform.position = smoothedPosition;
    }

    #endregion
}