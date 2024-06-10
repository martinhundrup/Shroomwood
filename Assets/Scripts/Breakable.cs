using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    #region ATTRIBUTES

    // The current amount of damage this object can take before it is destroyed.
    [SerializeField] private float health;

    // Holds reference to all the items and amounts to be dropped when this object is destroyed. 
    private Dictionary<GameObject, int> itemDrops;

    // Contains a key value for intstantiating the dictionary, as dictionaries cannot be serialized in the inspector.
    [Serializable]
    private class KeyValuePair
    {
        public GameObject _item;
        public int _amount;
    }

    // The list to be filled in the inspector.
    [SerializeField] private List<KeyValuePair> itemList = new List<KeyValuePair>();

    #endregion

    #region PROPERTIES

    // Gets the amount of damage this object can take before it is destroyed.
    public float Health
    {
        get { return health; }
    }

    #endregion

    #region UNITY CALLBACKS

    // Called once at beginning of scene.
    private void Start()
    {
        itemDrops = new Dictionary<GameObject, int>();

        LoadDictionary();
    }

    private void SpawnItems()
    {
        if (itemDrops == null || this.itemDrops.Count == 0) return;
        foreach (var _item in this.itemDrops.Keys)
        {
            for (int i = 0; i < this.itemDrops[_item]; i++)
            {
                var obj = Instantiate(_item);
                obj.transform.position = this.transform.position;
            }
        }
    }

    // The method called when another object with a 2D collider overlaps with this object's collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitbox = collision.gameObject.GetComponent<Hitbox>();

        // a hitbox has collided with this and did not belong to this obj
        if (hitbox != null && !this.CompareTag(hitbox.Tag))
        {
            Debug.Log("hit");

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
            itemDrops[_item._item] = _item._amount;
        }
    }

    // Destroys this game object if health is at or below 0.
    private void CheckHealth()
    {
        if (health <= 0)
        {
            SpawnItems();
            Destroy(this.gameObject);
        }
    }

    // The method called when the hurtbox is hit.
    private void Hurt(float _damage)
    {
        health -= _damage;
        CheckHealth();
    }

    #endregion
}
