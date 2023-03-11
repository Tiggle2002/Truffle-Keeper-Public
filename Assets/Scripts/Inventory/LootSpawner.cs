using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootSpawner
{
    private static ObjectPool<PoolObject> lootPool;

    static LootSpawner()
    {
        GameObject lootPrefab = Resources.Load<GameObject>("Prefabs/Items/Loot");

        Transform decorationTransform = GameObject.Find("Decoration").GetComponent<Transform>();

        lootPool = new(lootPrefab, 30, decorationTransform);
    }

    public static void SpawnLoot(Vector3 spawnPos, ItemObject itemToDrop, int quantity, bool addForce = true)
    {
        GameObject lootObj = lootPool.PullGameObject();

        lootObj.GetComponent<LootableObject>().Set(itemToDrop, quantity);

        lootObj.transform.position = spawnPos;

        lootObj.name = $"Loot : {itemToDrop.ID}";

        if (addForce)
        {
            Rigidbody2D lootRb = lootObj.gameObject.GetComponent<Rigidbody2D>();
            lootRb.velocity = Vector2.zero;
            ApplyForceToLoot(lootRb);
        }
    }

    public static void SpawnMultipleLoot(this Loot[] lootData, Vector3 spawnPos,bool addForce = true)
    {
        List<Loot> lootToDrop = lootData.SelectLootToSpawn();

        foreach (var lootObject in lootToDrop)
        {
            int quantityToSpawn = lootObject.GetRandomQuantity();

            for (int i = 0; i < quantityToSpawn; i++)
            {
                PoolObject loot = lootPool.Pull();

                LootableObject lootObj = loot.GetComponent<LootableObject>();

                lootObj.Set(lootObject.item, 1);

                lootObj.transform.position = spawnPos;

                lootObj.name = $"Loot : {lootObject.item.ID}";

                if (addForce)
                {
                    Rigidbody2D lootRb = lootObj.gameObject.GetComponent<Rigidbody2D>();
                    lootRb.velocity = Vector2.zero;
                    ApplyForceToLoot(lootRb);
                }
            }
        }

    }

    public static List<Loot> SelectLootToSpawn(this Loot[] lootData)
    {
        float randomValue = Random.value;
        List<Loot> lootToDrop = new();
        foreach (var loot in lootData)
        {
            if (randomValue < loot.dropChance)
            {
                lootToDrop.Add(loot);
            }
        }
        return lootToDrop;
    }

    private static void ApplyForceToLoot(Rigidbody2D lootRb)
    {
        Vector2 force = new(Random.Range(-3.5f, 3.5f), Random.Range(2.5f, 5f));
        lootRb.AddForce(force, ForceMode2D.Impulse);
    }

    private static int GetRandomQuantity(this Loot lootData)
    {
        return Random.Range(lootData.quantity.x, lootData.quantity.y + 1);
    }
}