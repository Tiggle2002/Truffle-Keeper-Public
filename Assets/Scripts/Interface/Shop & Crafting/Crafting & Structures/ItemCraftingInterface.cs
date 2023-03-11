using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemCategory { None, Weapon, Material }

/// <summary>
/// Displays Item Crafting Recipes in Categories
/// </summary>
/// 
public class ItemCraftingInterface : CraftingInterface, TEventListener<CraftingEvent>, ICategorised
{
    #region Fields
    [FoldoutGroup("Interface References"), SerializeField]
    private Image selectedItemImage;
    [FoldoutGroup("Interface References"), SerializeField]
    private TextMeshProUGUI selectedItemName;
    private List<ItemSlotData> currentCategory;

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout), SerializeField]
    private Dictionary<ItemType, List<ItemSlotData>> productMap = new()
    {
        { ItemType.Weapon, new List<ItemSlotData>() },
        { ItemType.Ammunition, new List<ItemSlotData>() },
        { ItemType.Material, new List<ItemSlotData>() }
    };

    [FoldoutGroup("References"), SerializeField]
    private CraftableItemSlot[] craftableItemSlots;
    #endregion

    protected override void Start()
    {
        base.Start();
        
    }

    public override void OpenInterface()
    {
        base.OpenInterface();

        GetComponentInChildren<CategoryButton>().Select(); //Lazy Solution to Select the inital page - It returns the first 
        GetComponentInChildren<CategoryButton>().SelectPage();
    }

    #region Changing Recipes
    public void SetCategory(int category)
    {
        ItemSlotData[] itemsOfCategory = GetProductsOfCategory(category);

        if (itemsOfCategory != null)
        {
            SetCraftableItems(itemsOfCategory);
        }
    }

    public ItemSlotData[] GetProductsOfCategory(int category)
    {
        if (productMap.TryGetValue((ItemType)category, out var products))
        {
            return products.ToArray();
        }
        return null;
    }

    private void SetCraftableItems(ItemSlotData[] items)
    {
        currentCategory = items.ToList();
        for (int i = 0; i < craftableItemSlots.Length; i++)
        {
            if (i >= items.Length)
            {
                craftableItemSlots[i].Deactivate();
            }
            else
            {
                SetSlot(i);
            }
        }

        if (currentCategory != null && currentCategory.Count > 0)
        {
            SetRecipe(currentCategory[0].item, currentCategory[0].item.craftingRecipe);
            UpdateInterface();
        }
    }

    public void AddRecipes(ItemSlotData[] itemsToAdd)
    {
        for (int i = 0; i < itemsToAdd.Length; i++)
        {
            productMap[itemsToAdd[i].item.type].Add(itemsToAdd[i]);
        }
    }
    #endregion

    #region Methods
    private void SetSlot(int i)
    {
        ItemObject itemObject = currentCategory[i].item;
        int quantity = currentCategory[i].quantity;

        if (itemObject == null || itemObject.ID == null)
        {
            craftableItemSlots[i].Deactivate();
        }
        else
        {
            craftableItemSlots[i].Activate(itemObject, quantity);
        }
    }

    //private void SetItemImage(Sprite sprite)
    //{
    //    selectedItemImage.sprite = sprite;
    //}

    //private void SetItemName(string name)
    //{
    //    selectedItemName.text = name;
    //}
    #endregion

    #region TEvent Methods
    public void OnEvent(CraftingEvent eventData)
    {
        if (eventData.eventCraftingInterface != this || eventData.eventType != CraftingEventType.SetRecipe || craftingRecipe == eventData.recipe)
        {
            return;
        }

        SetRecipe(eventData.itemData.item, eventData.recipe);
        UpdateInterface();
    }

    private void SetRecipe(ItemObject item, CraftingMaterial[] recipe)
    {
        craftingRecipe = recipe;
        //SetItemImage(item.sprite);
        //SetItemName(item.ID);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Subscribe<CraftingEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Unsubscribe<CraftingEvent>();
    }
    #endregion
}
