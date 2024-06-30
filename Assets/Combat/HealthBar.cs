using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Breakable))]
public class HealthBar : MonoBehaviour
{
    private Breakable breakable;
    private Canvas canvas;
    private Slider slider;

    private void Awake()
    {
        breakable = GetComponent<Breakable>();
        canvas = GetComponentInChildren<Canvas>();
        slider = GetComponentInChildren<Slider>();
        breakable.OnDamageTaken += UpdateDisplay;
        canvas.enabled = false;
    }

    private void UpdateDisplay()
    {
        canvas.enabled = true;
        slider.value = breakable.Health / breakable.MaxHealth;
        Debug.Log(slider.value);
    }
}
