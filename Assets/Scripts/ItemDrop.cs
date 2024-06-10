using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemDrop : MonoBehaviour
{
    // TODO: make it so the item is only attracted to the player if a slot is open.

    #region EVENTS

    // An event called when a hurtbox takes damage (collides with a hitbox).
    public delegate void CollectAction(ItemDrop _item);

    // The event called when this object collides with a hitbox.
    public event CollectAction OnCollect;

    #endregion

    #region ATTRIBUTES

    // The item data that represents this object
    [SerializeField] protected ItemData itemData;
    protected Rigidbody2D rb;
    private static readonly System.Random globalRandom = new System.Random();

    #endregion

    #region PROPERTIES

   
    // Gets the item data.
    public ItemData ItemData
    {
        get { return this.itemData; } 
    }

    #endregion

    #region UNITY CALLBACKS

    // Called when this object enters the scene.
    protected void Awake()
    {
        float seed = (globalRandom.Next(0, 201) - 100) / 100f;

        var iM = FindObjectOfType<InventoryManager>();
        if (iM == null) Destroy(this.gameObject);
        else
        {
            iM.Subscribe(this);
        }

        this.rb = GetComponent<Rigidbody2D>();
        this.rb.freezeRotation = true;

        this.rb.AddForce((seed * Vector2.one).normalized * 0.05f);
    }

    private void FixedUpdate()
    {
        rb.velocity = rb.velocity * 0.8f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // TODO: check with inventory manager if we can be picked up before actually getting picked up.


            OnCollect(this);
            Destroy(this.gameObject);
        }
    }
    #endregion
}