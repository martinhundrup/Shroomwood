using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    private int roomWidth = 21;
    private int roomHeight = 13;
    [SerializeField] private int numberOfTiles = 100;
    private Tilemap topTilemap;
    private Tilemap wallTilemap;
    private Tilemap shadowTilemap;
    [SerializeField] private RuleTile topTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private RuleTile shadowTile;
    [SerializeField] private string topTilemapTag;
    [SerializeField] private string wallTilemapTag;
    [SerializeField] private string shadowTilemapTag;
    private int borderWidth;
    private CameraBounder cameraBounder;
    [SerializeField] private List<ObstacleGenerator> obstacles;
    [SerializeField] private List<EnemyGenerator> enemies;

    private int[] roomTiles; // Work in a 1D array so each room has a unique single-integer ID
    private int[] obstacleTiles; // Work in a 1D array so each room has a unique single-integer ID
    private List<int> roomTilesList; // Stores indices of all tiles that have been chosen to become rooms
    [SerializeField] private GameObject dungeonExit;

    private void Awake()
    {
        borderWidth = DataDictionary.GameSettings.RoomBorder;
        roomWidth = DataDictionary.GameSettings.RoomSize.x;
        roomHeight = DataDictionary.GameSettings.RoomSize.y;
        topTilemap = GameObject.FindGameObjectWithTag(topTilemapTag).GetComponent<Tilemap>();
        wallTilemap = GameObject.FindGameObjectWithTag(wallTilemapTag).GetComponent<Tilemap>();
        shadowTilemap = GameObject.FindGameObjectWithTag(shadowTilemapTag).GetComponent<Tilemap>();
        roomTiles = new int[roomWidth * roomHeight];
        obstacleTiles = new int[roomWidth * roomHeight];
        roomTilesList = new List<int>();

        //Generate(numberOfTiles, true, true, true, true);
    }

    public void Generate(int _numTiles, bool up, bool down, bool left, bool right)
    {
        this.numberOfTiles = _numTiles;
        if (numberOfTiles > (roomWidth - (roomWidth * 2)) * (roomHeight - (roomWidth * 2))) return; // Ensure numberOfTiles is valid
        InitRoomTiles();

        // place any obstacles
        PlaceObstacles();
        PlaceEnemies();

        SetTopDoorway(up);
        SetRightDoorway(right);
        SetBottomDoorway(down);
        SetLeftDoorway(left);

        DrawRoomTiles();
        DrawObstacles();

        cameraBounder = new GameObject().AddComponent<CameraBounder>();
        cameraBounder.transform.parent = this.transform;
        cameraBounder.transform.localPosition = Vector2.zero + new Vector2(0f, -0.5f);
        cameraBounder.transform.localScale = new Vector2(roomWidth - 1, roomHeight - 1);
    }

    private void InitRoomTiles()
    {
        int placedTileCount = 0;

        int startingRoomIndex = this.Convert2DTo1DIndex(roomHeight / 2, roomWidth / 2);
        roomTiles[startingRoomIndex] = 1;
        roomTilesList.Add(startingRoomIndex);
        placedTileCount++;

        while (placedTileCount < numberOfTiles) // Change to < to ensure we place the correct number of tiles
        {
            PlaceRandomTile();
            placedTileCount++;
        }
    }

    private void PlaceObstacles()
    {
        if (obstacles == null || obstacles.Count == 0) return;
        int index = 1;

        foreach (var obstacle in obstacles)
        {
            int i = index++;
            obstacle.PlaceObstacles(obstacleTiles, i);
        }
    }

    private void PlaceEnemies()
    {
        if (enemies == null || enemies.Count == 0) return;
        int index = 100;

        foreach (var enemy in enemies)
        {
            enemy.OnSpawnEnemy += DrawEnemy;
            int i = index++;
            enemy.PlaceEnemies(obstacleTiles, roomTiles, i);
            enemy.OnSpawnEnemy -= DrawEnemy;
        }
    }

    private void DrawEnemy(int i, GameObject enemy)
    {
        Debug.Log("draw enemy");

        int x = (int)ConvertToWorldPosition(i).x;
        int y = (int)ConvertToWorldPosition(i).y;

        var ob = Instantiate(enemy, transform);
        ob.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);
    }

    private void SetTopDoorway(bool doorway)
    {
        if (doorway)
        {
            bool done = false;
            int index = roomWidth / 2;
            while (!done)
            {
                if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                roomTiles[index] = 1;
                obstacleTiles[index] = 0;
                index += roomWidth;
                //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                done = roomTiles[index] != 0;
            }
        }
        else
        {
            for (int i = 0; i < roomWidth; i++)
            {
                int x = (int)ConvertToWorldPosition(i).x + (int)this.transform.localPosition.x;
                int y = (int)ConvertToWorldPosition(i).y + (int)this.transform.localPosition.y - 1;

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                topTilemap.SetTile(tilePosition, topTile);
            }
        }
    }

    private void SetBottomDoorway(bool doorway)
    {
        if (doorway)
        {
            bool done = false;
            int index = roomTiles.Length - 1 - roomWidth / 2;
            while (!done)
            {
                if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                roomTiles[index] = 1;
                obstacleTiles[index] = 0;
                index -= roomWidth;
                //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                done = roomTiles[index] != 0;
            }
        }
        else
        {
            for (int i = (roomWidth * roomHeight - roomWidth); i < roomWidth * roomHeight; i++)
            {
                int x = (int)ConvertToWorldPosition(i).x + (int)this.transform.localPosition.x;
                int y = (int)ConvertToWorldPosition(i).y + (int)this.transform.localPosition.y + 1;

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                topTilemap.SetTile(tilePosition, topTile);
            }
        }
    }

    private void SetLeftDoorway(bool doorway)
    {
        if (doorway)
        {
            bool done = false;
            int index = roomWidth * (roomHeight / 2);
            while (!done)
            {
                if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                roomTiles[index] = 1;
                obstacleTiles[index] = 0;
                index++;
                //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                done = roomTiles[index] != 0;
            }
        }
        else
        {
            for (int i = 0; i < roomHeight * roomWidth; i += roomWidth)
            {
                int x = (int)ConvertToWorldPosition(i).x + (int)this.transform.localPosition.x - 1;
                int y = (int)ConvertToWorldPosition(i).y + (int)this.transform.localPosition.y;

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                topTilemap.SetTile(tilePosition, topTile);
            }
        }
    }

    private void SetRightDoorway(bool doorway)
    {
        if (doorway)
        {
            bool done = false;
            int index = roomWidth * (1 + roomHeight / 2) - 1;
            while (!done)
            {
                if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                roomTiles[index] = 1;
                obstacleTiles[index] = 0;
                index--;
                //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
                done = roomTiles[index] != 0;
            }
        }
        else
        {
            for (int i = roomWidth - 1; i < roomHeight * roomWidth; i += roomWidth)
            {
                int x = (int)ConvertToWorldPosition(i).x + (int)this.transform.localPosition.x + 1;
                int y = (int)ConvertToWorldPosition(i).y + (int)this.transform.localPosition.y;

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                topTilemap.SetTile(tilePosition, topTile);
            }
        }
    }

    public void PlaceExit()
    {
        Instantiate(dungeonExit, this.transform);
    }

    private void PlaceRandomTile()
    {
        bool success = false;

        while (!success && roomTilesList.Count > 0)
        {
            // First, pick a random tile in the list of placed tiles
            int existingTile = roomTilesList[UnityEngine.Random.Range(0, roomTilesList.Count)];

            // Choose a random direction to go in
            int[] directions = new int[] { 0, 1, 2, 3 };
            ShuffleArray(directions); // Shuffle directions to ensure randomness

            foreach (int dir in directions)
            {
                int newRoom = -1;
                switch (dir)
                {
                    case 0: // Check up
                        newRoom = existingTile - roomWidth;
                        break;
                    case 1: // Check down
                        newRoom = existingTile + roomWidth;
                        break;
                    case 2: // Check left
                        newRoom = existingTile - 1;
                        // Check for left boundary
                        if (existingTile % roomWidth == 0)
                            newRoom = -1;
                        break;
                    case 3: // Check right
                        newRoom = existingTile + 1;
                        // Check for right boundary
                        if (existingTile % roomWidth == roomWidth - 1)
                            newRoom = -1;
                        break;
                }

                // Ensure newRoom is not on the border
                if (newRoom >= roomWidth * borderWidth && newRoom < roomWidth * (roomHeight - borderWidth) &&
                    newRoom % roomWidth >= borderWidth && newRoom % roomWidth < roomWidth - borderWidth &&
                    roomTiles[newRoom] == 0)
                {
                    roomTiles[newRoom] = 1;
                    roomTilesList.Add(newRoom);
                    //Debug.Log($"existing room: {existingTile}, new room: {newRoom}");
                    success = true;
                    break;
                }
            }
        }
    }

    // Utility function to shuffle an array
    private void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    private void DrawRoomTiles()
    {
        for (int i = 0; i < roomTiles.Length; i++)
        {
            if (roomTiles[i] == 0)
            {
                int x = (int)ConvertToWorldPosition(i).x + (int)this.transform.localPosition.x;
                int y = (int)ConvertToWorldPosition(i).y + (int)this.transform.localPosition.y;

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                topTilemap.SetTile(tilePosition, topTile);
                wallTilemap.SetTile(tilePosition, wallTile);
                shadowTilemap.SetTile(tilePosition, shadowTile);
            }
        }
    }

    private void DrawObstacles()
    {
        for (int i = 0; i < obstacleTiles.Length; i++)
        {
            if (obstacleTiles[i] != 0) // don't place out of bounds
            {
                int x = (int)ConvertToWorldPosition(i).x;
                int y = (int)ConvertToWorldPosition(i).y;

                if (roomTiles[i] == 0)
                {
                    if (obstacles[obstacleTiles[i] - 1].DecorPrefab != null)
                    {
                        var ob = Instantiate(obstacles[obstacleTiles[i] - 1].DecorPrefab, transform);
                        ob.transform.localPosition = new Vector2(x + 0.5f, y + 1f);
                    }
                }
                else
                {
                    var ob = Instantiate(obstacles[obstacleTiles[i] - 1].ObstaclePrefab, transform);
                    ob.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);
                }
            }
        }
    }

    #region UTILITY

    private int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.roomWidth + column;
    }

    private Vector2 Convert1DTo2DIndex(int index)
    {
        int row = index / roomWidth;
        int column = index % roomWidth;
        return new Vector2(row, column);
    }

    private Vector2 ConvertToWorldPosition(int index)
    {
        Vector2 gridPosition = Convert1DTo2DIndex(index);
        float x = gridPosition.y - roomWidth / 2f - 0.5f;
        float y = gridPosition.x - roomHeight / 2f - 0.5f;
        return new Vector2(x, y);
    }

    #endregion
}