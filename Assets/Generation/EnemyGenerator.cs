using Sirenix.OdinInspector.Editor.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable, CreateAssetMenu(menuName = "Generators/Enemy Generator")]
public class EnemyGenerator : ScriptableObject
{
    public delegate void SpawnEnemy(int index, GameObject enemy);
    public event SpawnEnemy OnSpawnEnemy;

    [SerializeField] private List<GameObject> enemyPrefabs;
    private float balance;
    protected int roomWidth;
    protected int roomHeight;
    protected int index;
    public int Index
    {
        get { return index; }
    }

    // finds the amount of points the room has to spend on enemies
    private void CalculateBalance()
    {
        int gameLevel = DataDictionary.GameSettings.GameLevel;
        balance = 2 + gameLevel * 2 + UnityEngine.Random.Range(-gameLevel, gameLevel);
    }

    private GameObject GetRandomEnemy()
    {
        if (balance <= 0) return null; // we don't need more enemies

        else
        {
            var enemy = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
            balance -= enemy.GetComponent<Enemy>().Cost;
            return enemy;
        }
    }

    public void PlaceEnemies(int[] obstacleTiles, int[] wallTiles, int _index)
    {
        CalculateBalance();
        index = _index;
        

        roomHeight = DataDictionary.GameSettings.RoomSize.x;
        roomWidth = DataDictionary.GameSettings.RoomSize.y;

        float offsetX = UnityEngine.Random.Range(0f, 100f);
        float offsetY = UnityEngine.Random.Range(0f, 100f);

        List<int> availableTiles = new List<int>();

        for (int y = 0; y < roomHeight; y++)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                int i = Convert2DTo1DIndex(y, x);
                if (obstacleTiles[i] == 0 && wallTiles[i] != 0) // if empty tile
                {
                    //Debug.Log($"Added to available tiles value:{roomTiles[i]}");
                    availableTiles.Add(i);
                }
            }
        }
        Debug.Log($"balance: {balance}");
        var enemy = GetRandomEnemy();
        int enemyCount = 0;
        while (enemy != null)
        {
            Debug.Log("Place enemy");
            int i = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
            //roomTiles[i] = index;
            availableTiles.Remove(i);

            if (OnSpawnEnemy != null)
                OnSpawnEnemy(i, enemy);

            enemy = GetRandomEnemy();
            enemyCount++;
        }
        Debug.Log($"enemy count {enemyCount}");
    }

    private Vector2 ConvertToWorldPosition(int index)
    {
        Vector2 gridPosition = Convert1DTo2DIndex(index);
        float x = gridPosition.y - roomWidth / 2f - 0.5f;
        float y = gridPosition.x - roomHeight / 2f - 0.5f;
        return new Vector2(x, y);
    }

    private Vector2 Convert1DTo2DIndex(int index)
    {
        int row = index / roomWidth;
        int column = index % roomWidth;
        return new Vector2(row, column);
    }

    protected int Convert2DTo1DIndex(int row, int column)
    {
        return row * this.roomWidth + column;
    }
}
