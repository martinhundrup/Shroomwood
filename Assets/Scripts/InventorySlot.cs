using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void InventorySlotClickedHandler(int slotID);
    public InventorySlotClickedHandler OnInventorySlotClicked;

    [SerializeField] private int index;
    [SerializeField] private Color nuetral;
    [SerializeField] private Color hover;
    [SerializeField] private GameObject tooltipDisplay;
    [SerializeField] private GameObject weapontipDisplay;
    private GameObject activeTooltip;

    [SerializeField] private Image itemDisplay;
    private TMPro.TextMeshProUGUI amountText;

    // The current item type being stored in this slot.
    private ItemData itemData;
    private bool isHovered;

    // The amount of whatever item stored.
    private int amount;

    public ItemData ItemData
    {
        get { return this.itemData; }
        set { this.itemData = value; this.Refresh(); }
    }
    public int Amount
    {
        get { return this.amount; }
        set { this.amount = value; this.Refresh(); }
    } 
    public int Index
    {
        get { return this.index; }
    }

    private void Awake()
    {
        FindObjectOfType<InventoryManager>().SubscribeToInventorySlot(this);
        Debug.Log("subscribed to inventory manager");
        amountText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        this.Refresh();
    }

    private void Update()
    {
        if (isHovered && Input.GetMouseButtonDown(0))
        {
            if (OnInventorySlotClicked != null)
                OnInventorySlotClicked(this.index);
        }
    }

    // This method is called when the cursor enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = hover;

        if (this.itemData != null)
        {
            // show a weapon
            if (this.itemData.GetType() == typeof(WeaponData))
            {
                activeTooltip = Instantiate(weapontipDisplay, GetComponentInParent<Canvas>().transform);
                activeTooltip.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position;
                activeTooltip.GetComponent<Weapontip>().Init(this.itemData);
            }
            else // default dipsplay
            {
                activeTooltip = Instantiate(tooltipDisplay, GetComponentInParent<Canvas>().transform);
                activeTooltip.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position;
                activeTooltip.GetComponent<Tooltip>().Init(this.itemData);
            }
            
            activeTooltip.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position; 
            activeTooltip.GetComponent<Tooltip>().Init(this.itemData);
        }

        
        isHovered = true;
    }

    // This method is called when the cursor exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = nuetral;
        Destroy(activeTooltip);
        isHovered = false;
    }
    
    public void Refresh()
    {
        if (itemData == null)
        {
            itemDisplay.sprite = null;
            itemDisplay.enabled = false;
            amountText.text = string.Empty;
        }
        else
        {

            itemDisplay.sprite = itemData.Icon;
            itemDisplay.enabled = true;
            if (amount < 1)
                amountText.text = string.Empty;
            else
                amountText.text = amount.ToString();
        }
    }
}
