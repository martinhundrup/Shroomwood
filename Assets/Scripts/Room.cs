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

    // Whether or not the enemies have been spawned already.
    private bool hasSpawnedEnemies;

    // The amount of enemies in the room.
    [SerializeField] private int enemyCount = 0;

    // Refs to the doors to close when player enters.
    private Doorway[] doors;

    private Enemy[] enemies;

    // Called when object is created.
    private void Awake()
    {
        this.cameraFollower = FindObjectOfType<CameraFollower>();

        this.cameraBounder = GetComponentInChildren<CameraBounder>();
        this.cameraBounder.OnPlayerEnter += this.PlayerEnter;

        this.doors = GetComponentsInChildren<Doorway>();
        this.InitEnemies();

        this.ActivateDoors(false);
    }

    // Called when this room's camera bounder has been entered.
    private void PlayerEnter()
    {
        this.cameraFollower.ChangeBounds(cameraBounder.transform);

        if (!hasSpawnedEnemies && enemyCount > 0)
        {
            this.ActivateEnemies(true);
            this.ActivateDoors(true);
            hasSpawnedEnemies = true;
        }
    }

    // Initailizes enemies at scene start.
    private void InitEnemies()
    {
        this.enemies = GetComponentsInChildren<Enemy>();
        this.enemyCount = this.enemies.Length;

        foreach (var _enemy in this.enemies)
        {
            _enemy.gameObject.SetActive(false);
            _enemy.OnDeath += this.EnemyDeath;
        }
    }

    // Activates or deactives all doors in the room.
    private void ActivateDoors(bool _bool)
    {
        foreach (var _door in this.doors)
        {
            _door.gameObject.SetActive(_bool);
        }
    }

    // Spawns all enemies.
    private void ActivateEnemies(bool _bool)
    {
        foreach (var _enemy in this.enemies)
        {
            _enemy.gameObject.SetActive(_bool);
        }
    }

    // Called when an enemy of the room is killed.
    private void EnemyDeath()
    {
        Debug.Log("enemy died");
        enemyCount--;

        if (enemyCount <= 0)
            ActivateDoors(false);
    }

}
