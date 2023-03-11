using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DamageAbility : DamageOnCollision
{
    [FoldoutGroup("Feedback")]
    [SerializeField] protected MMF_Player abilityFeedback;

    public abstract bool TargetsAvailable();

    public abstract void PerformAbility();

    public abstract IEnumerator PerformAbilityCoroutine(float delay = 0f);

    public abstract void ResetAbility();
}
