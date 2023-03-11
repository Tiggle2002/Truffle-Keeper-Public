using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurvivalElements;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Standard Item", menuName = "Scriptable Object/Survival Element/Item/Ammunition")]
public class AmmoObject : ItemObject
{
    [FoldoutGroup("Projectile Settings")]
    public bool dropoff;
    [FoldoutGroup("Projectile Settings")]
    public bool explosive;
    [FoldoutGroup("Projectile Settings"), ShowIf("explosive")]
    public bool explodeOnImpact;
    [FoldoutGroup("Projectile Settings"), ShowIf("explosive"), Range(0, 3f)]
    public float explosionDelay;

    public void Awake()
    {
        type = ItemType.Ammunition;
    }
}
