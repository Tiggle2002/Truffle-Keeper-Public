using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using UnityEngine;
using UnityEngine.EventSystems;

interface ICraftable
{
    CraftingMaterial[] requiredItems { get; set; }

    bool CanCraft();

    void Craft();
}

public class CraftButton : InterfaceButton, TEventListener<StructureEvent>, TEventListener<MonumentEvent>
{
    public CraftingMaterial[] RequiredItems { get { return requiredItems; } }
    private CraftingMaterial[] requiredItems;
    protected int price;
    private StructureData structureData;    

    [FoldoutGroup("Craft Settings"), SerializeField] private CategoryType craftType;

    #region Craft Feedback
    [FoldoutGroup("Craft Feedback"), SerializeField] protected MMF_Player craftFeedback;
    [FoldoutGroup("Craft Feedback"), SerializeField] protected MMF_Player craftFailedFeedback;
    #endregion

    public void SetRecipe(CraftingMaterial[] recipe)
    {
        requiredItems = recipe;
    }

    public void SetPrice(int price)
    {
        this.price = price;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        Debug.Log("pointer click");
        TryCraft();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        Debug.Log("pointer enter");
    }

    public virtual void TryCraft()
    {
        if (requiredItems == null) return;

        if (CanCraft() && CanPurchase())
        {
            PerformTransaction();
        }
        else
        {
            craftFailedFeedback?.PlayFeedbacks();
        }
    }

    public virtual bool CanCraft()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (requiredItems[i].item == null || requiredItems[i].item.ID == null || requiredItems[i].quantity == 0)
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

    public virtual bool CanPurchase()
    {
        if (price == 0)
        {
            return true;
        }

        return price <= PlayerManager.Instance.inventory.FindQuantityOfItem(UIService.Instance.currencyObject);
    }

    protected virtual void PerformTransaction()
    {
        Craft();
        Purchase();
        PerformAction();
    }

    public virtual void Craft()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (requiredItems[i].item != null && requiredItems[i].item.ID != null && requiredItems[i].quantity > 0)
            {
                InventoryEvent.Trigger(InventoryEventType.ItemConsumed, new EventItem(requiredItems[i].item, requiredItems[i].quantity));
            }
        }
        craftFeedback?.PlayFeedbacks();
    }

    public virtual void Purchase()
    {
        if (price <= 0)
        {
            return;
        }
        InventoryEvent.Trigger(InventoryEventType.ItemConsumed, new EventItem(UIService.Instance.currencyObject, price));
    }

    private void PerformAction()
    {
        if (craftType == CategoryType.Structure)
        {
            ActiveStructure.TryBuildAtCurrentLocation(structureData);
        }
        else if (craftType == CategoryType.Monument)
        {
            MonumentEvent.Trigger(MonumentEventType.Upgraded);
        }
    }

    public void OnEvent(StructureEvent eventData)
    {
        if (craftType != CategoryType.Structure) return;

          switch (eventData.eventType)
        {
            case StructureEventType.StructureSelected:
                SetRecipe(eventData.data.craftingRecipe);
                SetPrice(eventData.data.price);
                structureData = eventData.data;
                break;
            case StructureEventType.StructureUpgradeAvailable:
                SetRecipe(eventData.data.upgradedStructureData.craftingRecipe);
                SetPrice(eventData.data.upgradedStructureData.price);
                structureData = eventData.data.upgradedStructureData;
                break;
            case StructureEventType.RepairSelected:
                SetRecipe(eventData.recipe);
                SetPrice(eventData.data.price);
                structureData = eventData.data;
                break;
        }
    }

    public void OnEvent(MonumentEvent eventData)
    {
        switch (eventData.eventType)
        {
            case MonumentEventType.UpgradeRecipeChanged:
                SetRecipe(eventData.recipe);
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<StructureEvent>();
        this.Subscribe<MonumentEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<StructureEvent>();
        this.Unsubscribe<MonumentEvent>();
    }
}

[System.Serializable]
public struct CraftingMaterial
{
    public ItemObject item;
    public int quantity;
}
