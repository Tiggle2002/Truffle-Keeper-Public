using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemCraftingInterface))]
public class WaveRecipeUnlocker : MonoBehaviour, TEventListener<MonumentEvent>
{
    [TableList(NumberOfItemsPerPage = 1)]
    public ItemsToUnlock[] itemsToUnlocks;

    private ItemCraftingInterface craftingStation;

    public void Awake()
    {
        craftingStation = GetComponent<ItemCraftingInterface>();
    }

    public void OnEvent(MonumentEvent eventData)
    {
        if (eventData.eventType == MonumentEventType.UpgradeRecipeChanged)
        {
            UnlockRecipes(eventData.upgradeIndex);
        }
    }

    [Button("Unlock Recipes")]
    private void UnlockRecipes(int upgradeIndex)
    {
        ItemSlotData[] itemsToUnlock = GetItems(upgradeIndex);

        if (itemsToUnlock != null && itemsToUnlock.Length > 0)
        {
            craftingStation.AddRecipes(itemsToUnlock);
        }
    }

    private ItemSlotData[] GetItems(int upgradeIndex)
    {
        if (upgradeIndex < itemsToUnlocks.Length)
        {
            return itemsToUnlocks[upgradeIndex].items;
        }
        return null;
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
public struct ItemsToUnlock
{
    public ItemSlotData[] items;
}
