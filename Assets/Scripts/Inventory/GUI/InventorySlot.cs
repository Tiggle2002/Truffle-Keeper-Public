using AnimationImporter.PyxelEdit;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : ItemSlot
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        SelectSlot();
    }

    private void SelectSlot()
    {
        if (Index > 9) return;

        if (TemporaryItem.SelectionEnabled())
        {
            if (TemporaryItem.TemporaryItemSelected())
            {
                this.SwapSlots();
            }
            else
            {
                this.SetTemporaryItem(SlotData, Index);
                this.Clear();
            }
        }
        else
        {
            HUDEvent.Trigger(HUDEventType.SlotSelected, Index);
        }
    }
}

public static class TemporaryItem
{
    public static int formerSlotIndex;
    public static ItemSlotData slotData;
    private static GameObject temporaryItem;
    private static Image temporaryItemImage;
    private static TextMeshProUGUI temporaryItemQuantity;
    private static InventorySlot temporaryItemFormerSlot;
    private static Camera playerCam;

    private static bool selectionEnabled = false;

    static TemporaryItem()
    {
        temporaryItem = Resources.Load<GameObject>("Prefabs/UI/Temporary Item Canvas");
        temporaryItem = GameObject.Instantiate(temporaryItem).transform.GetChild(0).gameObject;
        temporaryItem.SetActive(false);

        temporaryItemImage = temporaryItem.GetComponentInChildren<Image>();
        temporaryItemQuantity = temporaryItem.GetComponentInChildren<TextMeshProUGUI>();
        playerCam = Camera.main;
    }

    public static void SetTemporaryItem(this InventorySlot formerSlot, ItemSlotData data, int slotIndex)
    {
        if (!selectionEnabled) return;

        slotData = new(data.item, data.quantity);
        formerSlotIndex = slotIndex;
        temporaryItem.SetActive(true);
        temporaryItemImage.sprite = slotData.item.sprite;
        temporaryItemQuantity.text = slotData.quantity.ToString();
        temporaryItemFormerSlot = formerSlot;
    }

    public static void Update()
    {
        if (!selectionEnabled || !temporaryItem.activeInHierarchy) return;

        Vector3 pos =  Input.mousePosition;
        Vector3 canvasMousePosition = pos;
        temporaryItem.transform.position = new Vector3(canvasMousePosition.x, canvasMousePosition.y, 0f);
    }

    public static void SetEnabled(bool enabled)
    {
        selectionEnabled = enabled;
    }

    public static bool SelectionEnabled() => selectionEnabled;

    public static bool TemporaryItemSelected() => temporaryItem.activeInHierarchy;

    public static void SwapSlots(this InventorySlot slotA)
    {
        if (slotA == temporaryItemFormerSlot)
        {
            temporaryItemFormerSlot.UpdateSlot(slotData.item, slotData.quantity);
        }

        ItemSlotData dataTempA = new(slotA.SlotData.item, slotA.SlotData.quantity);

        slotA.UpdateSlot(slotData.item, slotData.quantity);

        temporaryItemFormerSlot.UpdateSlot(dataTempA.item, dataTempA.quantity);
        temporaryItem.SetActive(false);
        InventoryEvent.Trigger(InventoryEventType.ContentRearranged, slotA.Index, temporaryItemFormerSlot.Index);
    }
}