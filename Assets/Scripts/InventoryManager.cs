using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region ATTRIBUTES

    // Holds reference to the instantiated player script.
    private PlayerController player;

    // The items and corresponding amounts currently in the main inventory.
    private Dictionary<ItemData, int> inventory;

    #endregion

    #region UNITY CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        this.player = FindObjectOfType<PlayerController>();
        this.inventory = new Dictionary<ItemData, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region METHODS

    // Subscribes to an ItemDrops OnCollect event.
    public void Subscribe(ItemDrop _item)
    {
        _item.OnCollect += ItemCollected;
    }

    // Adds an item to the inventory when collected.
    private void ItemCollected(ItemData _item)
    {
        if (this.inventory.ContainsKey(_item))
        {
            this.inventory[_item]++;
        }
        else
        {
            this.inventory.Add(_item, 1);
        }

        Debug.Log(_item.ItemName + " was collected: amount: " + this.inventory[_item]);
    }

    #endregion
}
