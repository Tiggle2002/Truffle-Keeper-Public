using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using SurvivalElements;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class ContainerInterface : BaseItemInterface, TEventListener<InventoryEvent>
{
    #region References
    [FoldoutGroup("References")]
    protected Inventory correspondingContainer;
    [FoldoutGroup("References")]
    [SerializeField] private Canvas hotBarCanvas;
    [FoldoutGroup("References")]
    [SerializeField] private Sprite normalSprite;
    [FoldoutGroup("References")]
    [SerializeField] private Sprite selectedSprite;

    [FoldoutGroup("Interface Feedbacks")]
    [SerializeField] private MMF_Player slotSelectedFeedback;
    #endregion

    #region Inventory Slot References
    protected ItemSlot selectedSlot;
    #endregion

    #region Initialisation
    protected override void Awake()
    {
        base.Awake();
        hotBarCanvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        TemporaryItem.Update();
    }

    protected override void SetInitialState()
    {
        base.SetInitialState();
        InitialiseSlots();
        correspondingContainer = PlayerManager.Instance.GetComponent<Inventory>();
        UpdateInterface();
        SelectSlot(0, false);
    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetIndex(i);
        }
    }
    #endregion

    #region Methods
    protected void DetectContentChanges()
    {
        ItemSlotData[] sortedUISlots = itemSlots.Select(slot => slot.SlotData)
                                                                             .OrderBy(slotData => slotData.item?.ID)
                                                                             .ToArray();
        SlotData[] sortedContent = correspondingContainer.content.OrderBy(slotData => slotData.item?.ID)
                                                                          .ToArray();

        for (int i = 0; i < sortedUISlots.Length; i++)
        {
            if (sortedUISlots[i].item.ID == sortedContent[i].item.ID && sortedUISlots[i].quantity != sortedContent[i].quantity)
            {
                itemSlots[i].UpdateSlot(sortedContent[i].item, sortedContent[i].quantity);
            }
        }
    }

    protected override void UpdateInterface()
    {
        for (int i = 0; i < correspondingContainer.content.Length; i++)
        {
            ItemObject itemObject = correspondingContainer.content[i].item;
            int itemQuantity = correspondingContainer.content[i].quantity;
            if (itemObject == null)
            {
                itemSlots[i].gameObject.SetActive(false);
                itemSlots[i].Clear();
            }
            else
            {
                if (!itemSlots[i].gameObject.activeInHierarchy)
                {
                    itemSlots[i].gameObject.SetActive(true);
                }
                itemSlots[i].UpdateSlot(itemObject, itemQuantity);
            }
            itemSlots[i].SetSelectable();
        }
    }
    #endregion

    #region Interface Navigation
    private void SelectSlot(int index, bool playFeedback = true)
    {
        if (selectedSlot == null)
        {
            selectedSlot = itemSlots[0];
            selectedSlot.SetSlotImage(selectedSprite);
        }
        if (index > itemSlots.Length)
        {
            return;
        }

        if (itemSlots[index].Selectable == false || selectedSlot.Index == index) 
        {
            return;
        }

        selectedSlot.SetSlotImage(normalSprite);
        selectedSlot = itemSlots[index];
        selectedSlot.SetSlotImage(selectedSprite);

        if (playFeedback)
        {
            slotSelectedFeedback.GetFeedbackOfType<MMF_SquashAndStretch>().SquashAndStretchTarget = selectedSlot.transform;
            slotSelectedFeedback.PlayFeedbacks();
        }

        PublishSelectedItem();
    }

    public bool SelectableSlotsAvailable()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Selectable == true)
            {
                return true;
            }
        }
        return false;
    }

    private void PublishSelectedItem()
    {
        if (selectedSlot.SlotData.Empty())
        {
            ItemEvent.Trigger(ItemEventType.ItemSelected, null);
        }
        else
        {
            ItemEvent.Trigger(ItemEventType.ItemSelected, selectedSlot.SlotData.item);
        }
    }

    private void ScrollThroughHotbar(int increment)
    {
        if (SelectableSlotsAvailable() == false || selectedSlot == null)
        {
            return;
        }
        int newSlotIndex = FindNextSelectableSlot(increment);

        SelectSlot(newSlotIndex);
    }

    private int FindNextSelectableSlot(int increment)
    {
        int newSlotIndex = Utilities.LoopAroundArray(selectedSlot.Index, increment, itemSlots.Length);

        if (itemSlots[newSlotIndex].Selectable)
        {
            return newSlotIndex;
        }
        else
        {
            increment += increment > 0 ? 1 : -1;
            return FindNextSelectableSlot(increment);
        }
    }

    public override void CloseInterface()
    {
        base.CloseInterface();
        SelectSlot(0);
        TemporaryItem.SetEnabled(false);
    }

    public override void OpenInterface()
    {
        base.OpenInterface();
        TemporaryItem.SetEnabled(true);
    }

    protected override void SetInteractionEnabled(bool shouldPrevent)
    {
        base.SetInteractionEnabled(shouldPrevent);
        hotBarCanvas.GetComponent<GraphicRaycaster>().enabled = shouldPrevent;
    }
    #endregion

    #region Event Methods
    public override void OnEvent(HUDEvent information)
    {
        base.OnEvent(information);

        switch (information.eventType)
        {
            case HUDEventType.SlotSelected:
                SelectSlot(information.slotIndex);
                break;
            case HUDEventType.HotbarScroll:
                  ScrollThroughHotbar(information.slotIndex);
                break;
        }
    }

    public void OnEvent(InventoryEvent eventData)
    {
        if (eventData.eventInventory != correspondingContainer)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case InventoryEventType.ItemPurchased:
            case InventoryEventType.ContentUpdated:
                UpdateInterface();
                PublishSelectedItem();
                break;
            case InventoryEventType.ItemDepleted:
                UpdateInterface();
                break;
            case InventoryEventType.ContentRearranged:
                PublishSelectedItem();
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
    #endregion
}



