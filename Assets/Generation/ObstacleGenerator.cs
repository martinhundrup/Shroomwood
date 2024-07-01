using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName ="Generators/Obstacle Generator")]
public class ObstacleGenerator : ScriptableObject
{
    [SerializeField] private float threshold;
    [SerializeField] private float scale;
    [SerializeField] protected GameObject obstaclePrefab;
    [SerializeField] private GameObject decorPrefab;
    protected int roomWidth;
    protected int roomHeight;
    protected int index;

    public int Index
    {
        get { return index; }
    }
    public GameObject ObstaclePrefab
    {
        get { return obstaclePrefab; }
    }
    public GameObject DecorPrefab
    {
        get { return decorPrefab; }
    }


    virtual public void PlaceObstacles(int[] obstacleTiles, int _index)
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
                if (obstacleTiles[i] == 0) // if empty tile
                {
                    float perlinValue = Mathf.PerlinNoise((x * scale) + offsetX, (y * scale) + offsetY);
                    if (perlinValue > threshold) // Adjust the threshold as needed
                    {
                        obstacleTiles[i] = index;
                    }
                }
            }
        }
    }

    protected int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.roomWidth + column;
    }
}
