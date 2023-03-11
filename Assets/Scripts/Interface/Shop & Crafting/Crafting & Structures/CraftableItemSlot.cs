using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftableItemSlot : ItemSlot
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        EventItem itemToCraft = new(SlotData.item, SlotData.quantity);
        CraftingEvent.Trigger(CraftingEventType.SetRecipe, SlotData.item.craftingRecipe, GetComponentInParent<CraftingInterface>(), itemToCraft);
    }
}