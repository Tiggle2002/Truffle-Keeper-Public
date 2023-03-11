using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseNPCSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private MMF_Player spawnFeedback;
    private Func<NPC, GameObject> GetNPCFunc;

    public void InitialiseSpawner(Func<NPC, GameObject> getEnemyFunc)
    {
        this.GetNPCFunc = getEnemyFunc;
    }

    public abstract bool Active();

    public IEnumerator SpawnNPCs(NPC[] npc)
    {
        for (int i = 0; i < npc.Length; i++)
        {
            SpawnNPC(npc[i]);

            float delayBetweenSpawns = npc[i].GetDelay();
            if (spawnFeedback != null)
            {
                spawnFeedback.transform.position = transform.position;
                spawnFeedback?.PlayFeedbacks();
            }
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }

    private void SpawnNPC(NPC npc)
    {
        GameObject spawnedNPC = GetNPCFunc.Invoke(npc);
        RespawnNPC(spawnedNPC);
        SetPosition(spawnedNPC);
    }

    private void RespawnNPC(GameObject npcObject)
    {
        npcObject.gameObject.GetComponent<Health>().Respawn();
    }

    protected virtual void SetPosition(GameObject npcObject)
    {
        npcObject.transform.position = spawnPoint.position;
    }
}

public class EnemySpawner : BaseNPCSpawner, TEventListener<WaveEvent>
{
    public float DistanceFromMonument => Mathf.Abs(0 - transform.position.x);

    #region Variables
    [SerializeField] private int activeAfterWave;
    private bool active;
    #endregion

    #region Methods
    public override bool Active() => active && transform.WithinLevel();



    public void OnEvent(WaveEvent eventData)
    {
        if (eventData.eventType != WaveEventType.WaveBegun) return;
        
        active = eventData.waveCount > activeAfterWave;
    }
    #endregion

    public void OnEnable()
    {
        this.Subscribe();
    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }
}



