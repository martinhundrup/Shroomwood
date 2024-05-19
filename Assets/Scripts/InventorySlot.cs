using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{   
    // The current item type being stored in this slot.
    private ItemData item;

    // The sprite to display
    private Image sprite;

    // The digit display for the amount of items
    private TMPro.TextMeshProUGUI text;

    // The amount of whatever item stored.
    private int amount;

    private void Awake()
    {
        this.sprite = GetComponent<Image>();
        this.text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Gets or sets the item amount.
    public int Amount
    {
        get { return amount; }
        set 
        { 
            amount = value;
            UpdateTextDisplay();
        }
    }

    // Gets or sets the item being stored.
    public ItemData Item
    {
        get { return item; }
        set 
        {
            if (value == null)
            {
                item = null;
                sprite.sprite = null;
            }
            else
            {
                Debug.Log(value.Icon); 
                item = value;
                sprite.sprite = value.Icon;
            }            
        }
    }

    // Updates the text display to the number
    private void UpdateTextDisplay()
    {
        if (amount == 0)
        {
            text.text = string.Empty;
        }
        else
        {
            text.text = Amount.ToString();
        }
    }

    // Sets the text to nothing and sprite to null; used for when no item is contained.
    public void NoItem()
    {
        this.Item = null;
        this.amount = 0;
    }
}
