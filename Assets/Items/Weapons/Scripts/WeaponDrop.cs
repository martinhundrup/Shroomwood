using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : ItemDrop
{
    [SerializeField] private WeaponModifiers modifiers;

    public WeaponModifiers Modifiers 
    { 
        get { return modifiers; } 
    }

    new private void Awake()
    {
        base.Awake();
        this.modifiers = new WeaponModifiers();
    }
}
