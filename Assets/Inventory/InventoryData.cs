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
    private Pair<ItemData, int>[,] inventory = new Pair<ItemData, int>[3,5];

    // Gets the item information at a parameterized index.
    public Tuple<ItemData, int> GetItem(int _y, int _x)
    {
        if (inventory[_y, _x] == null)
        {
            return null;
        }
        else
        {
            return new Tuple<ItemData, int>(inventory[_y, _x].First, inventory[_y, _x].Second);
        }
    }

    // Attempts to add an item to the inventory.
    public bool AddItem(ItemData _item, int _amount)
    {
        // check to see if item has been collected yet
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (inventory[i,j] != null && inventory[i,j].First == _item)
                {
                    inventory[i, j].Second++;
                    return true;
                }
            }
        }

        // did not find slot for it - find first empty slot
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (inventory[i, j] == null)
                {
                    inventory[i, j] = new Pair<ItemData, int>(_item, 1);
                    return true;
                }
                else if (inventory[i, j].Second <= 0)
                {
                    inventory[i, j].First = _item;
                    inventory[i, j].Second = 1;
                    return true;
                }
            }
        }

        // item could not fit
        return false;
    }
}
