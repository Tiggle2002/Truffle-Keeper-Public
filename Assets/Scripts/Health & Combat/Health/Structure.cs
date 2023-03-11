using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Diagnostics;
using System;

public abstract class Structure: SerializedMonoBehaviour
{
    public StructureData Data { get { return structureData; } }

    #region Serialised Properties
    [ShowInInspector, ReadOnly] public StructureState CurrentState { get { return state; } }

    [SerializeField, FoldoutGroup("Disable Settings")] protected bool rotateAroundCenter;
    [SerializeField, FoldoutGroup("Disable Settings")] protected bool disableOnStart;
    [SerializeField, FoldoutGroup("Disable Settings")] protected bool disableCollider;
    [SerializeField, FoldoutGroup("Disable Settings")] protected bool disableSpriteRenderer;
    [SerializeField] protected StructureData structureData;


    [FoldoutGroup("Feedback"), SerializeField] private MMF_Player buildFeedback;
    [FoldoutGroup("Feedback"), SerializeField] private MMF_Player deconstructFeedback;
    #endregion

    protected StructureState state;
    protected SpriteRenderer sr;
    protected BoxCollider2D bc;
    protected Action onUpgraded; 

    protected virtual void InitialiseComponents()
    {
        sr = GetComponentInChildren<SpriteRenderer>(true);
        bc = GetComponent<BoxCollider2D>();
        if (disableOnStart)
        {
            sr.enabled = false;
            bc.enabled = false;
        }
    }

    protected abstract void Initialise();

    public virtual void Upgrade()
    {
        if (structureData.upgradeable)
        {
            structureData = structureData.upgradedStructureData;
            onUpgraded?.Invoke();
            Build();
        }
    }

    public abstract bool Upgradeable();

    public abstract bool Repairable();

    public void Build()
    {
        buildFeedback?.PlayFeedbacks();
        SetState(StructureState.Intact);
        SetEnabled(true);
    }

    protected virtual void SetEnabled(bool enabled)
    {
        if (disableCollider) bc.enabled = enabled;
        if (disableSpriteRenderer) sr.enabled = enabled;
    }

    public void SetState(StructureState newState)
    {
        state = newState;
        StructureEvent.Trigger(StructureEventType.StateChange);

         if (state == StructureState.Constructing)
        {
            SetEnabled(false);
        }
    }

    public void Repair()
    {
        Build();
        GetComponentInChildren<Health>(true).Respawn();
    }

    public int GetRepairPrice()
    {
        float currentHP = GetComponent<Health>().CurrentHealthPercentage().PercentageToDecimal();

        return Mathf.RoundToInt((structureData.price / 4f) + ((1f - currentHP) * (structureData.price / 2)));
    }

    protected virtual void Deconstruct()
    {
        deconstructFeedback?.PlayFeedbacks();
        state = StructureState.Destroyed;
    }

    public void CancelBuild()
    {
        StopAllCoroutines();
        buildFeedback?.StopFeedbacks();
    }

    public virtual void OnEnable()
    {
        if (rotateAroundCenter)
        {
            transform.RotateAroundOrigin();
        }
        InitialiseComponents();
        Initialise();
    }

    public void AddUpgradedListener(params Action[] actionsToAdd)
    {
        for (int i = 0; i < actionsToAdd.Length; i++)
        {
            onUpgraded += actionsToAdd[i];
        }
    }

    public void RemoveUpgradedListener(params Action[] actionsToAdd)
    {
        for (int i = 0; i < actionsToAdd.Length; i++)
        {
            onUpgraded -= actionsToAdd[i];
        }
    }
}

public enum StructureState { Intact, Damaged, Destroyed, Constructing }



