using Sirenix.OdinInspector;
using SurvivalElements;
using System.Collections;
using UnityEngine;


public class ProximityEnemySpawner : BaseNPCSpawner, TEventListener<CampEvent>
{
    [SerializeField, FoldoutGroup("Spawn Settings"), MinMaxSlider(10, 100)] private Vector2 possibleDistances;

    private bool canSpawn = true;
    public override bool Active()
    {
        return canSpawn;
    }

    protected override void SetPosition(GameObject npcObject)
    {
        Vector2 spawnPos = new Vector2(0, 5f);
        float distanceX = Random.Range(possibleDistances.x, possibleDistances.y);
        
        if (Random.value < 0.5)
        {
            spawnPos.x = PlayerManager.Instance.transform.position.x - distanceX;
        }
        else
        {
            spawnPos.x = PlayerManager.Instance.transform.position.x + distanceX;
        }

        npcObject.transform.position = spawnPos;
    }

    public void OnEvent(CampEvent eventData)
    {
        if (eventData.entityType != EntityType.Player)

        switch (eventData.eventType)
        {
            case CampEventType.ObjectEntered:
                    canSpawn = false;
                    break;
            case CampEventType.ObjectExited:
                canSpawn = false;
                break;
        }
    }
}