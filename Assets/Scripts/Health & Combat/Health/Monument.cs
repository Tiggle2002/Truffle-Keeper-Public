using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IAttachedInterface : TEventListener<PlayerEvent>
{
    bool canBeOpened { get; set; }

    void ToggleInterface();

    void SetInteractEnabled(bool enabled);
}

public class Monument : MonoBehaviour, TEventListener<PlayerEvent>
{
    public UnityEvent triggeredEvent;

    private bool repaired = false;

    private bool playerInRange;

    public void OnEvent(PlayerEvent eventData)
    {
        if (eventData.eventType == PlayerEventType.PlayerInteracted && playerInRange)
        {
            triggeredEvent?.Invoke();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !repaired)
        {
            transform.SetInteractable();
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.RemoveInteractable();
            playerInRange = false;
        }
    }
}


