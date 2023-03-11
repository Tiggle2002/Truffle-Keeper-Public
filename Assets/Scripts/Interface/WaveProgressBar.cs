using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : ProgressBar, TEventListener<WaveEvent>, TEventListener<GameEvent>
{
    #region References
    [FoldoutGroup("References")]
    [SerializeField] protected TextMeshProUGUI wavePercentage;
    [FoldoutGroup("References")]
    [SerializeField] protected TextMeshProUGUI waveCounter;
    [FoldoutGroup("References")]
    [SerializeField] protected TextMeshProUGUI waveStartText;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player waveStartedFeedback;
    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player waveEndedFeedback;
    #endregion

    protected void Awake()
    {
        gameObject.transform.localScale = Vector3.zero;
        UpdateWaveCounter(0);
    }

    #region Methods
    private void InitialiseInterface()
    {
        UpdateBar(0);
        UpdateWaveInterface();
        waveStartedFeedback?.PlayFeedbacks();
    }

    private void UpdateWaveInterface(int newPercentage = 0)
    {
        wavePercentage.text = $"Completed : {newPercentage}%";
        UpdateBar(newPercentage.PercentageToDecimal());
    }

    private void UpdateWaveCounter(int waveIndex)
    {
        waveCounter.text = $"Wave {waveIndex}";
    }

    private IEnumerator CloseWaveCounter()
    {
        yield return new WaitForSeconds(0.75f);
        waveEndedFeedback?.PlayFeedbacks();
    }
    #endregion

    #region Event Methods
    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                gameObject.SetActive(false);
                break;
        }
    }

    public void OnEvent(WaveEvent eventData)
    {
        if (!wavePercentage || !waveCounter)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case WaveEventType.WaveBegun:
                InitialiseInterface();
                UpdateWaveCounter(eventData.waveCount);
                break;
            case WaveEventType.WaveProgress:
                UpdateWaveInterface(eventData.points);
                break;
            case WaveEventType.WaveDefeated:
                StartCoroutine(CloseWaveCounter()); 
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<WaveEvent>();
        this.Subscribe<GameEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<WaveEvent>();
        this.Unsubscribe<GameEvent>();
    }
    #endregion
}
