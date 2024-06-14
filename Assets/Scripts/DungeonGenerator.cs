using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth = 20;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] private int numberOfTiles = 100;
    private Tilemap tilemap;
    [SerializeField] private RuleTile ruleTile;

    private int[] roomTiles; // Work in a 1D array so each room has a unique single-integer ID
    private List<int> roomTilesList; // Stores indices of all tiles that have been chosen to become rooms
    [SerializeField] private GameObject dungeonExit;

    private void Awake()
    {
        this.tilemap = GameObject.FindGameObjectWithTag("Meadow Tilemap").GetComponent<Tilemap>();
        this.roomTiles = new int[mapWidth * mapHeight];
        roomTilesList = new List<int>();
    }

    public void Generate(int _numTiles, bool up, bool down, bool left, bool right)
    {
        this.numberOfTiles = _numTiles;
        if (numberOfTiles > (mapWidth - 2) * (mapHeight - 2)) return; // Ensure numberOfTiles is valid
        InitRoomTiles();

        if (up) SetBottomDoorway();
        if (right) SetRightDoorway();
        if (down) SetTopDoorway();
        if (left) SetLeftDoorway();
        DrawRoomTiles();
    }

    private void InitRoomTiles()
    {
        int placedTileCount = 0;

        int startingRoomIndex = this.Convert2DTo1DIndex(mapHeight / 2, mapWidth / 2);
        roomTiles[startingRoomIndex] = 1;
        roomTilesList.Add(startingRoomIndex);
        placedTileCount++;

        while (placedTileCount < numberOfTiles) // Change to < to ensure we place the correct number of tiles
        {
            PlaceRandomTile();
            placedTileCount++;
        }
    }

    private void SetBottomDoorway()
    {
        bool done = false;
        int index = mapWidth / 2;
        while (!done)
        {
            if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            roomTiles[index] = 1;
            index += mapWidth;
            //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            done = roomTiles[index] != 0;
        }
    }

    private void SetTopDoorway()
    {
        bool done = false;
        int index = roomTiles.Length - 1 - mapWidth / 2;
        while (!done)
        {
            if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            roomTiles[index] = 1;
            index -= mapWidth;
            //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            done = roomTiles[index] != 0;
        }
    }

    private void SetLeftDoorway()
    {
        bool done = false;
        int index = mapWidth * (mapHeight / 2);
        while (!done)
        {
            if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            roomTiles[index] = 1;
            index++;
            //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            done = roomTiles[index] != 0;
        }
    }

    private void SetRightDoorway()
    {
        bool done = false;
        int index = mapWidth * (1 + mapHeight / 2) - 1;
        while (!done)
        {
            if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            roomTiles[index] = 1;
            index--;
            //if (roomTilesList.Contains(index)) roomTilesList.Remove(index);
            done = roomTiles[index] != 0;
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
                        newRoom = existingTile - mapWidth;
                        break;
                    case 1: // Check down
                        newRoom = existingTile + mapWidth;
                        break;
                    case 2: // Check left
                        newRoom = existingTile - 1;
                        // Check for left boundary
                        if (existingTile % mapWidth == 0)
                            newRoom = -1;
                        break;
                    case 3: // Check right
                        newRoom = existingTile + 1;
                        // Check for right boundary
                        if (existingTile % mapWidth == mapWidth - 1)
                            newRoom = -1;
                        break;
                }

                // Ensure newRoom is not on the border
                if (newRoom >= mapWidth && newRoom < mapWidth * (mapHeight - 1) &&
                    newRoom % mapWidth != 0 && newRoom % mapWidth != mapWidth - 1 &&
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
                tilemap.SetTile(tilePosition, ruleTile);
            }
        }
    }

    #region UTILITY

    private int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.mapWidth + column;
    }

    private Vector2 Convert1DTo2DIndex(int index)
    {
        int row = index / mapWidth;
        int column = index % mapWidth;
        return new Vector2(row, column);
    }

    private Vector2 ConvertToWorldPosition(int index)
    {
        Vector2 gridPosition = Convert1DTo2DIndex(index);
        float x = gridPosition.y - mapWidth / 2f - 0.5f;
        float y = gridPosition.x - mapHeight / 2f - 0.5f;
        return new Vector2(x, y);
    }

    #endregion
}