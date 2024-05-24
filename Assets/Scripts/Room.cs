using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Application;
using System.Runtime.CompilerServices;

public class Room : MonoBehaviour
{
    // Reference to the camera bounder child.
    private CameraBounder cameraBounder;

    // Reference to camera.
    private CameraFollower cameraFollower;

    // Contains a key value for filling the values in the inspector.
    [Serializable]
    private class KeyValuePair
    {
        public GameObject _enemy;
        public Vector2 _pos;
    }

    // The list to be filled in the inspector.
    [SerializeField] private List<KeyValuePair> enemyList = new List<KeyValuePair>();

    // Called when object is created.
    private void Awake()
    {
        this.cameraFollower = FindObjectOfType<CameraFollower>();

        this.cameraBounder = GetComponentInChildren<CameraBounder>();
        this.cameraBounder.OnPlayerEnter += this.PlayerEnter; 
    }

    // Called when this room's camera bounder has been entered.
    private void PlayerEnter()
    {
        this.cameraFollower.ChangeBounds(cameraBounder.transform);
        this.SpawnEnemies();
    }

    // Spawns all enemies.
    private void SpawnEnemies()
    {
        foreach (var pair in  enemyList)
        {
            GameObject _e = Instantiate(pair._enemy);
            _e.transform.position = pair._pos + (Vector2)this.transform.position;
        }
    }

}
