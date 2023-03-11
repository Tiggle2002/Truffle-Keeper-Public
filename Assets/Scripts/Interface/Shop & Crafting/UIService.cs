using Sirenix.OdinInspector;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class UIService : SerializedMonoBehaviour
{
    public static UIService Instance;

    public void Awake()
    {
        Instance = this;
    }

    [FoldoutGroup("General Sprites")]
    public Sprite emptySprite;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite resourceSlot;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite selectedResourceSlot;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite lackingResourcesSlot;

    [FoldoutGroup("Structure Sprites")]
    public Sprite structureCard;

    [FoldoutGroup("Structure Sprites")]
    public Sprite selectedStructureCard;

    [FoldoutGroup("Currency Settings")]
    public ItemObject currencyObject;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite craftingItemCategory;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite craftingItemCategorySelected;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite craftableItemSlot;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite selectedCraftableItemSlot;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite newItemSprite;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite enoughResourcesSlot;

    [FoldoutGroup("Crafting Sprites")]
    public Sprite notEnoughResourcesSlot;
}
