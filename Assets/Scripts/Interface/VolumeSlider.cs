using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private MMSoundManager.MMSoundManagerTracks track;
    private Slider slider;
    
    public void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void ChangeVolume()
    {
        MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.SetVolumeTrack, track, slider.value);
    }

    public void OnEnable()
    {
        if (slider != null && MMSoundManager.Instance)
        {
            slider.value = MMSoundManager.Current.GetTrackVolume(track, false);
        }
    }
}
