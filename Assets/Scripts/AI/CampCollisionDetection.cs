using System.Collections;
using UnityEngine;

public class CampCollisionDetection : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Health colliderHealth = collision.GetComponentInChildren<Health>();
        if (colliderHealth)
        {
            CampEvent.Trigger(CampEventType.ObjectEntered, colliderHealth.entityObject.entityType);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Health colliderHealth = collision.GetComponentInChildren<Health>();
        if (colliderHealth)
        {
            CampEvent.Trigger(CampEventType.ObjectExited, colliderHealth.entityObject.entityType);
        }
    }
}

public struct CampEvent
{
    public CampEventType eventType;
    public EntityType entityType;

    public CampEvent(CampEventType eventType, EntityType entityType)
    {
        this.eventType = eventType;
        this.entityType = entityType;
    }

    static CampEvent eventToCall;

    public static void Trigger(CampEventType eventType, EntityType entityType)
    {
        eventToCall.eventType = eventType;
        eventToCall.entityType = entityType;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum CampEventType { ObjectEntered, ObjectExited }