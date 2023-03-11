using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IPurchaseable
{
    bool CanAfford();

    void Purchase();
}

public class ItemPurchaseButton : ItemSlot, IPurchaseable,TEventListener<InventoryEvent>
{
    #region Fields

    public CraftingMaterial[] requiredItems { get; set; }

    [FoldoutGroup("References"), SerializeField]
    private Sprite tradeEnabledSprite;

    [FoldoutGroup("References"), SerializeField]
    private Sprite tradeDisabledSprite;

    [FoldoutGroup("UI References"), SerializeField]
    private Image buttonImage;

    [FoldoutGroup("UI References"), SerializeField]
    private TextMeshProUGUI price;

    [FoldoutGroup("UI References"), SerializeField]
    private TextMeshProUGUI itemName;


    private bool holdToTrade = false;
    private bool purchasePermitted = true;
    private Coroutine purchaseCoroutine;
    #endregion

    #region Purchase Feedbacks
    [FoldoutGroup("Purchase Feedbacks")]
    [SerializeField] protected MMF_Player tradeFeedback;
    [FoldoutGroup("Purchase Feedbacks")]
    [SerializeField] protected MMF_Player tradeFailedFeedback;
    #endregion

    #region Methods
    public void DisplayProduct()
    {
        if (SlotData.Empty())
        {
            return;
        }

        if (!SlotData.item.oneTimePurchase)
        {
            quantityTextMesh.text = $"{SlotData.quantity}";
        }
        else
        {
            quantityTextMesh.text = string.Empty;
        }

        itemImage.sprite = SlotData.item.sprite;
    }

    public void ClearSlot(Sprite emptySprite)
    {
        quantityTextMesh.text = string.Empty;
        itemImage.sprite = emptySprite;
    }

    public void SetPurchaseMode(bool canPurchase)
    {
        this.purchasePermitted = canPurchase;

        UpdateButton();
    }

    public void UpdateButton()
    {
        holdToTrade = SlotData.item.maxStackQuantity > 1 ? true : false;

        SetProductAlpha();
        DisplayPrice();
        DisplayProduct();
        SetButtonSprite();
    }

    private void SetProductAlpha()
    {
        float alpha = 1f;
        if (!purchasePermitted)
        {
            alpha = 0.5f;
        }
        if (itemImage != null)
        {
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, alpha);
        }
    }

    public void SetButtonSprite()
    {
        buttonImage.sprite = GetSprite();
    }

    public Sprite GetSprite()
    {
        if (purchasePermitted)
        {
            return tradeEnabledSprite;
        }
        else
        {
            return tradeDisabledSprite;
        }
    }

    public void DisplayPrice()
    {
        if (purchasePermitted)
        {
            string priceText = SlotData.Empty() ? "" : (SlotData.item.price).ToString();
            if (price != null)
            {
                if (itemName != null)
                {
                    itemName.text = SlotData.item.ID;
                }
                price.text = $"Purchase : {priceText}";
            }
        }
        else
        {
            if (itemName != null)
            {
                itemName.text = string.Empty;
            }
            if (price != null)
            {
                price.text = string.Empty;
            }
        }
    }
    #endregion

    #region Button Update Methods
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (!holdToTrade)
        {
            TryTrade();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (holdToTrade)
        {
            purchaseCoroutine = StartCoroutine(PerformPurchasesAtRate(6));
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (holdToTrade)
        {
            if (purchaseCoroutine != null)
            {
                StopCoroutine(purchaseCoroutine);
            }
        }
    }
    #endregion

    #region Purchase Methods
     public virtual void TryTrade()
    {
        if (CanAfford())
        {
            Purchase();
            GiveItem();
            tradeFeedback?.PlayFeedbacks();
        }
        else
        {
            tradeFailedFeedback.PlayFeedbacks();
        }
    }

    public virtual bool CanAfford()
    {
        //if (!canPurchase)
        //{
        //    return false;
        //}

        return SlotData.CanPurchase();
    }

    public void Purchase()
    {
        EventItem cost = new(UIService.Instance.currencyObject, SlotData.item.price);
        InventoryEvent.Trigger(InventoryEventType.ItemConsumed, cost);
    }

    public void GiveItem()
    {
        EventItem itemBought = new(SlotData.item, SlotData.quantity);
        InventoryEvent.Trigger(InventoryEventType.ItemPurchased, itemBought);
    }

    private IEnumerator PerformPurchasesAtRate(int purchasesPerSecond)
    {
        float currentPurchasesPerSecond = purchasesPerSecond;
        while (true)
        {
            for (int i = 0; i < purchasesPerSecond; i++)
            {
                TryTrade();
                yield return new WaitForSeconds(1f/currentPurchasesPerSecond);
                if (currentPurchasesPerSecond < 10f)
                {
                    currentPurchasesPerSecond += 0.5f;
                }
            }
        }
    }

    private void CheckRequirementsAreNowMet(ItemObject item)
    {
        if (SlotData.Empty() || SlotData.item.requiredItem == null)
        {
            return;
        }

        if (item.ID == SlotData.item.requiredItem.ID)
        {
            if (!purchasePermitted)
            {
                SetPurchaseMode(true);
            }
        }
    }
    #endregion

    #region TEvent Methods
    public void OnEvent(InventoryEvent eventData)
    {
        if (eventData.eventItem == null || eventData.eventItem.item == null)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case InventoryEventType.ItemPurchased:
                CheckRequirementsAreNowMet(eventData.eventItem.item);
                break;
            case InventoryEventType.ContentUpdated:
                SetPurchaseMode(CanAfford());
                break;
        }
    }

    protected virtual void OnEnable()
    {
        this.Subscribe<InventoryEvent>();
    }

    protected virtual void OnDisable()
    {
        this.Unsubscribe<InventoryEvent>();
    }
    #endregion
}

public static class PurhcaseMethods
{
    public static bool CanPurchase(this ItemSlotData itemData)
    {
        if (itemData.item.price == 0) return true;

        if (itemData.item.price > PlayerManager.Instance.inventory.FindQuantityOfItem(UIService.Instance.currencyObject))
        {
            return false;
        }
        if (PlayerManager.Instance.inventory.AvailableSpaceForItem(itemData.item) < itemData.quantity)
        {
            return false;
        }
        return true;
    }
}
