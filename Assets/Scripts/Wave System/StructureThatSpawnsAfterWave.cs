using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class StructureThatSpawnsAfterWave : MonoBehaviour, TEventListener<MonumentEvent>
{
    [SerializeField] private int waveToSpawnAfter;

    private BoxCollider2D bc;
    private SpriteRenderer sr;

    public void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        if (waveToSpawnAfter >= 0)
        {
            bc.enabled = false;
            sr.enabled = false;
        }
    }

    public bool ShouldSpawnThisWave(int waveNumber)
    {
        return waveToSpawnAfter <= waveNumber;
    }

    public void Spawn()
    {
        bc.enabled = true;
        sr.enabled = true;
        Destroy(this);
    }

    public void OnEvent(MonumentEvent eventData)
    {
        if (eventData.eventType == MonumentEventType.UpgradeRecipeChanged)
        {
            if (ShouldSpawnThisWave(eventData.upgradeIndex))
            {
                Spawn();
            }
        }
    }

    public void OnEnable()
    {
        this.Subscribe<MonumentEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<MonumentEvent>();
    }
}
