using UnityEngine;

public abstract class WaveBasedNPCSpawner : NPCSpawnManager, TEventListener<TimeEvent>
{
    public abstract void OnEvent(TimeEvent eventData);

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
