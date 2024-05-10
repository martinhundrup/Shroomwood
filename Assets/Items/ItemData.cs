using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{

    // The name of the item.
    [SerializeField] private string itemName;

    // The sprite representation for the item.
    [SerializeField] private Sprite icon;

    // A short desciption for defining the item.
    [TextArea]
    [SerializeField] private string description;

    // Contains a prefab the represents the item drop.
    [SerializeField] private Object itemDrop;

    // Gets the name of the item.
    public string ItemName
    {
        get { return this.itemName; }
    }

    // Gets the sprite representation for this item.
    public Sprite Icon
    {
        get { return this.icon; }
    }

    // Gets the description for the object.
    public string Description
    {
        get { return this.description; }
    }

    // Gets the item drop.
    public Object ItemDrop
    {
        get { return this.itemDrop; }
    }
}