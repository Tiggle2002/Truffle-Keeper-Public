using Sirenix.OdinInspector;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityData AbilityData { get { return AbilityData; } }

    [FoldoutGroup("Ability References"), SerializeField] private AbilityData abilityData;
}
