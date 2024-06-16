using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapSize;
    [SerializeField] private int numberOfTiles = 6;
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;
    [SerializeField] private GameObject roomPrefab;
    private int furthestRoomIndex = -1;

    private int[] roomTiles; // Work in a 1D array so each room has a unique single-integer ID
    private List<int> roomTilesList; // Stores indices of all tiles that have been chosen to become rooms

    private void Awake()
    {
        this.roomTiles = new int[mapSize * mapSize];
        roomTilesList = new List<int>();

        if (numberOfTiles > mapSize * mapSize) return; // Ensure numberOfTiles is valid
        InitRoomTiles();

        this.furthestRoomIndex = GetFurthestRoomIndex();

        DrawRoomTiles();
    }

    private void InitRoomTiles()
    {
        int placedTileCount = 0;

        int startingRoomIndex = this.Convert2DTo1DIndex(mapSize / 2, mapSize / 2);
        roomTiles[startingRoomIndex] = 1;
        roomTilesList.Add(startingRoomIndex);
        placedTileCount++;

        while (placedTileCount < numberOfTiles) // Change to < to ensure we place the correct number of tiles
        {
            PlaceRandomTile();
            placedTileCount++;
        }
    }

    private void PlaceRandomTile()
    {
        bool success = false;

        while (!success && roomTilesList.Count > 0)
        {
            // First, pick a random tile in the list of placed tiles
            //int existingTile = roomTilesList[UnityEngine.Random.Range(0, roomTilesList.Count)];
            int existingTile = FindLeastChosenRoom();

            // Choose a random direction to go in
            int[] directions = new int[] { 0, 1, 2, 3 };
            ShuffleArray(directions); // Shuffle directions to ensure randomness

            foreach (int dir in directions)
            {
                int newRoom = -1;
                switch (dir)
                {
                    case 0: // Check up
                        newRoom = existingTile - mapSize;
                        break;
                    case 1: // Check down
                        newRoom = existingTile + mapSize;
                        break;
                    case 2: // Check left
                        newRoom = existingTile - 1;
                        // Check for left boundary
                        if (existingTile % mapSize == 0)
                            newRoom = -1;
                        break;
                    case 3: // Check right
                        newRoom = existingTile + 1;
                        // Check for right boundary
                        if (existingTile % mapSize == mapSize - 1)
                            newRoom = -1;
                        break;
                }

                if (newRoom >= 0 && newRoom < mapSize * mapSize && roomTiles[newRoom] == 0)
                {
                    roomTiles[existingTile]++;
                    roomTiles[newRoom] = 1;
                    roomTilesList.Add(newRoom);
                    success = true;

                    // stop from being able to choose this tile
                    if (!CanPlaceRoomNextTo(existingTile))
                        roomTiles[existingTile] = int.MaxValue;

                    break;
                }
            }
        }
    }

    private int FindLeastChosenRoom()
    {
        int min = int.MaxValue;
        int chosen = -1;

        foreach (int room in roomTilesList)
        {
            if (roomTiles[room] < min && CanPlaceRoomNextTo(room))
            {
                min = roomTiles[room];
                chosen = room;
            }
        }

        return chosen;
    }

    private bool CanPlaceRoomNextTo(int index)
    {
        int[] directions = { -mapSize, mapSize, -1, 1 };

        foreach (int direction in directions)
        {
            int newRoom = index + direction;
            if (newRoom >= 0 && newRoom < mapSize * mapSize && roomTiles[newRoom] == 0)
            {
                // Check for left and right boundary
                if ((direction == -1 && index % mapSize == 0) || (direction == 1 && index % mapSize == mapSize - 1))
                {
                    continue;
                }
                return true;
            }
        }
        return false;
    }

    private void DrawRoomTiles()
    {
        foreach (var room in roomTilesList)
        {
            var x = Instantiate(roomPrefab, this.transform);
            x.transform.position = ConvertToWorldPosition(room);


            // determine where neighboring rooms are
            bool up = room - mapSize > 0 && roomTiles[room - mapSize] != 0;
            bool down = room + mapSize < roomTiles.Length && roomTiles[room + mapSize] != 0;
            bool left = room - 1 > 0 && roomTiles[room - 1] != 0;
            bool right = room + 1 < roomTiles.Length && roomTiles[room + 1] != 0;

            int numTiles = roomWidth * roomHeight - (roomHeight * 2 + roomWidth * 2);

            x.GetComponent<DungeonGenerator>().
                Generate(UnityEngine.Random.Range((int)(numTiles * 0.6f), (int)(numTiles * 0.8f)), up, down, left, right);

            if (room == furthestRoomIndex)
            {
                x.GetComponent<DungeonGenerator>().PlaceExit();
            }
        }
    }

    private int GetFurthestRoomIndex()
    {
        Vector2 centerPosition = new Vector2(mapSize / 2, mapSize / 2);
        int furthestRoomIndex = -1;
        float maxDistance = float.MinValue;

        foreach (var roomIndex in roomTilesList)
        {
            Vector2 roomPosition = Convert1DTo2DIndex(roomIndex);
            float distance = Vector2.Distance(centerPosition, roomPosition);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoomIndex = roomIndex;
            }
        }

        return furthestRoomIndex;
    }

    #region UTILITY

    private int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.mapSize + column;
    }

    private Vector2 Convert1DTo2DIndex(int index)
    {
        int row = index / mapSize;
        int column = index % mapSize;
        return new Vector2(row, column);
    }

    private Vector2 ConvertToWorldPosition(int index)
    {
        Vector2 gridPosition = Convert1DTo2DIndex(index);
        float x = roomWidth *  (gridPosition.y - mapSize / 2f);
        float y = roomHeight * (gridPosition.x - mapSize / 2f);
        return new Vector2(x, y);
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

    #endregion
}