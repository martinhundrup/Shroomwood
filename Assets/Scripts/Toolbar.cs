using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{
    // Ref to the central inventory manager.
    [SerializeField] private InventoryData inventory;

    // The inventory slot children.
    [SerializeField] private InventorySlot[] slots;

    // Ref to the inventory manager.
    private InventoryManager inventoryManager;

    // Runs when object is created.
    private void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.OnCollect += ItemCollected;
    }
      
    // Called on beggining of scene.
    private void Start()
    {
        
    }

    // To be called when an item is collected.
    private void ItemCollected()
    {
        int index = 0;
        foreach (var item in inventory.inventory)
        {
            slots[index].Item = item.Key;
            slots[index].Amount = item.Value;
            index++;
        }

        for (; index < slots.Length; index++) // set the other inventory slots to empty
        {
            slots[index].NoItem();
        }
    }
}
