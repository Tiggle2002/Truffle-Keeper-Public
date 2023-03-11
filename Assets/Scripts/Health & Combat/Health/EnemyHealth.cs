using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class EnemyHealth : CharacterHealth
{
    protected override void TriggerDeathEvent()
    {
        base.TriggerDeathEvent();
        WaveEvent.Trigger(WaveEventType.EnemyKilled, points: entityObject.XP);
    }

    public int GetPoints()
    {
        return entityObject.XP;
    }
}


