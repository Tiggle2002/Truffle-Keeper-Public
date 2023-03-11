using Sirenix.OdinInspector;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialStructureThatSpawnsAfterWave : MonoBehaviour, TEventListener<WaveEvent>
{
    [SerializeField] private int waveToSpawnAfter;
    [Title("Text"), HideLabel, MultiLineProperty(3), SerializeField] protected string textToDisplay;

    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private bool playerAtSite = false;

    public void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        bc.enabled = false;
        sr.enabled = false;
    }

    public bool ShouldSpawnThisWave(int waveNumber)
    {
        return waveToSpawnAfter <= waveNumber;
    }

    public void Spawn()
    {
        bc.enabled = true;
        sr.enabled = true;
        DisplayStructureTutorial();
    }

    private void DisplayStructureTutorial()
    {
        Vector3 textPos = new(transform.position.x, transform.position.y + 10);

        StartCoroutine(textPos.PresentTextMeshCoroutine(textToDisplay, PlayerAtSite));
    }

    private bool PlayerAtSite()
    {
        return playerAtSite;
    }

    public void OnEvent(WaveEvent eventData)
    {
        if (eventData.eventType == WaveEventType.WaveDefeated)
        {
            if (ShouldSpawnThisWave(eventData.waveCount))
            {
                Spawn();
            }
        }
    }

    public void OnEnable()
    {
        this.Subscribe<WaveEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<WaveEvent>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerAtSite = true;
            }
        }
    }
}
