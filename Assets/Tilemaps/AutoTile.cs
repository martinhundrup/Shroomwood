using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
// Dynaimcally draws floor tiles when the player enters a room
public class AutoTile : MonoBehaviour
{
    [SerializeField] private RuleTile tile;
    private Tilemap tilemap;
    int roomWidth;
    int roomHeight;


    private void Awake()
    {
        roomWidth = DataDictionary.GameSettings.RoomSize.x;
        roomHeight = DataDictionary.GameSettings.RoomSize.y;
        tilemap = GetComponent<Tilemap>();
        GameEvents.OnPlayerEnterRoom += DrawTiles;
    }

  private void DrawTiles(Transform bounder)
    {
        // Calculate the bottom left corner position of the room
        int startX = -1 +  Mathf.FloorToInt(bounder.position.x - (bounder.localScale.x / 2));
        int startY = -1 + Mathf.FloorToInt(bounder.position.y - (bounder.localScale.y / 2));

        // Draw tiles
        for (int i = 0; i <= 1 + roomWidth * 2; i++) // Multiply by 2 because of tile size 0.5x
        {
            for (int j = 0; j <= 1 + roomHeight * 2; j++) // Multiply by 2 because of tile size 0.5x
            {
                Vector3Int tilePosition = new Vector3Int(startX * 2 + i, startY * 2 + j, 0);
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }
}
