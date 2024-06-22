using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDictionary : MonoBehaviour
{

    private static DataDictionary instance;
    private PlayerStats playerStats;
    private GameSettings gameSettings;

    public static PlayerStats PlayerStats
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("DataDictionaryManager").AddComponent<DataDictionary>();
                instance.Initialize();
            }
            return instance.playerStats;
        }
    }

    public static GameSettings GameSettings
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("DataDictionaryManager").AddComponent<DataDictionary>();
                instance.Initialize();
            }
            return instance.gameSettings;
        }
    }

    private void Initialize()
    {
        playerStats = Resources.Load<PlayerStats>("ShroomieStats");
        gameSettings = Resources.Load<GameSettings>("GameSettings");
        if (playerStats == null)
        {
            Debug.LogError("MoveDictionaryData not found in Resources folder.");
        }
    }

}
