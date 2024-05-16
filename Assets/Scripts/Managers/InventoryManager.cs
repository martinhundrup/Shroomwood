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

    [SerializeField] private InventoryData inventory;

    

    #endregion

    #region UNITY CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        this.player = FindObjectOfType<PlayerController>();
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
        if (this.inventory.inventory.ContainsKey(_item))
        {
            this.inventory.inventory[_item]++;
            Debug.Log(_item.ItemName + " was collected: amount: " + this.inventory.inventory[_item]);
            this.OnCollect();
            
        }
        else if (inventory.inventory.Count < inventory.SlotCount) // only add item if inventory is not full
        {
            this.inventory.inventory.Add(_item, 1);
            Debug.Log(_item.ItemName + " was collected: amount: " + this.inventory.inventory[_item]);
            this.OnCollect();
        }

    }

    #endregion
}
