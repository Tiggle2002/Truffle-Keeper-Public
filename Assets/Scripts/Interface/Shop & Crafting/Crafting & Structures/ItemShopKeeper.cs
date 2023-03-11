using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
           
public class ItemShopKeeper : TEventListener<InventoryEvent>, TEventListener<InterfaceEvent>, IDisposable
{
    #region Fields
    private ItemShop shopInterface;
    private List<ItemObject> oneTimePurchaseItemsThatHaveAlreadyBeenBought = new();
    #endregion

    public ItemShopKeeper(ItemShop shopInterface)
    {
        this.shopInterface = shopInterface;

        this.Subscribe<InterfaceEvent>();
        this.Subscribe<InventoryEvent>();
    }

    private void TryPreventPurchaseOfItem(ItemObject item)
    {
        if (item.oneTimePurchase)
        {
            ItemSlot slotWithItem = GetSlotWithItem(item);
            oneTimePurchaseItemsThatHaveAlreadyBeenBought.Add(item);
            SetPurchaseability((ItemPurchaseButton)slotWithItem);
        }
    }

    private ItemSlot GetSlotWithItem(ItemObject item)
    {
        ItemSlotData[] itemsInCategory = shopInterface.GetProductsOfCategory((int)item.type);
            ;

        for (int i = 0; i < itemsInCategory.Length; i++)
        {
            if (itemsInCategory[i].item.ID == item.ID)
            {
                return shopInterface.GetSlot(i);
            }
        }
        return null;
    }

    public void SetPurchaseability(ItemPurchaseButton slot)
    {
        bool canPurchase = CheckItemCanBePurchased(slot.SlotData.item);

        slot.SetPurchaseMode(canPurchase);
    }

    private bool CheckItemCanBePurchased(ItemObject item)
    {
        if (oneTimePurchaseItemsThatHaveAlreadyBeenBought.Contains(item))
        {
            return false;
        }
        return RequiredItemBought(item);
    }

    private bool RequiredItemBought(ItemObject item)
    {
        ItemObject requiredItem = item.requiredItem;

        if (requiredItem == null || oneTimePurchaseItemsThatHaveAlreadyBeenBought.Contains((ItemObject)item.requiredItem))
        {
            return true;
        }
        return false;
    }

    #region Events
    public virtual void OnEvent(InterfaceEvent eventData)
    {
        if (eventData.eventInterface != shopInterface)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case InterfaceEventType.SlotSelected:
                if (eventData.interfaceButton != null)
                shopInterface.CreateInformationWindowForSelectedItem(eventData.interfaceButton);
                break;
        }
    }

    public virtual void OnEvent(InventoryEvent eventData)
    {
        if (!shopInterface.Open) return;

        switch (eventData.eventType)
        {
            case InventoryEventType.ItemPurchased:
                TryPreventPurchaseOfItem(eventData.eventItem.item);
                shopInterface.TryUpdateCurrencyCounter();
                break;
        }
    }

    public void Dispose()
    {
        this.Unsubscribe<InterfaceEvent>();
        this.Unsubscribe<InventoryEvent>();
    }
    #endregion
}