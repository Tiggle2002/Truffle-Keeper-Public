using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class WaveManager : WaveBasedNPCSpawner
{
    public int WaveCount { get { return waveCount; } }

    #region Serialized Variables
    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    protected float distanceMultiplier = 0.01f;

    [SerializeField] private static int waveCount = 0;
    [SerializeField, Min(1)] private int startWaveIndex;
    [SerializeField] private MMF_Player waveStartedFeedback;
    #endregion

    #region Variables
    private WaveAccountant waveAccountant;
    private int totalWavePoints;
    #endregion

    #region Initialisation
    protected override void InitialiseManager()
    {
        waveCount = startWaveIndex;
        waveAccountant = new WaveAccountant();

        AddSpawnCondition(enemy => waveCount >= enemy.minimumWaveReached);

        AddActionBeforeWave(() => totalWavePoints = 0);
        AddActionBeforeWave(PublishWaveStarted);

        AddActionAfterWave(PublishWaveStarted);
        AddActionAfterWave(IncrementWaveCount);
        AddActionAfterWave(SetWavePoints);
    }
    #endregion

    private void IncrementWaveCount()
    {
        waveCount++;
    }

    private void PublishWaveStarted()
    {
        waveStartedFeedback?.PlayFeedbacks();
        WaveEvent.Trigger(WaveEventType.WaveBegun, waveCount: waveCount);
    }

    private void SetWavePoints()
    {
        SumEnemyPoints(npcsJustSpawned.ToArray());
        waveAccountant.SetMaxPoints(totalWavePoints);
    }

    private void SumEnemyPoints(NPC[] enemies)
    {
        foreach (NPC npc in enemies)
        {
            totalWavePoints += npc.obj.GetComponentInChildren<EnemyHealth>(true).GetPoints();
        }
    }

    protected override float CalculateNPCFrequencyForSpawner(BaseNPCSpawner spawner)
    {
        EnemySpawner enemySpawner = spawner as EnemySpawner;

        if (enemySpawner)
        {
            return baseNPCCount + (waveCount * waveMultipler) + (enemySpawner.DistanceFromMonument * distanceMultiplier);
        }

        Debug.Log($"Incorrect Spawner Attached to {gameObject.name}");
        return 0f;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        waveAccountant?.Dispose();
    }

    public override void OnEvent(TimeEvent eventData)
    {
        switch (eventData.eventType)
        {
            case TimeType.Night:
                Spawn();
                break;
        }
    }
}





