using System.Collections;
using UnityEngine;

public class AnimalSpawner : BaseNPCSpawner
{
    [SerializeField] private Transform[] flyingAnimalSpawnPoints;
    [SerializeField] private Transform[] landAnimalSpawnPoints;

    public override bool Active()
    {
        return true;
    }

    protected override void SetPosition(GameObject npcObject)
    {
        if (npcObject.GetComponent<AnimalFSM>().AnimalType == AnimalType.Flying)
        {
            npcObject.transform.position = flyingAnimalSpawnPoints[Random.Range(0, flyingAnimalSpawnPoints.Length)].position;
        }
        else
        {
            npcObject.transform.position = landAnimalSpawnPoints[Random.Range(0, flyingAnimalSpawnPoints.Length)].position;
        }
    }
}