using SurvivalElements;
using System.Collections;
using UnityEngine;

public class ItemTradeButton : ItemPurchaseButton, ICraftable, TEventListener<CraftingEvent>
{
    #region Methods
    public override void TryTrade()
    {
        if (CanAfford())
        {
            Purchase();
            Craft();
            GiveItem();
            tradeFeedback?.PlayFeedbacks();
        }
        else
        {
            tradeFailedFeedback?.PlayFeedbacks();
        }
    }

    public override bool CanAfford()
    {
        return base.CanAfford() && CanCraft();
    }

    public bool CanCraft()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (requiredItems[i].item == null || requiredItems[i].quantity == 0)
            {
                continue;
            }
            if (requiredItems[i].quantity > PlayerManager.Instance.inventory.FindQuantityOfItem(requiredItems[i].item))
            {
                return false;
            }
        }
        return true;
    }

    public void Craft()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (requiredItems[i].item != null && requiredItems[i].item.ID != null && requiredItems[i].quantity > 0)
            {
                InventoryEvent.Trigger(InventoryEventType.ItemConsumed, new EventItem(requiredItems[i].item, requiredItems[i].quantity));
            }
        }
    }
    #endregion

    #region TEvent Methods
    public void OnEvent(CraftingEvent eventData)
    {
        switch (eventData.eventType)
        {
            case CraftingEventType.SetRecipe:
                SetTradeElements(eventData.itemData.item, eventData.itemData.quantity, eventData.recipe);
                break;
        }
    }

    private void SetTradeElements(ItemObject item, int quantity,CraftingMaterial[] materialsRequired)
    {
        requiredItems = materialsRequired;
        
        UpdateSlot(item, quantity);

        SetPurchaseMode(CanAfford());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<CraftingEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<CraftingEvent>();
    }
    #endregion
}