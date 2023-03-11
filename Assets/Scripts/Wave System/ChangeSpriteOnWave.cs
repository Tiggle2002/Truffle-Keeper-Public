using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ChangeSpriteOnWave : BaseWaveReactant
{
    [SerializeField, FoldoutGroup("Sprites")] Sprite[] sprites;
    private SpriteRenderer sr;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();    
    }

    public override void OnEvent(MonumentEvent eventData)
    {
        if (eventData.eventType == MonumentEventType.UpgradeRecipeChanged)
        {
            ChangeSpriteAccordingToWaveIndex(eventData.upgradeIndex);
        }
    }

    private void ChangeSpriteAccordingToWaveIndex(int upgradeIndex)
    {
        if (upgradeIndex < sprites.Length)
        {
            if (sprites[upgradeIndex] != null)
            {
                sr.sprite = sprites[upgradeIndex];
            }    
        }
    }
}