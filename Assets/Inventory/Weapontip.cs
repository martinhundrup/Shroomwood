using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class Weapontip : Tooltip
{
    [SerializeField] private TextMeshProUGUI dmgText;
    [SerializeField] private TextMeshProUGUI spdText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI knockText;

    new public void Init(ItemData _itemData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Make sure the image ignores raycasts;
        WeaponData _weaonnData = _itemData as WeaponData;

        this.dmgText.text = _weaonnData.Damage.ToString();
        this.spdText.text = _weaonnData.Cooldown.ToString();
        this.stunText.text = _weaonnData.StunTime.ToString();
        this.knockText.text = _weaonnData.Knockback.ToString();
    }
}
