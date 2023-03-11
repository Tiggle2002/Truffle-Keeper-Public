using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, TEventListener<InventoryEvent>
{
    #region Variables
    public int availableSlotCount 
    { 
        get 
        { 
            return content.Length - filledSlotCount; 
        } 
    }
    protected int filledSlotCount
    {
        get
        {
            int filledSlotCounter = 0;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].item != null)
                {
                    filledSlotCounter++;
                }
            }
            return filledSlotCounter;
        }
    }
    #endregion

    #region References
    [TableList]
    public SlotData[] content;
    #endregion

    #region Content Changing Methods
    public void AddItemToInventory(ItemObject itemToAdd, int quantityToAdd)
    {
        List<int> slotsWithItem = this.FindSlotsWithItemAndSpace(itemToAdd);
        if (itemToAdd.maxStackQuantity > 1 && slotsWithItem.Count > 0)
        {
            AddItemToSlots(itemToAdd, quantityToAdd, slotsWithItem);
        }
        else
        {
            while (quantityToAdd > 0)
            {
                if (quantityToAdd > itemToAdd.maxStackQuantity)
                {
                    AddItemToInventory(itemToAdd, itemToAdd.maxStackQuantity);
                    quantityToAdd -= itemToAdd.maxStackQuantity;
                }
                else
                {
                    AddItem(itemToAdd, quantityToAdd);
                    quantityToAdd = 0;
                }
            }
        }

        InventoryEvent.Trigger(InventoryEventType.ContentUpdated, eventInventory: this);
    }

    private void AddItemToSlots(ItemObject itemToAdd, int quantityToAdd, List<int> slotsWithItem)
    {
        for (int i = 0; i < slotsWithItem.Count; i++)
        {
            int slotIndex = slotsWithItem[i];
            if (content[slotIndex].quantity < itemToAdd.maxStackQuantity)
            {
                content[slotIndex].quantity += quantityToAdd;

                if (content[slotIndex].quantity > itemToAdd.maxStackQuantity)
                {
                    ItemObject item = itemToAdd;
                    int remainingQuantity = content[slotIndex].quantity - itemToAdd.maxStackQuantity;
                    content[slotIndex].quantity = itemToAdd.maxStackQuantity;

                    if (remainingQuantity > 0)
                    {
                        AddItemToInventory(item, remainingQuantity);
                    }
                }
                return;
            }
        }
    }

    private void AddItem(ItemObject itemToAdd, int quantity)
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].item == null && content[i].CanContainItem(itemToAdd))
            {
                content[i].item = itemToAdd.Copy();
                content[i].quantity = quantity;
                return;
            }
        }
    }

    public void RemoveItem(ItemObject itemToRemove, int quantity)
    {
        List<int> indexes = this.FindSlotsWithItem(itemToRemove);

        if (indexes.Count == 0)
        {
            return;
        }

        int currentQuantity = this.FindQuantityOfItem(itemToRemove);

        if (currentQuantity < quantity)
        {
            Debug.LogError("Inventory Error : Trying to Remove More than is Available!");
        }

        int quantityToRemove = quantity;
        for (int i = 0; i < indexes.Count; i++)
        {
            int index = indexes[i];
            if (content[index].quantity >= quantityToRemove)
            {
                content[index].quantity -= quantityToRemove;
                if (content[index].quantity <= 0)
                {
                    content[index].Clear();
                }
                quantityToRemove = 0;
                break;
            }
            else
            {
                quantityToRemove -= content[index].quantity;
                content[index].Clear();
            }
        }
        InventoryEvent.Trigger(InventoryEventType.ContentUpdated, eventInventory: this);

        int remainingQuantity = currentQuantity - quantity;

        if (remainingQuantity <= 0)
        {
            InventoryEvent.Trigger(InventoryEventType.ItemDepleted, new(itemToRemove, 0));
        }
    }

    private void SwapItems(int indexA, int indexB)
    {
        SlotData tempA = new SlotData(content[indexA].item, content[indexA].quantity, content[indexA].containableItems);

        content[indexA] = content[indexB];
        content[indexB] = tempA;
    }
    #endregion

    #region Interface Methods
    public void OnEnable()
    {
        this.Subscribe<InventoryEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<InventoryEvent>();
    }

    public void OnEvent(InventoryEvent eventData)
    {
        switch (eventData.eventType)
        {
            case InventoryEventType.ItemAdded:
            AddItemToInventory(eventData.eventItem.item, eventData.eventItem.quantity);
            break;
            case InventoryEventType.ItemPurchased:
            AddItemToInventory(eventData.eventItem.item, eventData.eventItem.quantity);
            break;
            case InventoryEventType.ItemRemoved:
            RemoveItem(eventData.eventItem.item, eventData.eventItem.quantity);
            break;
            case InventoryEventType.ItemConsumed:
            RemoveItem(eventData.eventItem.item, eventData.eventItem.quantity);
            break;
            case InventoryEventType.ContentRearranged:
            SwapItems(eventData.slotA, eventData.slotB);
            break;
        };
    }
    #endregion
}

[System.Serializable]
public class SlotData
{
    [TableColumnWidth(1)]
    public ItemObject item;
    public ItemType[] containableItems;
    [TableColumnWidth(1)]
    public int quantity;

    public SlotData(ItemObject item, int quantity, ItemType[] containableItems)
    {
        this.item = item;
        this.quantity = quantity;
        this.containableItems = containableItems;
    }

    public bool CanContainItem(ItemObject item)
    {
        for (int i = 0; i < containableItems.Length; i++)
        {
            if (containableItems[i] == item.type)
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    public bool Empty()
    {
        return item == null;
    }
}

public static class ContentChecker
{
    public static List<int> FindSlotsWithItem(this Inventory container, ItemObject item)
    {
        List<int> indexOfSlotsWithItem = new List<int>();

        for (int i = 0; i < container.content.Length; i++)
        {
            if (container.content[i].item != null)
            {
                if (container.content[i].item.ID == item.ID)
                {
                    indexOfSlotsWithItem.Add(i);
                }
            }
        }
        return indexOfSlotsWithItem;
    }

    public static List<int> FindSlotsWithItemAndSpace(this Inventory container, ItemObject item)
    {
        List<int> indexOfSlotsWithItem = new List<int>();

        for (int i = 0; i < container.content.Length; i++)
        {
            if (container.content[i].item != null)
            {
                if (container.content[i].item.ID == item.ID)
                {
                    if (container.content[i].quantity < item.maxStackQuantity)
                    {
                        indexOfSlotsWithItem.Add(i);
                    }
                }
            }
        }
        return indexOfSlotsWithItem;
    }

    public static int AvailableSpaceForItem(this Inventory container, ItemObject item)
    {
        int availableSpace = 0;
        SlotData[] compatibleSlots = container.GetCompatibleSlots(item);

        for (int i = 0; i < compatibleSlots.Length; i++)
        {
            if (compatibleSlots[i].item == null)
            {
                availableSpace += item.maxStackQuantity;
            }
            else if (compatibleSlots[i].item.ID == item.ID)
            {
                availableSpace += item.maxStackQuantity - compatibleSlots[i].quantity;
            }
        }
        return availableSpace;
    }

    public static SlotData[] GetCompatibleSlots(this Inventory container, ItemObject itemObject)
    {
        List<SlotData> compatibleSlots = new();
        for (int i = 0; i < container.content.Length; i++)
        {
            if (container.content[i].CanContainItem(itemObject))
            {
                compatibleSlots.Add(container.content[i]);
            }
        }
        return compatibleSlots.ToArray();
    }

    public static int FindQuantityOfItem(this Inventory container, ItemObject item)
    {
        int quantity = 0;
        for (int i = 0; i < container.content.Length; i++)
        {
            if (container.content[i].item == null)
            {
                continue;
            }
            if (container.content[i].item.ID == item.ID)
            {
                quantity += container.content[i].quantity;
            }
        }
        return quantity;
    }

    public static bool CanContainItem(this Inventory container, ItemType itemType)
    {
        ItemType[] containableItems = container.GetContainableItems();
        for (int i = 0; i < containableItems.Length; i++)
        {
            if (containableItems[i] == itemType)
            {
                return true;
            }
        }
        return false;
    }

    public static ItemType[] GetContainableItems(this Inventory container)
    {
        List<ItemType> containableItems = new();
        for (int i = 0; i < container.content.Length; i++)
        {
            if (container.content[i] == null)
            {
                continue;
            }
            for (int j = 0; j < container.content[i].containableItems.Length; j++)
            {
                if (!containableItems.Contains(container.content[i].containableItems[j]))
                {
                    containableItems.Add(container.content[i].containableItems[j]);
                }
            }
        }
        return containableItems.ToArray();
    }
}

