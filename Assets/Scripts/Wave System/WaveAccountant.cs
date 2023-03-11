using System;
using UnityEngine;

public class WaveAccountant : IDisposable, TEventListener<WaveEvent>
{
    #region Variables
    private int totalPoints;
    private int currentPoints;
    private int waveCount;
    #endregion

    public WaveAccountant()
    {
        this.Subscribe<WaveEvent>();
    }

    #region Data Finders
    public bool WaveCompleted()
    {
        return currentPoints >= totalPoints;
    }

    public int WavePercentageDone()
    {
        return Mathf.RoundToInt(currentPoints / (float)totalPoints * 100);
    }
    #endregion

    #region Methods
    public void SetMaxPoints(int wavePoints)
    {
        this.totalPoints = wavePoints;
        currentPoints = 0;
        waveCount++;
    }
    public void UpdateCurrentWavePoints(int enemyPoints)
    {
        currentPoints += enemyPoints;

        if (currentPoints <= totalPoints)
        {
            WaveEvent.Trigger(WaveEventType.WaveProgress, WavePercentageDone(), waveCount);
        }

        if (currentPoints == totalPoints)
        {
            WaveEvent.Trigger(WaveEventType.WaveDefeated, WavePercentageDone(), waveCount);
        }
    }
    #endregion

    #region Event Methods
    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.EnemyKilled:
                UpdateCurrentWavePoints(eventData.points);
                break;
        }
    }

    public void Dispose()
    {
        this.Unsubscribe<WaveEvent>();
    }
    #endregion
}