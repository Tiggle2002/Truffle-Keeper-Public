using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundWallSpawner : MonoBehaviour, TEventListener<MonumentEvent>
{
    [SerializeField] private MMF_Player wallBuildingFeedback;
    [SerializeField] private WallData[] wallUpgrades;
    private SpriteRenderer sr;
    private BoxCollider2D campCollider;
    private ParticleSystem buildParticles;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        campCollider = GetComponent<BoxCollider2D>();
        buildParticles = wallBuildingFeedback.GetFeedbackOfType<MMF_ParticlesInstantiation>().ParticlesPrefab;
    }

    public void OnEvent(MonumentEvent eventData)
    {
        if (eventData.eventType == MonumentEventType.UpgradeRecipeChanged)
        {
            if (eventData.upgradeIndex == 0) return;
            IncreaseWallLength(eventData.upgradeIndex);
        }
    }

    [Button("Increase Length")]
    private void IncreaseWallLength(int index)
    {
        if (index >= wallUpgrades.Length || wallUpgrades[index].wallLength <= 0) return;

        StartCoroutine(BuildWallCoroutine(index));
    }

    private IEnumerator BuildWallCoroutine(int index)
    {
        int wallSizeX = wallUpgrades[index].wallLength;

        var shape = buildParticles.shape;
        shape.scale = new(wallSizeX, buildParticles.shape.scale.y, buildParticles.shape.scale.z);
  

        yield return wallBuildingFeedback?.PlayFeedbacksCoroutine(transform.position);

     if(wallUpgrades[index].wallSprite != null)
        {
            sr.sprite = wallUpgrades[index].wallSprite;
        }

        sr.size = new(wallSizeX, sr.size.y);
        campCollider.size = new(wallSizeX, sr.size.y);
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

[System.Serializable]
public struct WallData
{
    public int wallLength;
    public Sprite wallSprite;
}
