using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerEvent
{
    public PlayerEventType eventType;

    public PlayerEvent(PlayerEventType eventType)
    {
        this.eventType = eventType;
    }

    static PlayerEvent eventToCall;

    public static void Trigger(PlayerEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum PlayerEventType { PlayerInteracted }
