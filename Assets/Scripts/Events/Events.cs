using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurvivalElements;
using Unity.VisualScripting;
using System;

#region Movement Event
public struct PlayerMovementEvent
{
    public MovementEventType eventType;
    public float direction;
    public bool isFacingRight;

    public PlayerMovementEvent(MovementEventType eventType, float direction, bool isFacingRight)
    {
        this.eventType = eventType;
        this.direction = direction;
        this.isFacingRight = isFacingRight;
    }

    static PlayerMovementEvent eventToCall;

    public static void Trigger(MovementEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(MovementEventType eventType, bool isFacingRight)
    {
        eventToCall.eventType = eventType;
        eventToCall.isFacingRight = isFacingRight;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(MovementEventType eventType, float direction = 0)
    {
        eventToCall.eventType = eventType;
        eventToCall.direction = direction;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum MovementEventType { FacingDirection }
#endregion

#region Inventory Event
public struct InventoryEvent
{
    public InventoryEventType eventType;
    public EventItem eventItem;
    public Inventory eventInventory;
    public int slotA;
    public int slotB;

    public InventoryEvent(InventoryEventType eventType, EventItem eventItem, Inventory eventInventory, int slotA, int slotB)
    {
        this.eventType = eventType;
        this.eventItem = eventItem;
        this.eventInventory = eventInventory;
        this.slotA = slotA;
        this.slotB = slotB;
    }

    static InventoryEvent eventToCall;

    public static void Trigger(InventoryEventType eventType, EventItem eventItem = null, Inventory eventInventory = null)
    {
        eventToCall.eventType = eventType;
        eventToCall.eventItem = eventItem;
        eventToCall.eventInventory = eventInventory;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(InventoryEventType eventType, int slotA, int slotB)
    {
        eventToCall.eventType = eventType;
        eventToCall.slotA = slotA;
        eventToCall.slotB = slotB;

        EventBus.TriggerEvent(eventToCall);
    }
}

public class EventItem
{
    public ItemObject item { get; private set; }
    public EntityObject entity { get; private set; }
    public int quantity { get; private set; }

    public EventItem(ItemObject eventItem, int eventItemQuantity)
    {
        this.item = eventItem;
        this.quantity = eventItemQuantity;
    }

    public EventItem(EntityObject eventEntity, int eventItemQuantity)
    {
        this.entity = eventEntity;
        this.quantity = eventItemQuantity;
    }
}

public enum InventoryEventType { ItemAdded, ItemRemoved, ItemConsumed, ItemPurchased, ItemDepleted, ContentUpdated, ContentRearranged }
#endregion

#region HUD Event
public struct HUDEvent
{
    public HUDEventType eventType;
    public BaseInterface eventInterface;
    public InterfaceButton interfaceButton;
    public int slotIndex;

    public HUDEvent(HUDEventType eventType, int slotIndex, BaseInterface eventInterface, InterfaceButton interfaceButton)
    {
        this.eventType = eventType;
        this.slotIndex = slotIndex;
        this.eventInterface = eventInterface;
        this.interfaceButton = interfaceButton;
    }

    static HUDEvent eventToCall;

    public static void Trigger(HUDEventType eventType, BaseInterface eventInterface = null)
    {
        eventToCall.eventType = eventType;
        eventToCall.eventInterface = eventInterface;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(HUDEventType eventType, BaseInterface eventInterface = null, InterfaceButton buttonPressed = null)
    {
        eventToCall.eventType = eventType;
        eventToCall.eventInterface = eventInterface;
        eventToCall.interfaceButton = buttonPressed;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(HUDEventType eventType, int slotIndex)
    {
        eventToCall.eventType = eventType;
        eventToCall.slotIndex = slotIndex;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum HUDEventType { InterfaceUpdated, InterfaceToggled, InterfaceClosed, SlotSelected, HotbarScroll }

public struct InterfaceEvent
{
    public InterfaceEventType eventType;
    public BaseInterface eventInterface;
    public InterfaceButton interfaceButton;
    public SelectableButton shopButton;
    public int shopPage;
    public int slotIndex;


    public InterfaceEvent(InterfaceEventType eventType, int slotIndex, BaseInterface eventInterface, InterfaceButton buttonPressed, int shopPage, SelectableButton shopButton)
    {
        this.eventType = eventType;
        this.slotIndex = slotIndex;
        this.eventInterface = eventInterface;
        this.interfaceButton = buttonPressed;
        this.shopButton = shopButton;
        this.shopPage = shopPage;
    }

    static InterfaceEvent eventToCall;

    public static void Trigger(InterfaceEventType eventType, BaseInterface eventInterface = null, InterfaceButton buttonPressed = null, SelectableButton shopButton = null)
    {
        eventToCall.eventType = eventType;
        eventToCall.eventInterface = eventInterface;
        eventToCall.interfaceButton = buttonPressed;
        eventToCall.shopButton = shopButton;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(InterfaceEventType eventType, int shopPage, BaseInterface eventInterface = null, InterfaceButton buttonPressed = null, SelectableButton shopButton = null)
    {
        eventToCall.eventType = eventType;
        eventToCall.shopPage = shopPage;
        eventToCall.eventInterface = eventInterface;
        eventToCall.interfaceButton = buttonPressed;
        eventToCall.shopButton = shopButton;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum InterfaceEventType { PageSelected, SlotSelected, ButtonUpdated, Opened, Closed }
#endregion

#region Item Event
public struct ItemEvent
{
    public ItemEventType eventType;

    public ItemObject eventItem;


    public ItemEvent(ItemEventType eventType, ItemObject eventItem)
    {
        this.eventType = eventType;

        this.eventItem = eventItem;
    }

    static ItemEvent eventToCall;

    public static void Trigger(ItemEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(ItemEventType eventType, ItemObject eventItem)
    {
        eventToCall.eventType = eventType;

        eventToCall.eventItem = eventItem;

        EventBus.TriggerEvent(eventToCall);
    }

    internal static void Trigger(object firstSwing)
    {
        throw new NotImplementedException();
    }
}

public enum ItemEventType { ItemSelected, ItemUseRequest, ItemUsed, ItemInUse, ItemNoLongerInUse, PlayerClick, FirstSwing, SecondSwing, FirstThrust, ItemUseCancelled, SecondThrust,
    ItemUseRequestHold,
    ItemUseRequestHoldCancelled,
    UseInputHalted,
    FirstStab,
    SecondStab,
    HaltMovement
}
#endregion

#region Wave Event
public struct WaveEvent
{
    public WaveEventType eventType;
    public int points;
    public int waveCount;
    public Vector3 positionOfDeath;

    public WaveEvent(WaveEventType eventType, int points, int waveCount, Vector3 positionOfDeath)
    {
        this.eventType = eventType;
        this.points = points;
        this.waveCount = waveCount;
        this.positionOfDeath = positionOfDeath;
    }

    static WaveEvent eventToCall;

    public static void Trigger(WaveEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(WaveEventType eventType, int points = 0, int waveCount = 0)
    {
        eventToCall.eventType = eventType;
        eventToCall.points = points;
        eventToCall.waveCount = waveCount;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void EnemyDeathAt(Vector3 positionOfDeath, WaveEventType eventType = WaveEventType.EnemyKilled)
    {
        eventToCall.eventType = eventType;
        eventToCall.positionOfDeath = positionOfDeath;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum WaveEventType { EnemyKilled, WaveDefeated, WaveBegun, WaveProgress, TimingNextWave,
    WavesCompleted,
    TriggerWaveStart
}
#endregion

public struct GameEvent
{
    public GameEventType eventType;

    public GameEvent(GameEventType eventType)
    {
        this.eventType = eventType;
    }

    static GameEvent eventToCall;

    public static void Trigger(GameEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum GameEventType 
{
    PlayerDeath, 
    LevelFailed,
}

#region Monument Event
public struct MonumentEvent
{
    public MonumentEventType eventType;
    public CraftingMaterial[] recipe;
    public int upgradeIndex;

    public MonumentEvent(MonumentEventType eventType, CraftingMaterial[] upgradeRecipe, int upgradeIndex)
    {
        this.eventType = eventType;
        this.recipe = upgradeRecipe;
        this.upgradeIndex = upgradeIndex;
    }

    static MonumentEvent eventToCall;

    public static void Trigger(MonumentEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(MonumentEventType eventType, int upgradeIndex)
    {
        eventToCall.eventType = eventType;
        eventToCall.upgradeIndex = upgradeIndex;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(MonumentEventType eventType, CraftingMaterial[] upgradeRecipe)
    {
        eventToCall.eventType = eventType;
        eventToCall.recipe = upgradeRecipe;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(MonumentEventType eventType, CraftingMaterial[] upgradeRecipe, int upgradeIndex)
    {
        eventToCall.eventType = eventType;
        eventToCall.recipe = upgradeRecipe;
        eventToCall.upgradeIndex = upgradeIndex;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum MonumentEventType { Upgraded, UpgradeRecipeChanged, SetRecipe }
#endregion

#region Crafting Event
public struct CraftingEvent
{
    public CraftingEventType eventType;
    public CraftingMaterial[] recipe;
    public CraftingInterface eventCraftingInterface;
    public EventItem itemData;

    public CraftingEvent(CraftingEventType eventType, CraftingMaterial[] upgradeRecipe, CraftingInterface craftingInterface, EventItem item)
    {
        this.eventType = eventType;
        this.recipe = upgradeRecipe;
        this.eventCraftingInterface = craftingInterface;
        this.itemData = item;
    }

    static CraftingEvent eventToCall;

    public static void Trigger(CraftingEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(CraftingEventType eventType, CraftingMaterial[] upgradeRecipe, CraftingInterface craftingInterface)
    {
        eventToCall.eventType = eventType;
        eventToCall.recipe = upgradeRecipe;
        eventToCall.eventCraftingInterface = craftingInterface;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(CraftingEventType eventType, CraftingMaterial[] upgradeRecipe, CraftingInterface craftingInterface, EventItem item)
    {
        eventToCall.eventType = eventType;
        eventToCall.recipe = upgradeRecipe;
        eventToCall.eventCraftingInterface = craftingInterface;
        eventToCall.itemData = item;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum CraftingEventType
{
    SetRecipe, AddRecipe
}
#endregion
