using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : InterfaceButton
{
    #region Properties
    public ItemSlotData SlotData {  get { return slotData; } }
    public int Index { get { return index; } }
    public bool Selectable { get { return selectable; } }
    #endregion

    #region Fields
    [FoldoutGroup("References"), SerializeField]
    protected Image itemImage;

    [FoldoutGroup("References"), SerializeField]
    protected Image slotImage;

    [FoldoutGroup("References"), SerializeField]
    protected TextMeshProUGUI quantityTextMesh;

    private bool selectable;
    private ItemSlotData slotData;
    private int index;
    #endregion

    protected void Awake()
    {
        if (slotImage == null)
        {
            slotImage = GetComponent<Image>();
        }
        if (itemImage == null)
        {
            itemImage = transform.Find("Item Image")?.GetComponent<Image>();
        }
        if (quantityTextMesh == null)
        {
            quantityTextMesh = transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        }
        slotData = new(null, 0);
    }

    #region Methods
    public void UpdateSlot(ItemObject item, int itemQuantity)
    {
        SetItem(item, itemQuantity);
        SetIconSprite(item == null ? null : item.sprite);
        SetQuantity(item, itemQuantity);
        SetSelectable();
    }

    public void SetItem(ItemObject item, int quantity)
    {
        slotData.item = item;
        slotData.quantity = quantity;
    }

    public void SetIconSprite(Sprite sprite)
    {
        if (itemImage == null) return;

        itemImage.sprite = sprite;
    }

    public void SetQuantity(ItemObject item, int itemQuantity)
    {
        slotData.quantity = itemQuantity;
        DisplayItemQuantity(item, itemQuantity);
    }

    public void DisplayItemQuantity(ItemObject item, int itemQuantity)
    {
        if (quantityTextMesh == null) return;

        if (item == null || item.maxStackQuantity == 1)
        {
            SetQuantityText(String.Empty);
        }
        else
        {
            SetQuantityText(itemQuantity.ToString());
        }
    }

    private void SetQuantityText(string text)
    {
        quantityTextMesh.text = text;
    }

    public void SetSlotImage(Sprite sprite)
    {
        Debug.Log(gameObject.name);

        slotImage.sprite = sprite;
    }

    public void SetSelectable()
    {
        if (slotData.Empty())
        {
            selectable = false;
        }
        else
        {
            selectable = slotData.item.holdable ? true : false;
        }
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }
    #endregion
}

public static class ItemSlotHelper
{
    public static void Clear(this ItemSlot slot)
    {
        slot.SetItem(null, 0);
        slot.SetQuantity(null,0);
        slot.SetIconSprite(UIService.Instance.emptySprite);
    }

    public static void Activate(this ItemSlot slot, ItemObject item, int quantity)
    {
        if (!slot.gameObject.activeInHierarchy)
        {
            slot.gameObject.SetActive(true);
        }
        slot.UpdateSlot(item, quantity);
    }

    public static void Deactivate(this ItemSlot slot)
    {
        slot.Clear();
        //slot.gameObject.SetActive(false);
    }
}