using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPool
{
    NPC[] enemyTypes;
    Transform parentTransform;
    private Dictionary<string, ObjectPool<PoolObject>> enemyPools;

    public NPCPool(NPC[] enemyTypes, Transform parentTransform)
    {
        this.enemyTypes = enemyTypes;
        this.parentTransform = parentTransform;

        enemyPools = new();
        CreatePools();
    }

    private void CreatePools()
    {
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            if (!enemyPools.ContainsKey(enemyTypes[i].ID))
            {
                ObjectPool<PoolObject> enemyPool = new(enemyTypes[i].obj, enemyTypes[i].poolSize, parentTransform);
                enemyPools.Add(enemyTypes[i].ID, enemyPool);
            }
        }
    }

    public int PoolCount(NPC npc)
    {
        return enemyPools[npc.ID].pooledCount;
    }

    public GameObject GetNPCObject(NPC npc)
    {
        if(!enemyPools.ContainsKey(npc.ID))
        {
            return null;
        }
        return enemyPools[npc.ID].PullGameObject();
    }
}
