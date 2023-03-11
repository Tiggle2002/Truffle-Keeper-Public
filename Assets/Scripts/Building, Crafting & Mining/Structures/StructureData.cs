using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure Data", menuName = "Scriptable Object/Data/Structure Data")]
public class StructureData : ScriptableObject
{
    [HorizontalGroup("Item", 75)]
    [PreviewField(75), Title("Icon"), HideLabel]
    public Sprite sprite;
    [VerticalGroup("Item/info"), Title("Item Description"), HideLabel, MultiLineProperty]
    public string description;

    [FoldoutGroup("Purchasing"), LabelWidth(100), HorizontalGroup("Purchasing/Purchase")]
    public bool purchaseable;
    [FoldoutGroup("Purchasing"), HorizontalGroup("Purchasing/Purchase"), ShowIf("purchaseable"),]
    public int price;
    [FoldoutGroup("Purchasing"), HorizontalGroup("Purchasing/Purchase"), ShowIf("purchaseable"), Range(0, 100)]
    public int unlockedLevel;

    [FoldoutGroup("Purchasing")]
    public CraftingMaterial[] craftingRecipe;

    [FoldoutGroup("Upgrading"), LabelWidth(100), HorizontalGroup("Upgrading/Upgrade")]
    public bool upgradeable;

    [FoldoutGroup("Upgrading"), LabelWidth(202.5f), PropertySpace(2.5f), ShowIf("upgradeable")]
    public StructureData upgradedStructureData;

    [FoldoutGroup("References")]
    public GameObject prefab;
    [FoldoutGroup("References")]
    public Vector3 prefabSpawnOffset;
    [FoldoutGroup("References")]
    public EntityObject healthData;

    [VerticalGroup("Item/info", 10), EnumToggleButtons]
    public FortificationType type;

    [FoldoutGroup("Fortification"), TableList(DrawScrollView = true, MaxScrollViewHeight = 200, MinScrollViewHeight = 100)]
    public List<StructureIntegrityLevel> structureLevels = new List<StructureIntegrityLevel>();
}
