using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu]
public class ObstacleGenerator : ScriptableObject
{
    [SerializeField] private float threshold;
    [SerializeField] private float scale;
    [SerializeField] private GameObject prefab;
    private int roomWidth;
    private int roomHeight;
    private int index;

    public int Index
    {
        get { return index; }
    }
    public GameObject Prefab
    {
        get { return prefab; }
    }

    public void PlaceObstacles(int[] roomTiles, int _index)
    {
        index = _index;

        roomHeight = DataDictionary.GameSettings.RoomSize.x;
        roomWidth = DataDictionary.GameSettings.RoomSize.y;

        float offsetX = UnityEngine.Random.Range(0f, 100f);
        float offsetY = UnityEngine.Random.Range(0f, 100f);

        for (int y = 0; y < roomHeight; y++)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                int i = Convert2DTo1DIndex(y, x);
                if (roomTiles[i] == 1) // if empty tile
                {
                    float perlinValue = Mathf.PerlinNoise((x * scale) + offsetX, (y * scale) + offsetY);
                    if (perlinValue > threshold) // Adjust the threshold as needed
                    {
                        roomTiles[i] = index;
                    }
                }
            }
        }
    }

    private int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.roomWidth + column;
    }
}
