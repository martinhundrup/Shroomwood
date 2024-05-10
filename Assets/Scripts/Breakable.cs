using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    #region ATTRIBUTES

    // The current amount of damage this object can take before it is destroyed.
    [SerializeField] private int health;

    // Holds reference to all the items and amounts to be dropped when this object is destroyed. 
    private Dictionary<ItemDrop, int> itemDrops;

    // The size of the area items are dropped in.
    [SerializeField] private float dropRadius;

    // Contains a key value for intstantiating the dictionary, as dictionaries cannot be serialized in the inspector.
    [Serializable]
    private class KeyValuePair
    {
        public ItemDrop key;
        public int val;
    }

    // The list to be filled in the inspector.
    [SerializeField] private List<KeyValuePair> itemList = new List<KeyValuePair>();

    #endregion

    #region PROPERTIES

    // Gets the amount of damage this object can take before it is destroyed.
    public int Health
    {
        get { return health; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Called once at beginning of scene.
    private void Start()
    {
        itemDrops = new Dictionary<ItemDrop, int>();

        LoadDictionary();
    }

    // Called when this object/script is destroyed.
    private void OnDestroy()
    {
        foreach (ItemDrop _item in this.itemDrops.Keys)
        {
            for (int i = 0; i < this.itemDrops[_item]; i++)
            {
                // generate a random position in the drop area
                Vector2 pos = transform.position;
                pos.x += dropRadius * UnityEngine.Random.value - dropRadius / 2; // Random.value returns a number between 0 and 1 inclusive
                pos.y += dropRadius * UnityEngine.Random.value - dropRadius / 2;

                // instantiate the drop and place it in the random area
                ItemDrop item = Instantiate(_item);
                item.transform.position = transform.position;
                item.TargetPos = pos;
            }
        }
    }

    // The method called when another object with a 2D collider overlaps with this object's collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // a hitbox has collided with this
        if (collision.gameObject.GetComponent<Hitbox>() != null)
        {
            Hurt(collision.gameObject.GetComponent<Hitbox>().Damage);
        }
    }

    #endregion

    #region METHODS

    // Stores all items from the serialized list into the item drops dictionary.
    private void LoadDictionary()
    {
        foreach (KeyValuePair _item in this.itemList)
        {
            itemDrops[_item.key] = _item.val;
        }
    }

    // Destroys this game object if health is at or below 0.
    private void CheckHealth()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // The method called when the hurtbox is hit.
    private void Hurt(int _damage)
    {
        health -= _damage;
        CheckHealth();
    }

    #endregion
}