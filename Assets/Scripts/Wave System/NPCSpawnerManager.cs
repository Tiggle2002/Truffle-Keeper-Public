using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class NPCSpawnManager : MonoBehaviour, TEventListener<GameEvent>
{
    #region  Serialized Fields
    [FoldoutGroup("Enemy Spawn Settings"), SerializeField]
    protected int baseNPCCount = 2;

    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    protected float waveMultipler = 0.01f;

    [FoldoutGroup("Enemy Spawn Settings"), SerializeField, Range(0.01f, 10f)]
    protected float standardDeviation;

    [SerializeField] private NPC[] npcs;
    [SerializeField] private Transform parentTransform;
    #endregion

    #region Protected & Private Fields
    protected BaseNPCSpawner[] npcSpawners;
    protected List<NPC> npcsJustSpawned;
    protected NPCPool npcPool;

    private Func<NPC, bool> npcSpawnCondition;
    private Action onBeforeSpawn;
    private Action onAfterSpawn;
    #endregion

    private void Awake()
    {
        InitialiseSpawners();
        InitialiseManager();

        IProbability[] spawnProbabilities = npcs.Cast<IProbability>().ToArray();
        spawnProbabilities.InitialiseProbabilityDistribution();
    }

    private void InitialiseSpawners()
    {
        npcPool = new(npcs, parentTransform);

        npcSpawners = GetComponentsInChildren<BaseNPCSpawner>();

        for (int i = 0; i < npcSpawners.Length; i++)
        {
            npcSpawners[i].InitialiseSpawner(npcPool.GetNPCObject);
        }
    }

    protected abstract void InitialiseManager();

    [Button("Spawn")]
    public void Spawn()
    {
        onBeforeSpawn?.Invoke();
        GenerateNPCs();
        onAfterSpawn?.Invoke();
    }

    private void GenerateNPCs()
    {
        npcsJustSpawned = new();
        for (int i = 0; i < npcSpawners.Length; i++)
        {
            if (npcSpawners[i].Active() == false) continue;

            float minEnemies = CalculateNPCFrequencyForSpawner(npcSpawners[i]);

            int enemyCount = Mathf.RoundToInt(Random.Range(minEnemies - standardDeviation, minEnemies + standardDeviation));

            SpawnNPCInQuantity(npcSpawners[i], enemyCount);
        }
    }

    private void SpawnNPCInQuantity(BaseNPCSpawner spawner, int quantity)
    {
        NPC[] npcsForSpawner = new NPC[quantity];

        for (int i = 0; i < npcsForSpawner.Length; i++)
        {
            npcsForSpawner[i] = SelectNPC();

            if (npcsForSpawner[i] != null)
            {
                npcsJustSpawned.Add(npcsForSpawner[i]);
            }
        }

        StartCoroutine(spawner.SpawnNPCs(npcsForSpawner));
    }

    private NPC SelectNPC()
    {
        float randomValue = Random.value;

        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcSpawnCondition?.Invoke(npcs[i]) == true && randomValue < npcs[i].chance)
            {
                return npcs[i];
            }
        }
        return npcs[0];
    }

    protected void AddSpawnCondition(params Func<NPC, bool>[] conditions)
    {
        foreach (var condition in conditions)
        {
            npcSpawnCondition += condition;
        }
    }

    protected void AddActionBeforeWave(params Action[] conditions)
    {
        foreach (var condition in conditions)
        {
            onBeforeSpawn += condition;
        }
    }

    protected void AddActionAfterWave(params Action[] conditions)
    {
        foreach (var condition in conditions)
        {
            onAfterSpawn += condition;
        }
    }

    protected abstract float CalculateNPCFrequencyForSpawner(BaseNPCSpawner spawner);

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                for (int i = 0; i < npcSpawners.Length; i++)
                {
                    npcSpawners[i].StopAllCoroutines();
                }
                break;
        }
    }

    public virtual void OnEnable()
    {
        this.Subscribe<GameEvent>();
    }

    public virtual void OnDisable()
    {
        this.Unsubscribe<GameEvent>();
    }
}

