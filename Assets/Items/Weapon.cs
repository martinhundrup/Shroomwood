using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // reference to the weapon data.
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private WeaponModifiers modifiers;

    public WeaponModifiers Modifiers 
    { 
        get { return modifiers; } 
    }

    public WeaponData WeaponData 
    { 
        get { return weaponData; } 
    }

    private void Awake()
    {
        this.modifiers = new WeaponModifiers();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("collided with player");
            Destroy(this.gameObject);
        }
    }
}
