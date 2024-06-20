using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private string hitboxTag; // immune to hitboxes of the same tag
    [SerializeField] private float health;

    public float Health
    {
        get { return health; }
        private set 
        { 
            health = value;
            CheckHealth();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitbox = collision.gameObject.GetComponent<Hitbox>();

        if (hitbox != null && hitbox.HitboxTag != hitboxTag)
        {
            Health -= hitbox.Damage;
        }
    }


    private void CheckHealth()
    {
        if (health <= 0) { Destroy(this.gameObject); }
    }
}
