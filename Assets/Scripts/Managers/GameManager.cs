using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region DATA

    // Ref to player inventory scriptable obj.
    [SerializeField] private InventoryData playerInventory;

    #endregion

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
        uiManager = GetComponentInChildren<UIManager>();
        levelManager = GetComponentInChildren<LevelManager>();
        inventoryManager = GetComponentInChildren<InventoryManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        settingsManager = GetComponentInChildren<SettingsManager>();
        savingManager = GetComponentInChildren<SavingManager>();
    }

    // Gives the managers the information they need to do their job.
    private void InitManagers()
    { 
        // give references to the player's inventory.
        uiManager.PlayerInventory = playerInventory;
        inventoryManager.PlayerInventory = playerInventory;
    }

    // Subscribes to all necessary manager events.
    private void SubscribeToEvents()
    {
        timeManager.OnPause += this.OnPause;
    }

    // Called once when object is created.
    private void Awake()
    {
        
    }

    // Called on first frame of gameplay.
    private void Start()
    {
        FindManagers();
        SubscribeToEvents();
        InitManagers();        
    }

    // Called when the time manager detected the game was paused/unpaused.
    private void OnPause()
    {
        Debug.Log("game paused");
        uiManager.GamePaused(timeManager.IsPaused);
    }

}
