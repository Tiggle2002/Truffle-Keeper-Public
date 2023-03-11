using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public interface EventListenerBase { }

public interface TEventListener<T> : EventListenerBase
{
    void OnEvent(T eventData);
}

public struct EventBase
{
    public string eventName;

    public EventBase(string eventName)
    {
        this.eventName = eventName;
    }

    static EventBase eventToCall;

    public static void Trigger(string eventName)
    {
        eventToCall.eventName = eventName;

        EventBus.TriggerEvent(eventToCall);
    }
}

public static class EventBus
{
    private static Dictionary<Type, List<EventListenerBase>> listenerMap;

    static EventBus()
    {
        listenerMap = new Dictionary<Type, List<EventListenerBase>>();
    }

    public static void AddListener<Event>(TEventListener<Event> listenerToAdd) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!listenerMap.ContainsKey(eventType))
        {
            listenerMap.Add(eventType, new List<EventListenerBase>());
        }

        if (!CheckMapForListener(eventType, listenerToAdd))
        {
            listenerMap[eventType].Add(listenerToAdd);
        }
    }

    public static bool CheckMapForListener(Type eventType, EventListenerBase listener)
    {
        List<EventListenerBase> listeners;

        if (!listenerMap.TryGetValue(eventType,out listeners)) return false;

        for (int i = listeners.Count - 1 ; i >= 0; i--)
        {
            if (listeners[i] == listener)
            {
                return true;
            }
        }

        return false;
    }

    public static void RemoveListener<Event>(TEventListener<Event> listener) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!listenerMap.ContainsKey(eventType))
        {
            return;
        }

        if (listenerMap[eventType].Contains(listener))
        {
            int listenerIndex = listenerMap[eventType].IndexOf(listener);

            listenerMap[eventType].RemoveAt(listenerIndex);

            if (listenerMap[eventType].Count == 0)
            {
                listenerMap.Remove(eventType);
            }
        }
    }

    public static void RemoveAllListeners(EventListenerBase listener)
    {
        List<EventListenerBase> listeners;
        Type eventType;

        foreach (KeyValuePair<Type, List<EventListenerBase>> entry in listenerMap)
        {
            eventType = entry.Key;
            listeners = entry.Value;

            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);

                if (listeners.Count == 0)
                {
                    listenerMap.Remove(eventType);
                }
            }
        }
    }

    public static void TriggerEvent<Event>(Event eventToTrigger) where Event : struct
    {
        List<EventListenerBase> listeners;

        Type eventType = typeof(Event);

        if (listenerMap.TryGetValue(eventType, out listeners))
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                (listeners[i] as TEventListener<Event>).OnEvent(eventToTrigger);
            }
        }
    }
}

public static partial class EventReception
{
    public static void Subscribe<Event>(this TEventListener<Event> listener) where Event : struct
    {
        EventBus.AddListener(listener);
    }

    public static void Unsubscribe<Event>(this TEventListener<Event> listener) where Event : struct
    {
        EventBus.RemoveListener(listener);
    }
}