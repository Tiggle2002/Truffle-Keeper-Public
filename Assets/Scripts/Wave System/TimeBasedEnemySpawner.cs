using System.Collections;
using UnityEngine;

public class TimeBasedEnemySpawner : NPCSpawnManager, TEventListener<TimeEvent>
{
    protected override void InitialiseManager()
    {
        
    }

    protected override float CalculateNPCFrequencyForSpawner(BaseNPCSpawner spawner)
    {
        return Random.Range(1f, 5f);
    }

    public void OnEvent(TimeEvent eventData)
    {
        if (eventData.eventType == TimeType.Morning || eventData.eventType == TimeType.Day)
        {
            Spawn();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<TimeEvent>();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<TimeEvent>();
    }
}