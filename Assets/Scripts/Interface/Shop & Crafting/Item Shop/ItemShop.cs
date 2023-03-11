using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemShop : BaseItemInterface, TEventListener<WaveEvent>, ICategorised
{
    #region Product References
    [SerializeField] private Dictionary<ItemType, List<ItemSlotData>> productMap = new Dictionary<ItemType, List<ItemSlotData>>()
    {
        { ItemType.Weapon, new List<ItemSlotData>() },
        { ItemType.Consumable, new List<ItemSlotData>() }
    };
    #endregion

    #region References
    [FoldoutGroup("References")]
    [SerializeField] private ItemPurchaseButton selectedSlot;
    [FoldoutGroup("References")]
    [SerializeField] public Sprite purchaseEnabledButton;
    [FoldoutGroup("References")]
    [SerializeField] public Sprite purchaseDisabledButton;
    [FoldoutGroup("References")]
    [SerializeField] private TextMeshProUGUI currencyCounter;

    private ItemShopKeeper shopKeeper;
    #endregion

    #region Selected Item Referencs
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private CanvasGroup selectedItemCanvas;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI title;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI description;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI damageStat;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI rangeStat;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI speedStat;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI knockbackStat;
    [FoldoutGroup("Selected Item References")]
    [SerializeField] private TextMeshProUGUI purchaseText;
    #endregion

    #region Unity Update Methods
    protected override void SetInitialState()
    {
        base.SetInitialState();
        shopKeeper = new(this);
        SetCategory((int)ItemType.Weapon);
    }
    #endregion

    #region Interface Methods
    public override void OpenInterface()
    {
        base.OpenInterface();

        TryUpdateCurrencyCounter();
    }

    public void TryUpdateCurrencyCounter()
    {
        currencyCounter.text = PlayerManager.Instance.inventory.FindQuantityOfItem(UIService.Instance.currencyObject).ToString();
    }

    public override void CloseInterface()
    {
        base.CloseInterface();
        
        InterfaceEvent.Trigger(InterfaceEventType.Opened, this);

        if (selectedItemCanvas.gameObject.activeInHierarchy)
        {
            selectedItemCanvas.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Item Display Methods
    public void ChangeShopItemsOnDisplay(ItemSlotData[] slotData)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i >= slotData.Length || slotData[i].Empty())
            {
                DeactivateSlot(itemSlots[i]);
            }
            else if (!slotData[i].Empty() && itemSlots[i] != null)
            {
                SetSlotData(itemSlots[i], slotData[i]);
                shopKeeper?.SetPurchaseability((ItemPurchaseButton)itemSlots[i]);
            }
        }
        UpdateInterface();
    }

    private void DeactivateSlot(ItemSlot shopSlot)
    {
        shopSlot.SetItem(null, 0);
        shopSlot.transform.parent.gameObject.SetActive(false);
    }

    private void SetSlotData(ItemSlot slot, ItemSlotData slotData)
    {
        if (!slot.gameObject.activeInHierarchy)
        {
            slot.gameObject.transform.parent.gameObject.SetActive(true);
        }
        slot.UpdateSlot(slotData.item, slotData.quantity);
    }

    protected override void UpdateInterface()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].SlotData.Empty())
            {
                itemSlots[i].Clear();
            }
        }
    }
    #endregion

    #region Item Selection
    public ItemSlotData[] GetProductsOfCategory(int category)
    {
        if (productMap.TryGetValue((ItemType)category, out var products))
        {
            return products.ToArray();
        }
        return null;
    }

    public void CreateInformationWindowForSelectedItem(InterfaceButton interfaceButton)
    {
        return; //METHOD DEACTIVATED UNTIL I CAN BE BOTHERED TO REIMPLEMENT

        ItemPurchaseButton shopButton = interfaceButton.GetComponentInChildren<ItemPurchaseButton>(true);
        if (shopButton == null)
        {
            return;
        }

        ItemObject item = shopButton.SlotData.item;

        SetTextMeshElementsToSelectedItem(item);
        SetSlotData(selectedSlot,shopButton.SlotData);
        shopKeeper.SetPurchaseability(selectedSlot);
        selectedSlot.DisplayProduct();
        if (!selectedItemCanvas.gameObject.activeInHierarchy)
        {
            selectedItemCanvas.gameObject.SetActive(true);
        }
    }

    public void SetTextMeshElementsToSelectedItem(ItemObject item)
    {
        title.text = item.ID;
        if (description)
            description.text = item.description;
        if (damageStat)
            SetDamageText(item);
        if (rangeStat)
            SetRangeText(item);
        if (speedStat)
            SetSpeedText(item);
        if (knockbackStat)
            SetKnockbackText(item);
        if (purchaseText)
            purchaseText.text = $"Purchase : {item.price}";
    }

    private void SetDamageText(ItemObject item)
    {
        int damage = 0;
        if (item.comboWeapon && item.combos.Length > 0)
        {
            for (int i = 0; i < item.combos.Length; i++)
            {
                damage += item.combos[i].damage;
            }

            damage /= item.combos.Length;
        }
        else
        {
            damage = item.damage;
        }

        if (damage > 0)
        {
            damageStat.text = $"Damage : {damage}";
        }
        else
        {
            damageStat.text = string.Empty;
        }
    }

    private void SetRangeText(ItemObject item)
    {
        string text = rangeStat.text = $"Range : {item.range}";
        if (!item.ammoBased)
        {
            text = string.Empty;
        }

        rangeStat.text = text;
    }

    private void SetSpeedText(ItemObject item)
    {
        
    }

    private void SetKnockbackText(ItemObject item)
    {

    }
    #endregion

    #region Event Methods
    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.WaveBegun:
                if (interfaceOpen)
                {
                    CloseInterface();
                }
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<WaveEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        shopKeeper.Dispose();
        this.Unsubscribe<WaveEvent>();
    }

    public void SetCategory(int category)
    {
        ItemSlotData[] itemsOfCategory = GetProductsOfCategory(category);

        ChangeShopItemsOnDisplay(itemsOfCategory);
    }

    #endregion
}



[System.Serializable]
public class ItemSlotData
{
    public ItemObject item;
    public int quantity;

    public ItemSlotData(ItemObject item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public bool Empty()
    {
        return item == null || item.ID == null;
    }
}
