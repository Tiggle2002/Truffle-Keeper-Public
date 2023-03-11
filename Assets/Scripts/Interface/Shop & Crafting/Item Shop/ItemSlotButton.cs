using UnityEngine.EventSystems;

public class ItemSlotButton : SelectableButton
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        pointerClickFeedback?.PlayFeedbacks();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterFeedback?.PlayFeedbacks();
        InterfaceButton button = GetComponentInChildren<ItemPurchaseButton>();
        InterfaceEvent.Trigger(InterfaceEventType.SlotSelected, correspondingInterface, button);
    }
}
