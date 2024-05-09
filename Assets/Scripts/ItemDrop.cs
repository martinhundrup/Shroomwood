using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    #region ATRIBUTES

    // The transform of the game object the camera will target.
    [SerializeField] private Vector3 target;

    // The offset at which we look at the target.
    [SerializeField] private Vector3 offset;

    // The damping value to follow the target with. Smaller values result in closer following.
    [SerializeField] private float smoothness;

    // The 3D zero vector as a variable.
    private Vector3 zeroVector = Vector3.zero;

    // Whether or not the item is currently targeting the player.
    private bool targetsPlayer = false;

    // Whether or not the item as arrived at its target position.
    private bool hasArrived = false;

    #endregion

    #region PROPERTIES

    // Gets or sets the transform of the game object the camera will target.
    public Vector3 Target
    {
        get { return this.target; }
        set { this.target = value; }
    }

    // Gets or sets the damping value to follow the target with. Smaller values result in closer following.
    public float Smoothness
    {
        get { return this.smoothness; }
        set { this.smoothness = value; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Called once a frame. Varies with framerate.
    private void Update()
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, target)) > 0.2f) // only moves the object if it's not at the target already
        {
            UpdatePosition();
        }
        else // makes sure it stops moving when it arrived at initial target location.
        {
            hasArrived = true;

            // destroys this object when it arrives at the player.
            if (targetsPlayer)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Called when another object is in a 2D trigger collider.
    private void OnTriggerStay2D(Collider2D collision)
    {
        // the player is in our trigger collider
        if (hasArrived && collision.CompareTag("Player"))
        {
            target = collision.gameObject.transform.position;
            targetsPlayer = true;
        }
    }

    #endregion

    #region METHODS

    // Updates the items position towards the target.
    private void UpdatePosition()
    {
        // apply any offset
        Vector2 targetPos = target + offset;

        // smooth the position to a new variable
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPos, ref zeroVector, smoothness);

        // set our position to the smoothed position
        transform.position = smoothedPosition;
    }
    #endregion
}