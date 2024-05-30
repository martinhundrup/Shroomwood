using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void CollectAction();

    // The event called when this object collides with a hitbox.
    public event CollectAction OnCollect;

    #endregion

    #region ATTRIBUTES

    // Holds reference to the instantiated player script.
    private PlayerController player;

    // The player's inventory scriptable object.
    private InventoryData playerInventory;

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
        this.player = FindObjectOfType<PlayerController>();
    }

    #endregion

    #region METHODS

    // Subscribes to an ItemDrops OnCollect event.
    public void Subscribe(ItemDrop _item)
    {
        _item.OnCollect += ItemCollected;
    }

    // Adds an item to the inventory when collected.
    private void ItemCollected(ItemDrop _item)
    {
        // check if weapon
        if (_item is WeaponDrop weapon)
        {
            // _item is indeed a WeaponData
            Debug.Log("Collected a weapon: " + weapon.ItemData.ItemName);

            player.EquipWeapon((WeaponDrop)_item);
        }


        bool success = playerInventory.AddItem(_item.ItemData, 1);
        if (success)
        {
            Debug.Log(_item.ItemData.ItemName + " was collected");

            // TODO: implement some sort of UI message when a item is collected.
            //this.OnCollect();
        }
    }

    #endregion
}
