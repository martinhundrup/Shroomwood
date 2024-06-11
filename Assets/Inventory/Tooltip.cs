using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


[RequireComponent(typeof(CanvasGroup))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Init(ItemData _data)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Make sure the image ignores raycasts;
        

        nameText.text = _data.ItemName;
        descriptionText.text = _data.Description;
    }
}
