using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using Application; 

[CreateAssetMenu]
public class InventoryData : ScriptableObject
{
    // The items and corresponding amounts currently in the main inventory.
    private Pair<ItemData, int>[] inventory = new Pair<ItemData, int>[17];



    // Gets the item information at a parameterized index.
    public Tuple<ItemData, int> GetItem(int _index)
    {
        if (inventory[_index] == null)
        {
            return null;
        }
        else
        {
            return new Tuple<ItemData, int>(inventory[_index].First, inventory[_index].Second);
        }
    }

    public bool SetSlot(int index, Tuple<ItemData, int> data)
    {
        if (inventory[index] != null)
        {
            return false;
        }
        else
        {
            inventory[index] = new Pair<ItemData, int>(data.Item1, data.Item2);
            return true;
        }
    }

    // empties an inventory slot and returns the items stored there
    public Tuple<ItemData, int> EmptySlot(int _index)
    {
        if (inventory[_index] == null)
        {
            return null;
        }
        else
        {
            var t = new Tuple<ItemData, int>(inventory[_index].First, inventory[_index].Second);
            inventory[_index] = null; 
            return t;
        }
    }

    // Attempts to add an item to the inventory.
    // returns index at which it was added, or -1 if could not be added
    public int AddItem(ItemData _item, int _amount)
    {
        // check to see if item has been collected yet
        for (int i = 0; i < 15; i++)
        {
            if (inventory[i] != null && inventory[i].First == _item)
            {
                inventory[i].Second++;
                return i;
            }
        }

        // did not find slot for it - find first empty slot
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = new Pair<ItemData, int>(_item, 1);
                    return i;
                }
                else if (inventory[i].Second <= 0)
                {
                    inventory[i].First = _item;
                    inventory[i].Second = 1;
                    return i;
                }
            }
        }

        // item could not fit
        return -1;
    }
}
