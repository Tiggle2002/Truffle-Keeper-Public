using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using SurvivalElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : SingletonMonoBehaviour<GameManager>, TEventListener<GameEvent>
{
    public bool paused { get; private set; }
    private Canvas optionsMenu;

    private MMTimeManager timeManager;

    #region Unity Update Methods
    public override void Awake()
    {
        base.Awake();
        optionsMenu = FindObjectOfType<OptionsMenu>().GetComponent<Canvas>();
        timeManager = GetComponent<MMTimeManager>();
    }

    public void Start()
    {
        optionsMenu.enabled = false;
        MMSoundManager.Instance.InitializeSoundManager(true);
    }

    public void Update()
    {
        CheckForPauseInput();
    }
    #endregion

    #region Pausing & Options Methods
    private void CheckForPauseInput()
    {
        if (PlayerManager.Instance.PlayerInput.actions["Pause"].WasPressedThisFrame())
        {
            if (paused)
            {
                CloseOptionsMenu();

            }
            else
            {
                OpenOptionsMenu();
            }
        }
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.enabled = true;
        Pause();
        MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PauseTrack, MMSoundManager.MMSoundManagerTracks.Sfx);
        MMPlaylist.Instance.Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        if (timeManager)
        {
            timeManager.enabled = false;
        }
        paused = true;
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.enabled = false;
        Unpause();
        MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PlayTrack, MMSoundManager.MMSoundManagerTracks.Sfx);
        MMPlaylist.Instance.Play();
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        if (timeManager)
        {
            timeManager.enabled = true;
        }
        paused = false;
    }
    #endregion

    public void Quit()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                //MMPlaylist.Instance.Pause();
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<GameEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<GameEvent>();
    }
}
