using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class AnimalSpawnerManager : WaveBasedNPCSpawner
{
    #region Serialised Variables
    private static int waveCount = 0;
    #endregion

    public void Start()
    {
        Spawn();
    }

    public override void OnEvent(TimeEvent eventData)
    {
        waveCount++;
        if (eventData.eventType == TimeType.Morning)
        {
             Spawn();
        }
    }

    protected override void InitialiseManager()
    {
        AddSpawnCondition(animal => npcPool.PoolCount(animal) > 0);
        AddSpawnCondition(animal => waveCount >= animal.minimumWaveReached);
    }

    protected override float CalculateNPCFrequencyForSpawner(BaseNPCSpawner spawner)
    {
        return baseNPCCount + (waveCount * waveMultipler);
    }
}
