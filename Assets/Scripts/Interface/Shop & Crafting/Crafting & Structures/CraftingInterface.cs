using Sirenix.OdinInspector;
using SurvivalElements;
using UnityEngine;

/// <summary>
/// Displays Crafting Recipes
/// </summary>
public class CraftingInterface : BaseItemInterface, TEventListener<InventoryEvent>
{
    #region Fields
    protected CraftingMaterial[] craftingRecipe;
    #endregion

    public override void OpenInterface()
    {
        base.OpenInterface();
        UpdateInterface();
    }

    protected override void UpdateInterface()
    {
        if (craftingRecipe == null) return;

        for (int i = 0; i < craftingRecipe.Length; i++)
        {
            UpdateResourceSlots(i);
            SetSlotSprite(i);
        }
        DisableUnusedResourceSlots(craftingRecipe.Length);
    }

    private void UpdateResourceSlots(int index)
    {
        Debug.Log(this.gameObject.name);
        ItemObject itemObject = craftingRecipe[index].item;
        int itemQuantity = craftingRecipe[index].quantity;

        if (itemObject == null || itemObject.ID == null)
        {
            itemSlots[index].gameObject.SetActive(false);
            itemSlots[index].Clear();
        }
        else
        {
            itemSlots[index].gameObject.SetActive(true);
            itemSlots[index].UpdateSlot(itemObject, itemQuantity);
        }
    }

    private void DisableUnusedResourceSlots(int startIndex)
    {
        for (int i = startIndex; i < itemSlots.Length; i++)
        {
            itemSlots[i].Deactivate();
        }
    }

    protected void SetSlotSprite(int index)
    {
        Sprite slotSprite = GetResourceSlotSprite(index);

        itemSlots[index].SetSlotImage(slotSprite);
    }

    public Sprite GetResourceSlotSprite(int index)
    {
        if (craftingRecipe[index].quantity > PlayerManager.Instance.inventory.FindQuantityOfItem(craftingRecipe[index].item))
        {
            return UIService.Instance.notEnoughResourcesSlot;
        }
        return UIService.Instance.enoughResourcesSlot;
    }

    public void OnEvent(InventoryEvent eventData)
    {
        switch (eventData.eventType)
        {
            case InventoryEventType.ContentUpdated:
                UpdateInterface();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<InventoryEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<InventoryEvent>();
    }
}
