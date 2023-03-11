using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability Data", menuName = "Scriptable Object/Data/Ability Data")]
public class AbilityData : ScriptableObject
{
    [FoldoutGroup("Ability Fields")]
    public float speedBoostX;
    [FoldoutGroup("Ability Fields"), Range(0, 1)]
    public float miningSpeedPercentageToAdd;
    [FoldoutGroup("Ability Fields")]
    public int healthToAdd;

    [FoldoutGroup("AbilityFields")]
    public bool permanent;
    [FoldoutGroup("AbilityFields"), HideIf("permanent")]
    public float abilityLength;
}
