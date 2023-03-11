using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour, TEventListener<WaveEvent>
{
    [SerializeField] private float fadeDuration;
    [SerializeField] private MMPlaylist battlePlaylist;
    [SerializeField] private MMPlaylist calmPlaylist;

    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.WaveBegun:
                PlayBattleMusic();
                break;
            case WaveEventType.WaveDefeated:
                PlayCalmMusic();
                break;
        }
    }

    private void PlayCalmMusic()
    {
        calmPlaylist.FadeInPlaylist(fadeDuration);
        battlePlaylist.FadeOutPlaylist(fadeDuration);
    }

    private void PlayBattleMusic()
    {
        battlePlaylist.FadeInPlaylist(fadeDuration);
        calmPlaylist.FadeOutPlaylist(fadeDuration);
    }

    public void OnEnable()
    {
        this.Subscribe();
    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }
}
