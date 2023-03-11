using System.Collections;
using UnityEngine;

public struct BuildingEvent
{
    public BuildingEventType eventType;

    public BuildingEvent(BuildingEventType eventType)
    {
        this.eventType = eventType;
    }

    static BuildingEvent eventToCall;

    public static void Trigger(BuildingEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum BuildingEventType { Mining, StoppedMining }
