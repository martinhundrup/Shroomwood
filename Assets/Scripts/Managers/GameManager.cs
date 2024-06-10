using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application
{

    #region APPLICATION_WIDE

    // Self defined mutable tuple.
    public class Pair<T1, T2>
    {
        public Pair(T1 _first, T2 _second)
        {
            First = _first;
            Second = _second;
        }

        [SerializeField] public T1 First { get; set; }
        [SerializeField] public T2 Second { get; set; }
    }

    #endregion

    public class GameManager : MonoBehaviour
    {
        public delegate void GamePausedEvent(bool _isPaused);
        public GamePausedEvent OnGamePaused;

        public delegate void SceneLoadedEvent(int _gameLevel);
        public SceneLoadedEvent OnSceneLoaded;


        #region DATA

        // Ref to player inventory scriptable obj.
        [SerializeField] private InventoryData playerInventory;

        // Ref to the player data.
        [SerializeField] private PlayerData playerData;

        [SerializeField] private int gameLevel; // used as a seed for random generation - increases with every floor.

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

        public int GameLevel
        {
            get { return this.gameLevel; }
        }

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
            //uiManager.PlayerInventory = playerInventory;
            inventoryManager.PlayerInventory = playerInventory;
        }

        // Subscribes to all necessary manager events.
        private void SubscribeToEvents()
        {
            timeManager.OnPause += this.OnPause;
            playerData.OnHealthChanged += this.CheckPlayerHealth;
        }

        // Called once when object is created.
        private void Awake()
        {
            // singleton
            GameManager[] objs = FindObjectsOfType<GameManager>();
            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);

            this.playerData.ResetHealth();
        }

        // loads the game from the start
        public void LoadGame()
        {
            this.gameLevel = 0;
            SceneManager.LoadScene(1);
            if (this.OnSceneLoaded != null)
                this.OnSceneLoaded(gameLevel);
        }

        public void NextLevel()
        {
            this.gameLevel++;
            SceneManager.LoadScene(1);
            if (this.OnSceneLoaded != null)
                this.OnSceneLoaded(gameLevel);
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
            if (this.OnGamePaused != null)
                this.OnGamePaused(this.timeManager.IsPaused);
        }

        private void CheckPlayerHealth()
        {
            Debug.Log("Player took damage");
            if (playerData.CurrentHealth <= 0) // player had died
            {
                Debug.Log("Player has died");
            }
        }

    }
}
