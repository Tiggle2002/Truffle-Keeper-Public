using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System.Collections;
using UnityEngine;

public class HealthBar : ProgressBar, TEventListener<HealthEvent>, TEventListener<GameEvent>
{
    #region References
    [SerializeField, FoldoutGroup("Bar Properties")] private bool hideOnGameFailed = false;


    [FoldoutGroup("References") ,HideIf("playerHealth")]
    [SerializeField, Required] private Health correspondingHealth;
        
     public Health CorrespondingHealth()
    {
        if (correspondingHealth == null)
        {
            if (playerHealth)
            {
                correspondingHealth = PlayerManager.Instance?.GetComponent<Health>();
            }
            else
            {
                correspondingHealth = GetComponentInParent<Health>();
            }
        }
        return correspondingHealth;
    }

    [FoldoutGroup("Feedbacks")]
    [SerializeField] private MMF_Player healthRaisedFeedback;
    [FoldoutGroup("Feedbacks")]
    [SerializeField] private MMF_Player healthLoweredFeedback;
    #endregion

    [SerializeField] private bool playerHealth;

    #region Unity Update Methods
    protected override void Start()
    {
        base.Start();
        if (playerHealth)
        {
            correspondingHealth = PlayerManager.Instance?.GetComponent<Health>();
        }
        else if (correspondingHealth == null)
        {
            correspondingHealth = GetComponentInParent<Health>();
        }
    }
    #endregion

    #region Methods
    private void PlayHealthChangedEffects(float newHealth)
    {
        if (singleBar)
        {
            if (newHealth < bar.fillAmount)
            {
                healthLoweredFeedback?.PlayFeedbacks();
            }
            else if (newHealth > bar.fillAmount)
            {
                healthRaisedFeedback?.PlayFeedbacks();
            }    
        }
        else
        {
            if (newHealth < bars[0]?.fillAmount)
            {
                healthLoweredFeedback?.PlayFeedbacks();
            }
            else if (newHealth > bars[0]?.fillAmount)
            {
                healthRaisedFeedback?.PlayFeedbacks();
            }
        }
    }
    #endregion

    #region Event Methods
    public void OnEvent(HealthEvent eventData)
    {
      if (eventData.correspondingHealth != CorrespondingHealth())
        {
            return;
        }
      if (!interfaceOpen)
        {
            OpenInterface();

            if (!startOpen)
            StartCoroutine(this.CloseCoroutine(15f));
        }

        switch (eventData.eventType)
        {
            case HealthEventType.Damaged:
                PlayHealthChangedEffects(eventData.healthPercentage.PercentageToDecimal());
                UpdateBar(eventData.healthPercentage.PercentageToDecimal());
                break;
            case HealthEventType.Changed:
                PlayHealthChangedEffects(eventData.healthPercentage.PercentageToDecimal());
                UpdateBar(eventData.healthPercentage.PercentageToDecimal());
                break;
            case HealthEventType.Respawn:
                Show(true);  
                ResetBar();
                 break;
            case HealthEventType.Death:
                Show(false);
                break;
            case HealthEventType.Spawn:
                ResetBar(eventData.healthPercentage.PercentageToDecimal());
                break;

        }
    }

    protected override void OnEnable()
    {
        this.Subscribe<HealthEvent>();
        this.Subscribe<GameEvent>();
    }

    protected override void OnDisable()
    {
        this.Unsubscribe<HealthEvent>();
        this.Unsubscribe<GameEvent>();
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                if (hideOnGameFailed)
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }
    #endregion
}
