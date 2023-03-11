using UnityEngine;
using TMPro;
using System;



public enum StructureButtonState { None, Upgrade, Repair }

public class StructureButton : CraftButton
{
    #region Fields
    private StructureButtonState buttonState;
    private TextMeshProUGUI priceText;
    private StructureButtonSelector buttonSelector;
    #endregion

    public void Awake()
    {
        InitialiseComponents();
    }

    public void InitialiseComponents()
    {
        priceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();
        buttonSelector = new();
    }

    public void SetButtonState(Structure structure)
    {
        buttonState = buttonSelector.SelectButtonState(structure, ref price);
        priceText.text = buttonSelector.ButtonText();
        if (buttonState != StructureButtonState.None)
        {
           priceText.text += price;
        } 
        
        if (buttonState == StructureButtonState.Upgrade)
        { 
            StructureEvent.Trigger(StructureEventType.StructureUpgradeAvailable, structure.Data);
        }
        else if (buttonState == StructureButtonState.Repair)
        {
            CraftingMaterial[] repairRecipe = new CraftingMaterial[structure.Data.craftingRecipe.Length];

            for (int i = 0; i < repairRecipe.Length; i++)
            {
                repairRecipe[i].item = structure.Data.craftingRecipe[i].item;
                repairRecipe[i].quantity = Mathf.RoundToInt(structure.Data.craftingRecipe[i].quantity / 2f);
            }

            StructureEvent.Trigger(StructureEventType.RepairSelected, structure.Data,repairRecipe);
        }
    }

    public override bool CanPurchase()
    {
        return base.CanPurchase() && buttonState != StructureButtonState.None;
    }

    protected override void PerformTransaction()
    {
        Craft();
        Purchase();
        PerformStructureAction();
    }

    private void PerformStructureAction()
    {
        if (buttonState == StructureButtonState.Upgrade)
        {
            ActiveStructure.Site.scaffolding.Set(ScaffoldingType.Upgrade);
            StructureEvent.Trigger(StructureEventType.Upgrade);
        }
        else
        {
            ActiveStructure.Site.scaffolding.Set(ScaffoldingType.Repair);
            StructureEvent.Trigger(StructureEventType.Repair);
        }
    }
}

public class StructureButtonSelector
{
    private string buttonText;

    public StructureButtonState SelectButtonState(Structure structure, ref int price)
    {
        if (structure.CurrentState == StructureState.Constructing)
        {
            buttonText = "Constructing...";
            price = 0;

            return StructureButtonState.None;
        }
        else if (structure.Upgradeable())
        {
            buttonText = String.Concat("Upgrade : ");
            price = structure.Data.upgradedStructureData.price;

            return StructureButtonState.Upgrade;
        }
        else if (structure.Repairable())
        {
            buttonText = String.Concat("Repair : ");
            price = structure.GetRepairPrice();

            return StructureButtonState.Repair;
        }


        buttonText = "Max Level";
        return StructureButtonState.None;
    }

    public string ButtonText()
    {
        return buttonText;
    }
}


