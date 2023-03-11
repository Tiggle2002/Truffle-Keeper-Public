using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public struct StructureEvent
{
    public StructureEventType eventType;
    public StructureData data;
    public StructureSite site;
    public Structure structure;
    public CraftingMaterial[] recipe;

    public StructureEvent(StructureEventType eventType, StructureData data, StructureSite site, Structure structure, CraftingMaterial[] recipe)
    {
        this.eventType = eventType;
        this.data = data;
        this.site = site;
        this.structure = structure;
        this.recipe = recipe;
    }

    static StructureEvent eventToCall;

    public static void Trigger(StructureEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(StructureEventType eventType, StructureSite site)
    {
        eventToCall.eventType = eventType;
        eventToCall.site = site;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(StructureEventType eventType, StructureData data)
    {
        eventToCall.eventType = eventType;
        eventToCall.data = data;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(StructureEventType eventType, Structure structure)
    {
        eventToCall.eventType = eventType;
        eventToCall.structure = structure;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(StructureEventType eventType, StructureData data, StructureSite site)
    {
        eventToCall.eventType = eventType;
        eventToCall.site = site;
        eventToCall.data = data;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(StructureEventType eventType, StructureData data, CraftingMaterial[] recipe)
    {
        eventToCall.eventType = eventType;
        eventToCall.data = data;
        eventToCall.recipe = recipe;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum StructureEventType
{
    Build,
    Upgrade,
    Repair,
    StructureSelected,
    StructureUpgradeAvailable,
    Demolish,
    StateChange,
    HealthChanged,
    NoActiveSite,
    StructureBuilt,
    RepairSelected,
}
