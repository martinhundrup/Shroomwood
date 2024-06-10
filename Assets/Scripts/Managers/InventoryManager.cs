using Application;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void CollectAction();

    // The event called when this object collides with a hitbox.
    public event CollectAction OnCollect;

    #endregion

    #region ATTRIBUTES

    [SerializeField] 

    // Holds reference to the instantiated player script.
    private PlayerController player;

    // The player's inventory scriptable object.
    private InventoryData playerInventory;

    // The array of item slots - generated at runtime
    private InventorySlot[] inventorySlots = new InventorySlot[17]; // items 15 and 16 are weapon slots 1 and 2

    private Canvas inventoryCanvas;
    private Tuple<ItemData, int> movingItemPair;
    [SerializeField] GameObject movingItemDisplay;
    GameObject activeMovingItem;

    private int initCounter = 0;

    // Gets or sets the player's inventory.
    public InventoryData PlayerInventory
    {
        get { return this.playerInventory; }
        set { this.playerInventory = value; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        GetComponentInParent<GameManager>().OnGamePaused += this.GamePaused;
        //GetComponentInParent<GameManager>().OnSceneLoaded += this.OnSceneReload;
        initCounter = 0;
        GamePaused(false);
    }

    #endregion

    #region METHODS

    public void OnSceneReload(int gameLevel)
    {
        this.player = FindObjectOfType<PlayerController>();
        this.inventoryCanvas = GameObject.FindGameObjectWithTag("Inventory Canvas").GetComponent<Canvas>();

        //Invoke("RefreshAllSlots", 0.1f);
        RefreshAllSlots();
    }

    // resets all inventory slots
    // ensures they are displaying up-to-date information
    private void RefreshAllSlots()
    {
        if (inventoryCanvas == null || playerInventory == null) return;
        for (int i = 0; i < 17; i++)
        {
            var data = playerInventory.GetItem(i);
            if (data != null)
            {
                inventorySlots[i].ItemData = data.Item1;
                inventorySlots[i].Amount = data.Item2;
            }
        }
    }


    // Subscribes to an ItemDrops OnCollect event.
    public void Subscribe(ItemDrop _item)
    {
        _item.OnCollect += ItemCollected;
    }

    // Adds an item to the inventory when collected.
    private void ItemCollected(ItemDrop _item)
    {
        //// check if weapon
        //if (_item is WeaponDrop weapon)
        //{
        //    // _item is indeed a WeaponData
        //    Debug.Log("Collected a weapon: " + weapon.ItemData.ItemName);

        //    player.EquipWeapon((WeaponDrop)_item);
        //}


        int _i = playerInventory.AddItem(_item.ItemData, 1);
        if (_i == -1)
        {
            Debug.Log(_item.ItemData.ItemName + " could not be collected");
        }
        else
        {
            inventorySlots[_i].ItemData = _item.ItemData;
            inventorySlots[_i].Amount = playerInventory.GetItem(_i).Item2;
            Debug.Log(_item.ItemData.ItemName + " was collected");

            // TODO: implement some sort of UI message when a item is collected.
            //this.OnCollect();
        }
    }

    public void SubscribeToInventorySlot(InventorySlot _slot)
    {
        _slot.OnInventorySlotClicked += this.OnInventorySlotPressed;
        inventorySlots[_slot.Index] = _slot;
    }

    //TODO:
    //  - if the player clicks a activeMovingItem on the outside,
    //      I should remove it from the inventory and instantiate that many item drops in the overworld
    //  - if the user clicks exit while trying to move an item, it should also be thrown on the ground
    //  - I need to start tracking where each item was grabbed from, so I can swap two items around

    private void OnInventorySlotPressed(int _index)
    {
        bool changedWeapons = false;
        if (movingItemPair == null) // no active item, so pick up whatever is in slot
        {
            movingItemPair = playerInventory.EmptySlot(_index); // attempt to empty the slot
            if (movingItemPair != null) // we clicked on a item, so pick it up
            {
                inventorySlots[_index].ItemData = null;
                activeMovingItem = Instantiate(movingItemDisplay, inventoryCanvas.transform);
                activeMovingItem.GetComponent<Image>().sprite = movingItemPair.Item1.Icon;
            }
        }
        else if (playerInventory.GetItem(_index) == null) // no item on slot clicked
        {
            if (movingItemPair != null) // we are trying to move something, so put it in the slot
            {
                // check to see if a weapon slot is trying to be put into,
                // ifso, make sure the item trying to be put in is also a weapon
                if (_index == 15 || _index == 16)
                {
                    if (movingItemPair.Item1.GetType() != typeof(WeaponData))
                    {
                        return; // not a weapon, don't put it in
                    }
                    else // player weapons were modified
                    {
                        changedWeapons = true;
                    }
                }

                playerInventory.SetSlot(_index, movingItemPair);
                inventorySlots[_index].ItemData = movingItemPair.Item1;
                inventorySlots[_index].Amount = movingItemPair.Item2;

                // cleanup
                movingItemPair = null;
                Destroy(activeMovingItem);
                activeMovingItem = null;

                if (changedWeapons)
                    UpdatePlayerWeapons();
            }
        }
    }

    private void UpdatePlayerWeapons()
    {
        var t = playerInventory.GetItem(15);
        if (t != null) player.EquipWeapon(t.Item1 as WeaponData);
        else player.EquipWeapon(null);
    }

    public void GamePaused(bool _isPaused)
    {
        if (initCounter > 0) // ensure it doesn't init every pause
        {
            OnSceneReload(1);
        }


        if (inventoryCanvas == null)
            inventoryCanvas = this.inventoryCanvas = GameObject.FindGameObjectWithTag("Inventory Canvas").GetComponent<Canvas>();
        movingItemPair = null;
        Destroy(activeMovingItem);
        inventoryCanvas.enabled = _isPaused;
        initCounter++;

        if (player && !_isPaused)
            UpdatePlayerWeapons();
    }

    #endregion
}
