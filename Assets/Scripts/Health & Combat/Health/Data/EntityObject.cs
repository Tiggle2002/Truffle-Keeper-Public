using Sirenix.OdinInspector;
using SurvivalElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Entity", menuName = "Scriptable Object/Health-Based Entity")]
public class EntityObject : SerializedScriptableObject
{
    [EnumToggleButtons]
    public EntityType entityType;
    [HideIf("entityType", EntityType.Player)]
    public string ID;

    #region  Health Properties
    [FoldoutGroup("Health Properties")]
    [SerializeField] private int maxHealth;
    [FoldoutGroup("Health Properties")]
    [SerializeField] private int initialHealth;
    #endregion

    #region Loot Properties
    [FoldoutGroup("Enemy Properties"), HideIf("entityType", EntityType.Player)]
    public int XP;

    [FoldoutGroup("Loot"), HideIf("entityType", EntityType.Player)]
    public bool lootable;

    [FoldoutGroup("Loot"), HideIf("entityType", EntityType.Player), ShowIf("lootable"), TableList]
    public Loot[] lootToDrop;

    [FoldoutGroup("Damage Resistance"), HideIf("entityType", EntityType.Player)]
    public bool toolRequired;
    [FoldoutGroup("Damage Resistance"), HideIf("entityType", EntityType.Player), ShowIf("toolRequired")]
    public ToolType requiredTool;

    [FoldoutGroup("Damage Resistance"), HideIf("entityType", EntityType.Player), ShowIf("toolRequired")]
    public ToolTier minmumToolTier;
    #endregion

    #region Data Finders
    public int MaxHealth()
    {
        return maxHealth;
    }

    public int InitialHealth()
    {
        return initialHealth;
    }

    public void IncreaseHealth(int newMaxHealth, bool matchInitialHealth = true)
    {
        if (newMaxHealth <= 0)
        {
            return;
        }

        maxHealth += newMaxHealth;

        if (matchInitialHealth)
        {
            initialHealth = maxHealth;
        }
    }

    public void SetMaxHealth(int newMaxHealth, bool matchInitialHealth = true)
    {
        if (newMaxHealth <= 0)
        {
            return;
        }

        maxHealth = newMaxHealth;

        if (matchInitialHealth)
        {
            initialHealth = maxHealth;
        }
    }
    #endregion
}

public struct Loot
{
    public ItemObject item;

    [MinMaxSlider(1, 100, true)]
    public Vector2Int quantity;

    [Range(0, 1)]
    public float dropChance;
}
public enum EntityType { Player, Character, Structure, Enemy }
