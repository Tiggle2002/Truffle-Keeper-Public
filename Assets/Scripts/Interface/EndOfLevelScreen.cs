using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfLevelScreen : MonoBehaviour, TEventListener<GameEvent>, TEventListener<WaveEvent>
{
    [Range(0, 0.0001f)]
    [SerializeField] private float counterDelay;

    #region References
    [FoldoutGroup("References")]
    [SerializeField] private CanvasGroup deathScreen;
    [FoldoutGroup("References")]
    [SerializeField] private TextMeshProUGUI playTimeCounter;
    [FoldoutGroup("References")]
    [SerializeField] private TextMeshProUGUI enemiesDefeatedCounter;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] MMF_Player screenSpawn;
    [SerializeField] MMF_Player levelFailure;
    #endregion

    #region Variables
    private float playTime;
    private int playTimeCount;
    private int enemyKilledCount;
    private int trufflesEarnedCount;
    private float startTime;
    private float EndTime;
    private bool screenDisplayed;
    #endregion

    #region Update Methods
    public void Awake()
    {
        enemyKilledCount = 0;
        startTime = Time.time;
    }

    public void Update()
    {
        if (!deathScreen.gameObject.activeSelf)
            playTime += Time.deltaTime;
    }
    #endregion

    #region Methods
    private IEnumerator DisplayScreen()
    {
        screenDisplayed = true;
        EndTime = Time.time - startTime;
        deathScreen.gameObject.SetActive(true);
        
        yield return screenSpawn?.PlayFeedbacksCoroutine(transform.position);

        StartCoroutine(CountToTime(playTimeCounter, EndTime, counterDelay));
        if (enemyKilledCount > 0)
        {
            StartCoroutine(CountToNumber(enemiesDefeatedCounter, enemyKilledCount, counterDelay));
        }
        else
        {
            enemiesDefeatedCounter.text = "0";
        }
    }

    private IEnumerator CountToNumber(TextMeshProUGUI counter, float count, float counterDelay)
    {
        for (int i = 1; i <= count; i++)
        {
            counter.text = i.ToString();
            yield return new WaitForSeconds(counterDelay);
        }
    }

    private IEnumerator CountToTime(TextMeshProUGUI counter, float count, float counterDelay)
    {
        for (float i = 1; i <= count; i += 0.5f)
        {
            counter.text = GetTimeAsString(i);
            yield return new WaitForSeconds(counterDelay);
        }
    }

    private void SetFailureScreen()
    {
        levelFailure?.PlayFeedbacks();
    }

    private string GetTimeAsString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = Mathf.FloorToInt(seconds % 60);

        return $"{minutes} : {seconds} : {milliseconds}";
    }
    #endregion

    #region Events
    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                if (!screenDisplayed)
                {
                    SetFailureScreen();
                    StartCoroutine(DisplayScreen());
                }
                break;
        }
    }
    
    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.EnemyKilled:
                enemyKilledCount++;
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<GameEvent>();
        this.Subscribe<WaveEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<GameEvent>();
        this.Unsubscribe<WaveEvent>();
    }
    #endregion
}
