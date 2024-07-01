using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Player : MonoBehaviour
{
    private Slider slider;
    private PlayerStats stats;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        stats = DataDictionary.PlayerStats;
        stats.OnDamageTaken += UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        slider.value = stats.PlayerHealth / stats.PlayerMaxHealth;
    }
}
