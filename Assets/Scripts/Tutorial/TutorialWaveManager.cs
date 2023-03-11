using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialWaveManager : MonoBehaviour, TEventListener<GameEvent>, TEventListener<WaveEvent>
{
    #region Serialised Variables
    public int WaveCount { get { return waveCount; } }
    [SerializeField] private static int waveCount = 0;
    [Min(1)]
    [SerializeField] private int startWaveIndex;
    [SerializeField] private int timeBetweenWaves;
    [SerializeField] private MMF_Player waveStartedFeedback;

    [FoldoutGroup("Enemy Spawn Settings"), SerializeField]
    private NPC[] enemies;
    [FoldoutGroup("Enemy Spawn Settings"), SerializeField]
    private int baseEnemyCount = 2;
    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    [DetailedInfoBox("Info", "A Higher Value = More Spawns over Greater Distance")]
    private float distanceMultiplier = 0.01f;
    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    [DetailedInfoBox("Info", "A Higher Value = More Spawns over as Wave Number Increases")]
    private float waveMultipler = 0.01f;

    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    [DetailedInfoBox("Info", "A Higher Value = More Varied Spawn Rates")]
    private float standardDeviation;
    private float mean;
    #endregion

    #region Variables
    private EnemySpawner[] enemySpawners;
    private NPCPool enemyPools;
    private WaveAccountant waveAccountant;
    private float spawnRange;
    private int totalWavePoints;
    #endregion

    #region Unity Update Methods
    public void Start()
    {
        enemyPools = new(enemies, transform);
        InitialiseSpawners();
        InitialiseProbabilityDistribution();
        waveCount = startWaveIndex;
        spawnRange = 50;
        waveAccountant = new WaveAccountant();
    }
    #endregion

    #region Initialisation
    private void InitialiseSpawners()
    {
        enemySpawners = GetComponentsInChildren<EnemySpawner>();

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawners[i].InitialiseSpawner(enemyPools.GetNPCObject);
        }
    }

    private void InitialiseProbabilityDistribution()
    {
        NormaliseSpawnRates();
        CreateCumulativeSpawnRates();
    }

    private void NormaliseSpawnRates()
    {
        float total = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            total += enemies[i].chance;
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].chance /= total;
        }
    }

    private void CreateCumulativeSpawnRates()
    {
        for (int i = 1; i < enemies.Length; i++)
        {
            enemies[i].chance += enemies[i - 1].chance;
        }
    }
    #endregion

    [Button("Spawn Wave")]
    private void SpawnWave()
    {
        totalWavePoints = 0;
        GenerateEnemies();
        IncreaseSpawningDistance();
        waveCount++;
    }

    private void IncreaseSpawningDistance()
    {
        if (Random.value < 0.5)
        {
            spawnRange += 20;
        }
    }

    private void GenerateEnemies()
    {
        for (int i = 0; i < enemySpawners.Length; i++)
        {
            if (enemySpawners[i].Active())
            {
                continue;
            }


            float meanEnemies = CalculateMeanEnemiesForSpawnerWithRespectToDistance(enemySpawners[i].DistanceFromMonument);

            int enemyCount = Mathf.RoundToInt(Random.Range(meanEnemies - standardDeviation, meanEnemies + standardDeviation));

            SpawnEnemyQuantity(enemySpawners[i], enemyCount);
        }
        waveAccountant.SetMaxPoints(totalWavePoints);
        WaveEvent.Trigger(WaveEventType.WaveBegun, waveCount: waveCount);
    }

    private float CalculateMeanEnemiesForSpawnerWithRespectToDistance(float distance)
    {
        return baseEnemyCount + (distance * distanceMultiplier)
                                                           + (waveCount * waveMultipler);
    }

    public void SpawnEnemyQuantity(EnemySpawner spawner, int quantity)
    {
        NPC[] enemiesForSpawner = new NPC[quantity];

        for (int y = 0; y < enemiesForSpawner.Length; y++)
        {
            enemiesForSpawner[y] = SelectEnemy();
        }

        StartCoroutine(spawner.SpawnNPCs(enemiesForSpawner));
    }

    public NPC SelectEnemy()
    {
        float randomValue = Random.value;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (waveCount < enemies[i].minimumWaveReached)
            {
                continue;
            }
            if (randomValue < enemies[i].chance)
            {
                totalWavePoints += enemies[i].obj.GetComponentInChildren<EnemyHealth>(true).GetPoints();
                return enemies[i];
            }
        }

        totalWavePoints += enemies[0].obj.GetComponentInChildren<EnemyHealth>(true).GetPoints();
        return enemies[0];
    }


    public void OnEnable()
    {
        this.Subscribe<GameEvent>();
        this.Subscribe<WaveEvent>();
    }

    public void OnDisable()
    {
        waveAccountant?.Dispose();
        this.Unsubscribe<GameEvent>();
        this.Unsubscribe<WaveEvent>();
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                StopAllCoroutines();
                for (int i = 0; i < enemySpawners.Length; i++)
                {
                    enemySpawners[i].StopAllCoroutines();
                }
                break;
        }
    }

    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.TriggerWaveStart:
                SpawnWave();
                break;
        }
    }
}