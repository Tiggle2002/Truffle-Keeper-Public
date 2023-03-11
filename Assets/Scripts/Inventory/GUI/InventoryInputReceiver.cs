using SurvivalElements;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputReceiver : SingletonMonoBehaviour<InventoryInputReceiver>
{
    private InputAction toggleInventory;
    private InputAction changeActiveHotbarSlot;
    private ContainerInterface containerInterface;

    public void Start()
    {
        containerInterface = GetComponent<ContainerInterface>();
        toggleInventory = PlayerManager.Instance.PlayerInput.actions.FindAction("Toggle Inventory");
        changeActiveHotbarSlot = PlayerManager.Instance.PlayerInput.actions.FindAction("Change Active HotBar Slot");
    }

    public void Update()
    {
        if (GameManager.Instance.paused) return;
   
        RegisterInventoryToggle();
        RegisterNumberInput();
        RegisterScroll();
    }

    private void RegisterInventoryToggle()
    {
        if (toggleInventory.WasPressedThisFrame())
        {
            HUDEvent.Trigger(HUDEventType.InterfaceToggled, containerInterface);
        }
    }

    private void RegisterNumberInput()
    {
        if (Input.inputString == "") return;

        int number;
        if (Int32.TryParse(Input.inputString, out number) && number >= 0 && number < 10)
        {
            if (number != 0)
            {
                number--;
            }
            HUDEvent.Trigger(HUDEventType.SlotSelected, number);
        }
    }

    private void RegisterScroll()
    {
        float mouseWheelValue = changeActiveHotbarSlot.ReadValue<float>();

        if (mouseWheelValue != 0)
        {
            PublishScrollIncrement(mouseWheelValue);
        }
     }

    private void PublishScrollIncrement(float scrollValue)
    {
        int indexIncrement = scrollValue < 0 ? 1 : -1;

        HUDEvent.Trigger(HUDEventType.HotbarScroll, indexIncrement);
    }
}
