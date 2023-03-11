using MoreMountains.Feedbacks;
using SurvivalElements;
using System;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Collections.Generic;
using static PixelCrushers.LODManager;
using Unity.VisualScripting;

public enum StructureCategory { Fortification, Agriculture, Civil, Military, None }

public class StructureShopInterface : BaseInterface, TEventListener<StructureEvent>, ICategorised, TEventListener<PlayerEvent>, TEventListener<InventoryEvent>, TEventListener<MonumentEvent>
{
    #region Variables
    #region References
    [FoldoutGroup("Canvas References"), SerializeField]
    private Canvas userInterface;

    [FoldoutGroup("Canvas References"), SerializeField]
    private Canvas productsPage;

    [FoldoutGroup("Canvas References"), SerializeField]
    private Canvas structureSettingsPage;

    [FoldoutGroup("UI References"), SerializeField]
    private StructureButton structureButton;

    [FoldoutGroup("UI References"), SerializeField]
    private DismantleStructureButton dismantleStructureButton;

    private StructureSlot[] slots;

    private int currentBaseLevel;
    #endregion

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout), SerializeField]
    private Dictionary<StructureCategory, List<StructureProduct>> productMap = new Dictionary<StructureCategory, List<StructureProduct>>()
    {
        { StructureCategory.Fortification, new List<StructureProduct>() },
        { StructureCategory.Agriculture, new List<StructureProduct>() },
        { StructureCategory.Civil, new List<StructureProduct>() },
        { StructureCategory.Military, new List<StructureProduct>() }
    };
    #endregion

    #region Unity Update Methods
    protected override void Awake()
    {
        base.Awake();
        slots = GetComponentsInChildren<StructureSlot>();
    }
    #endregion

    #region Interface Methods
    public override void CloseInterface()
    {
        base.CloseInterface();
        SetUserInterfaceEnabled(false);
    }

    public override void OpenInterface()
    {
        if (ActiveStructure.Site == null)
        {
            return;
        }
        base.OpenInterface();
        SetUserInterfaceEnabled(true);
        UpdateInterface();
    }

    protected override void UpdateInterface()
    {
        DisplayCorrectInterface();
    }
    #endregion

    private void SetUserInterfaceEnabled(bool enable)
    {
        userInterface.enabled = enable;
        userInterface.gameObject.SetActive(enable);
    }

    public virtual void DisplayCorrectInterface()
    {
        if (ActiveStructure.Site.StructurePresent)
        {
            DisplayStructureSettings();
        }
        else
        {
            DisplayStructureProducts();
        }
    }

    private void DisplayStructureSettings()
    {
        Structure structure = ActiveStructure.Site.transform.GetChild(1).GetComponentInChildren<Structure>();

        DisplayStructureSettingsPage();
        structureButton?.SetButtonState(structure);
    }

    private void DisplayStructureSettingsPage()
    {
        productsPage.enabled = false;
        structureSettingsPage.enabled = true;
    }

    private void DisplayStructureProducts()
    {
        productsPage.enabled = true;
        structureSettingsPage.enabled = false;
        SetCategory((int)StructureCategory.Fortification);
    }

    public void SetCategory(int category)
    {
        StructureProduct[] structureProducts = GetProductsOfCategory(category);

        ChangeDisplayedCategory(structureProducts);
    }

    public StructureProduct[] GetProductsOfCategory(int category)
    {
        if (productMap.TryGetValue((StructureCategory)category, out var products))
        {
            return products.ToArray();
        }
        return null;
    }

    public void ChangeDisplayedCategory(StructureProduct[] products)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < products.Length)
            {
                slots[i].SetStructureData(products[i].structureData);
                slots[i].SetUnlocked(products[i].structureData.unlockedLevel <= currentBaseLevel);
            }
        }
    }

    #region TEvent Methods
    public void OnEvent(StructureEvent eventData)
    {
        if (ActiveStructure.Site == null)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case StructureEventType.Repair:
            case StructureEventType.Upgrade:
            case StructureEventType.StateChange:
            case StructureEventType.StructureBuilt:
                DisplayCorrectInterface();
                break;
            case StructureEventType.NoActiveSite:
                CloseInterface();
                break;
            case StructureEventType.Demolish:
                ActiveStructure.Site?.DestroyStructure();
                DisplayCorrectInterface();
                break;
        }
    }

    public void OnEvent(PlayerEvent eventData)
    {
        switch (eventData.eventType)
        {
            case PlayerEventType.PlayerInteracted:
                if (ActiveStructure.Site != null)
                {
                    HUDEvent.Trigger(HUDEventType.InterfaceToggled, eventInterface: this);
                    transform.position = new(ActiveStructure.Site.transform.position.x, ActiveStructure.Site.transform.position.y + 10f);
                }
                break;
        }
    }

    public void OnEvent(InventoryEvent eventData)
    {
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<StructureEvent>();
        this.Subscribe<PlayerEvent>();
        this.Subscribe<MonumentEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<StructureEvent>();
        this.Unsubscribe<PlayerEvent>();
        this.Unsubscribe<MonumentEvent>();
    }

    public void OnEvent(MonumentEvent eventData)
    {
        if (eventData.eventType != MonumentEventType.UpgradeRecipeChanged) return;

        currentBaseLevel = eventData.upgradeIndex;
    }


    #endregion
}

[System.Serializable]
public struct StructureProduct
{
    public StructureData structureData;

    public StructureProduct(StructureData structure)
    {
        this.structureData = structure;
    }

    public bool Empty()
    {
        return structureData == null;
    }
}