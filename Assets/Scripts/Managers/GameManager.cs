using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region MICRO MANAGERS

    // The Time Manager.
    private TimeManager timeManager;

    // The UI Manager.
    private UIManager uiManager;

    // The Level Manager.
    private LevelManager levelManager;

    // The Inventory Manager.
    private InventoryManager inventoryManager;

    // The Audio Manager.
    private AudioManager audioManager;

    // The Settings Manager.
    private SettingsManager settingsManager;

    // The Saving Manager
    private SavingManager savingManager;

    #endregion

    // Initializes all child manager components.
    private void FindManagers()
    {
        timeManager = GetComponentInChildren<TimeManager>();
        uiManager = GetComponent<UIManager>();
        levelManager = GetComponent<LevelManager>();
        inventoryManager = GetComponentInChildren<InventoryManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        settingsManager = GetComponentInChildren<SettingsManager>();
        savingManager = GetComponentInChildren<SavingManager>();
    }

    // Subscribes to all necessary manager events.
    private void SubscribeToEvents()
    {
        timeManager.OnPause += this.OnPause;
    }

    // Called once when object is created.
    private void Awake()
    {
        FindManagers();
        SubscribeToEvents();

    }

    private void OnPause(bool pause)
    {
        
    }

}
