using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCraftingInterface : CraftingInterface, TEventListener<StructureEvent>
{
    public void OnEvent(StructureEvent eventData)
    {
        switch (eventData.eventType)
        {
            case StructureEventType.StructureSelected:
                craftingRecipe = eventData.data.craftingRecipe;
                UpdateInterface();
                break;
            case StructureEventType.StructureUpgradeAvailable:
                craftingRecipe = eventData.data.upgradedStructureData.craftingRecipe;
                UpdateInterface();
                break;
            case StructureEventType.RepairSelected:
                craftingRecipe = eventData.recipe;
                UpdateInterface();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<StructureEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<StructureEvent>();
    }
}
