using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Application;
using System.Runtime.CompilerServices;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject door;

    // Reference to the camera bounder child.
    private CameraBounder cameraBounder;

    // Reference to camera.
    private CameraFollower cameraFollower;

    // Whether or not the enemies have been spawned already.
    private bool hasSpawnedEnemies;

    // The amount of enemies in the room.
    private int enemyCount = 0;

    // Refs to the doors to close when player enters.
    private Doorway[] doors;

    private Enemy[] enemies;

    // Called when object is created.
    private void Awake()
    {
        this.cameraFollower = FindObjectOfType<CameraFollower>();

        this.cameraBounder = GetComponentInChildren<CameraBounder>();
        this.cameraBounder.OnPlayerEnter += this.PlayerEnter;

        InitWallTiles();
        
        this.InitEnemies();
        
    }

    private void InitWallTiles()
    {
        if (GetComponentsInChildren<TileGenerator>() != null)
            foreach (var wall in GetComponentsInChildren<TileGenerator>())
            {
                wall.DrawTiles();
            }
    }

    // Generates either a solid wall, or a wall with a door depending on what's next to it.
    public void GenerateWalls(bool _up, bool _down, bool _left, bool _right) // which directions to generate rooms, rest are walls
    {
        

        if (_up)
        {
            GameObject _l = Instantiate(wall, this.transform);
            _l.transform.localPosition = new Vector3(-width / 4, height / 2 - 0.5f, 0f);
            _l.transform.localScale = new Vector3(width / 2 - 2, 1f, 1f);
            _l.GetComponent<TileGenerator>().DrawTiles();

            GameObject _r = Instantiate(wall, this.transform);
            _r.transform.localPosition = new Vector3(width / 4, height / 2 - 0.5f, 0f);
            _r.transform.localScale = new Vector3(width / 2 - 2, 1f, 1f);
            _r.GetComponent<TileGenerator>().DrawTiles();

            GameObject _d = Instantiate(door, this.transform);
            _d.transform.localPosition = new Vector3(0f, height / 2 - 0.5f, 0f);
            _d.transform.localScale = new Vector3(2f, 1f, 1f);
        }
        else
        {
            GameObject _w = Instantiate(wall, this.transform);
            _w.transform.localPosition = new Vector3(0f, height / 2 - 0.5f, 0f);
            _w.transform.localScale = new Vector3(width, 1f, 1f);
            _w.GetComponent<TileGenerator>().DrawTiles();
        }

        if (_down) {

            GameObject _l = Instantiate(wall, this.transform);
            _l.transform.localPosition = new Vector3(-width / 4, -height / 2 + 0.5f, 0f);
            _l.transform.localScale = new Vector3(width / 2 - 2, 1f, 1f);
            _l.GetComponent<TileGenerator>().DrawTiles();

            GameObject _r = Instantiate(wall, this.transform);
            _r.transform.localPosition = new Vector3(width / 4, -height / 2 + 0.5f, 0f);
            _r.transform.localScale = new Vector3(width / 2 - 2, 1f, 1f);
            _r.GetComponent<TileGenerator>().DrawTiles();

            GameObject _d = Instantiate(door, this.transform);
            _d.transform.localPosition = new Vector3(0f, -height / 2 + 0.5f, 0f);
            _d.transform.localScale = new Vector3(2f, 1f, 1f);
        }
        else
        {
            GameObject _w = Instantiate(wall, this.transform);
            _w.transform.localPosition = new Vector3(0f, -height / 2 + 0.5f, 0f);
            _w.transform.localScale = new Vector3(width, 1f, 1f);
            _w.GetComponent<TileGenerator>().DrawTiles();
        }

        if (_left) {

            GameObject _t = Instantiate(wall, this.transform);
            _t.transform.localPosition = new Vector3(-width / 2 + 0.5f, height / 4 + 0.5f, 0f);
            _t.transform.localScale = new Vector3(1f, height / 2 - 1, 1f);
            _t.GetComponent<TileGenerator>().DrawTiles();

            GameObject _b = Instantiate(wall, this.transform);
            _b.transform.localPosition = new Vector3(-width / 2 + 0.5f, -height / 4 - 0.5f, 0f);
            _b.transform.localScale = new Vector3(1f, height / 2 - 1, 1f);
            _b.GetComponent<TileGenerator>().DrawTiles();

            GameObject _d = Instantiate(door, this.transform);
            _d.transform.localPosition = new Vector3(-width / 2 + 0.5f, 0f, 0f);
            _d.transform.localScale = new Vector3(1f, 2f, 1f);
        }
        else
        {
            GameObject _w = Instantiate(wall, this.transform);
            _w.transform.localPosition = new Vector3(-width / 2 + 0.5f, 0f, 0f);
            _w.transform.localScale = new Vector3(1f, height, 1f);
            _w.GetComponent<TileGenerator>().DrawTiles();
        }

        if (_right) {
            GameObject _t = Instantiate(wall, this.transform);
            _t.transform.localPosition = new Vector3(width / 2 - 0.5f, height / 4 + 0.5f, 0f);
            _t.transform.localScale = new Vector3(1f, height / 2 - 1, 1f);
            _t.GetComponent<TileGenerator>().DrawTiles();

            GameObject _b = Instantiate(wall, this.transform);
            _b.transform.localPosition = new Vector3(width / 2 - 0.5f, -height / 4 - 0.5f, 0f);
            _b.transform.localScale = new Vector3(1f, height / 2 - 1, 1f);
            _b.GetComponent<TileGenerator>().DrawTiles();

            GameObject _d = Instantiate(door, this.transform);
            _d.transform.localPosition = new Vector3(width / 2 - 0.5f, 0f, 0f);
            _d.transform.localScale = new Vector3(1f, 2f, 1f);
        }
        else
        {
            GameObject _w = Instantiate(wall, this.transform);
            _w.transform.localPosition = new Vector3(width / 2 - 0.5f, 0f, 0f);
            _w.transform.localScale = new Vector3(1f, height, 1f);
            _w.GetComponent<TileGenerator>().DrawTiles();
        }

        this.doors = GetComponentsInChildren<Doorway>();
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
