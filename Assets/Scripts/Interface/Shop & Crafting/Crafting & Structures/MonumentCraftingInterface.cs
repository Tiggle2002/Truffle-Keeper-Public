using System.Collections;
using UnityEngine;

public class MonumentCraftingInterface : CraftingInterface, TEventListener<MonumentEvent>
{
    public void OnEvent(MonumentEvent eventData)
    {
        switch (eventData.eventType)
        {
            case MonumentEventType.UpgradeRecipeChanged:
                craftingRecipe = eventData.recipe;
                UpdateInterface();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<MonumentEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<MonumentEvent>();
    }
}