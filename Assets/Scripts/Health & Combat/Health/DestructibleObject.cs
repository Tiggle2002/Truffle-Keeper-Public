using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : Health
{
    [FoldoutGroup("Loot"), SerializeField]
    protected bool spawnAfterDeath = false;
    [FoldoutGroup("Loot"), SerializeField]
    private Transform lootSpawnPos;
    [FoldoutGroup("Feedbacks"), SerializeField]
    private MMF_Player wrongToolFeedback;

    protected override void InitialiseComponents()
    {
        base.InitialiseComponents();
    }

    public override bool ImmuneToInstigator(DamageInstigator instigator)
    {
        if (instigator is not DamageOnCollision)
        {
            return false;
        }
          
        return !InstigatorIsCorrectTool(instigator);
    }

    private bool InstigatorIsCorrectTool(DamageInstigator instigator)
    {
        if (!entityObject.toolRequired)
        {
            return true;
        }

        ItemObject instigatorTool = instigator.GetComponentInChildren<Item>(true).ItemObject;

        if ( instigatorTool.toolType == entityObject.requiredTool && entityObject.minmumToolTier <= instigatorTool.toolTier)
        {
            return true;
        }
        wrongToolFeedback?.PlayFeedbacks();
        return false;
    }

    public void SpawnLoot()
    {
        if (!entityObject.lootable)
        {
            return;
        }

        Transform spawnTransform = lootSpawnPos == null ? transform : lootSpawnPos;
        Vector3 spawnPos = spawnTransform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(0, 2.5f));
        entityObject.lootToDrop.SpawnMultipleLoot(spawnPos);
    }

    protected override IEnumerator Kill()
    {
        if (!spawnAfterDeath) SpawnLoot();

        yield return deathFeedback?.PlayFeedbacksCoroutine(this.transform.position, 1f, false);

        if (spawnAfterDeath) SpawnLoot();

        if (disableOnDeath)
        {
            gameObject.SetActive(false);
        }
        else if (destroyOnDeath)
        {
            Destroy(this.gameObject);
        }
    }
}

public struct WorldEvent
{
    public WorldEventType eventType;
    public DestructibleObject obstacle;

    public WorldEvent(WorldEventType eventType, DestructibleObject obstacle)
    {
        this.eventType = eventType;
        this.obstacle = obstacle;
    }

    static WorldEvent eventToCall;

    public static void Trigger(WorldEventType eventType)
    {
        eventToCall.eventType = eventType;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void Trigger(WorldEventType eventType, DestructibleObject obstacle)
    {
        eventToCall.eventType = eventType;
        eventToCall.obstacle = obstacle;    

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum WorldEventType { ObstacleDestroyed, BoundsChanged }

