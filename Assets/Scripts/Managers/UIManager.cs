using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //// The canvas that displays all information related to the inventory - only visible when paused.
    //[SerializeField] private Canvas inventoryCanvas;

    //// Ref to the player's inventory scriptable obj.
    //private InventoryData playerInventory;

    //[SerializeField] private InventorySlot[] inventorySlots;

    //// The UI elements that display the itme info.
    //private InventorySlot[,] slots;

    //// Gets or sets the player's inventory.
    //public InventoryData PlayerInventory
    //{
    //    get { return this.playerInventory; }
    //    set { this.playerInventory = value; }
    //}


    //// Called when object is created.
    //private void Awake()
    //{
    //    inventoryCanvas.enabled = false;
    //    slots = new InventorySlot[3, 5];

    //    int count = 0;

    //    // initialize the 2D array of inventory slots
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 5; j++)
    //        {
    //            slots[i, j] = inventorySlots[count++];
    //        }
    //    }
    //}

    //// Called by game manager when the game is paused.
    //public void GamePaused(bool _paused)
    //{
    //    // game unapused - disable canvas
    //    if (!_paused)
    //    {
    //        inventoryCanvas.enabled = false;
    //    }
    //    else // game puased - enable and update inventory display
    //    {
    //        inventoryCanvas.enabled = true;
    //        Canvas.ForceUpdateCanvases();
    //    }
    //}
}
