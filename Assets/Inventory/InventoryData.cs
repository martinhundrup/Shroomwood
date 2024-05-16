using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryData : ScriptableObject
{
    // The number of inventory slots - the number of unique items able to hold.
    [SerializeField] private int slotCount;

    // The items and corresponding amounts currently in the main inventory.
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    public int SlotCount
    {
        get { return slotCount; }
        set { slotCount = value; }
    }
}
