using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;


public class WaveTimer: BaseInterface, TEventListener<WaveEvent>
{
    [FoldoutGroup("UI References"), SerializeField]
    private TextMeshProUGUI timerMesh;
    [FoldoutGroup("UI References"), SerializeField]
    private Canvas timerCanvas;
    [FoldoutGroup("UI References"), SerializeField]
    private Canvas startWaveEarlyCanvas;
    [FoldoutGroup("UI References"), SerializeField]
    private Transform monumentSprite;

    [FoldoutGroup("Timer Feedback"), SerializeField] MMF_Player finalTenSecondsFeedback;

    [FoldoutGroup("Timer Feedback"), SerializeField] MMF_Player finalFifteenSecondsFeedback;

    private Camera playerCam;
    private int waveTimerLength = 60;

    protected override void Awake()
    {
        base.Awake();
        playerCam = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
    }

    public void TriggerWaveEarly()
    {
        StopAllCoroutines();
        timerCanvas.enabled = false;
        startWaveEarlyCanvas.enabled = false;
        WaveEvent.Trigger(WaveEventType.TriggerWaveStart);
    }

    public void Update()
    {
        Vector3 viewportPoint = playerCam.WorldToViewportPoint(new Vector3(0, 2, 0));
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            timerCanvas.gameObject.Unparent();
            timerCanvas.transform.position = new Vector3(0, 23, 1);
        }
        else
        {
            timerCanvas.transform.localPosition = new Vector3(0, 15, 1);
            timerCanvas.transform.SetParent(playerCam.transform);
        }
    }

    public IEnumerator TimeNextWave(int time)
    {
        yield return new WaitForSeconds(1f);
        timerCanvas.enabled = true;
        startWaveEarlyCanvas.enabled = true;
        for (int currentTime = time; currentTime >= 0; currentTime--)
        {
            PlayTimerFeedback(currentTime);
            timerMesh.text = $"Time Until Next Wave: {currentTime.ToString()}";
            yield return new WaitForSeconds(1f);
        }
        timerCanvas.enabled = false;
        startWaveEarlyCanvas.enabled = false;
        WaveEvent.Trigger(WaveEventType.TriggerWaveStart);
    }

    private void PlayTimerFeedback(int time)
    {
        if (time <= 10)
        {
            finalTenSecondsFeedback?.PlayFeedbacks();
        }
        else if (time <= 15)
        {
            finalFifteenSecondsFeedback?.PlayFeedbacks();
        }
    }

    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.WaveDefeated:
                StartCoroutine(TimeNextWave(waveTimerLength));
                if (waveTimerLength < 150)
                {
                    waveTimerLength += 15;
                }
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<WaveEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<WaveEvent>();
    }

    protected override void UpdateInterface()
    {
        throw new System.NotImplementedException();
    }
}
