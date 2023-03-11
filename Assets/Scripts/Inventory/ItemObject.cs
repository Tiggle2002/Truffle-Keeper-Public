using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum ItemType { None, Weapon, Currency, Ammunition, Structure, Material, Consumable }

public enum ContainerType { PlayerInventory, Chest, Shop }

public enum ToolType { None, Axe, Pickaxe, Hammer, Scythe }

public enum ToolTier { None, Stone, Copper, Iron, Wildstone }

[CreateAssetMenu(fileName = "Standard Item", menuName = "Scriptable Object/Survival Element/Item/Standard Item")]
[InlineEditor]
public class ItemObject : SerializedScriptableObject
{
    #region Item Properties
    [HorizontalGroup("Item", 75)]
    [PreviewField(75), Title("Icon"), HideLabel]
    public Sprite sprite;
    [VerticalGroup("Item/info"), Title("Item ID"), HideLabel]
    public string ID;
    [VerticalGroup("Item/info"), Title("Item Description"), HideLabel, MultiLineProperty]
    public string description;
    [VerticalGroup("Item/info", 10)]
    public ItemType type;
    public ItemCategory category;

    #region Inventory Properties
    public bool inventoryBased = true;
    [ShowIfGroup("inventoryBased")]
    [FoldoutGroup("inventoryBased/Inventory Properties"), Range(1, 1000)]
    public int maxStackQuantity = 1;

    [FoldoutGroup("inventoryBased/Inventory Properties")]
    public bool holdable = false;

    [FoldoutGroup("inventoryBased/Inventory Properties")]
    [ShowIf("holdable")]
    public Vector3 equippedOffset;

    [FoldoutGroup("inventoryBased/Inventory Properties")]
    public bool purchaseable;

    [FoldoutGroup("purchaseable/Purchase Properties")]
    public int price;

    [ShowIfGroup("purchaseable")]
    [FoldoutGroup("purchaseable/Purchase Properties")]
    public bool oneTimePurchase = false;

    [FoldoutGroup("purchaseable/Purchase Properties")]
    public ItemObject requiredItem = null;

    public CraftingMaterial[] craftingRecipe;
    #endregion

    #region Tool Properties]
    [FoldoutGroup("type/Weapon Properties"), ShowIf("type", ItemType.Weapon)]
    public ToolType toolType;

    [FoldoutGroup("type/Weapon Properties"), ShowIf("type", ItemType.Weapon)]
    public ToolTier toolTier;

    [FoldoutGroup("type/Weapon Properties"), ShowIf("type", ItemType.Weapon)]
    public bool comboWeapon;

    [FoldoutGroup("type/Weapon Properties"), ShowIf("type", ItemType.Weapon)]
    public bool holdToUse;

    [FoldoutGroup("type/Weapon Properties"), HideIfGroup("type", ItemType.Currency), HideIf("comboWeapon")]
    public int damage;

    [FoldoutGroup("type/Weapon Properties"), HideIfGroup("type", ItemType.Currency), ShowIf("comboWeapon"), TableList]
    public ComboData[] combos;

    [FoldoutGroup("type/Weapon Properties"), Range(0.1f, 2f), HideIf("type", ItemType.Currency), HideIf("comboWeapon"), HideIf("type", ItemType.Ammunition)]
    public float animatorSpeed;

    [FoldoutGroup("type/Weapon Properties"), HideIf("type", ItemType.Currency), Range(0.05f, 5f), HideIf("comboWeapon"), HideIf("type", ItemType.Ammunition)]
    public float useLength;

    [FoldoutGroup("type/Weapon Properties"), HideIf("type", ItemType.Currency), HideIf("comboWeapon"), HideIf("type", ItemType.Ammunition)]
    public float attackDelay;

    [FoldoutGroup("type/Weapon Properties"), HideIf("type", ItemType.Currency), HideIf("type", ItemType.Ammunition), Range(0.05f, 5f)]
    public float cooldown;

    [FoldoutGroup("type/Weapon Properties"), HideIf("type", ItemType.Currency), HideIf("type", ItemType.Ammunition)]
    public bool ammoBased;

    [FoldoutGroup("type/Weapon Properties"), HideIf("type", ItemType.Currency), HideIf("type", ItemType.Ammunition), ShowIf("ammoBased")]
    public AmmoObject ammoObject;

    [FoldoutGroup("type/Weapon Properties"), ShowIf("ammoBased")]
    public float range;
    #endregion
    #endregion

    #region References
    [Title("References", titleAlignment: TitleAlignments.Centered)]
    public GameObject prefab;
    #endregion

    #region Item Methods

    public ItemObject Copy()
    {
        ItemObject clone = Instantiate(this) as ItemObject;
        return clone;
    }

    public float UseLength()
    {
        return useLength * (AnimatorSpeed() - 1);
    }

    public float ComboUseLength(int comboIndex)
    {
        if (comboIndex > combos.Length -1)
        {
            return 0f;
        }
        return combos[comboIndex].animationLength - (combos[comboIndex].animationLength * (AnimatorSpeed() - 1f));
    }

    public float AnimatorSpeed()
    {
        return animatorSpeed * PlayerManager.Instance.GetComponent<AbilityReciever>().miningSpeed;
    }
    #endregion
}


