using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public interface IActionCondition
{
    bool Met(AIAction action);
}

public abstract class ActionCondition : IActionCondition
{
    public abstract bool Met(AIAction action);
}

[Serializable]
public class AttackedCondition : ActionCondition
{
    public override bool Met(AIAction action)
    {
        return action.GetComponentInParent<NPCFSM>().Health.CurrentHealthPercentage() < 100;
    }
}

[Serializable]
public class HealthPercentageBelow : ActionCondition
{
    [SerializeField] private int percentage;

    public override bool Met(AIAction action) => action.GetComponentInParent<NPCFSM>().Health.CurrentHealthPercentage() < percentage;
}

[Serializable]
public class TargetInRange : ActionCondition
{
    [SerializeField] private float range;

    public override bool Met(AIAction action) => action.targetSelection.TargetInRange(range);
}

public abstract class AIAction : SerializedMonoBehaviour
{
    #region Serialized Fields
    [SerializeField, FoldoutGroup("Action References"), Required] protected Movement AIMovement;
    [SerializeField, FoldoutGroup("Action References"), Required] public AITargetSelector targetSelection { get; private set; }

    [field: SerializeField, FoldoutGroup("Action Settings")]
    public bool InterruptibleByDamage { get; protected set; }
    [SerializeField, FoldoutGroup("Action Settings")]
    protected bool timed;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("timed")]
    protected float timeLength;
    [SerializeField, FoldoutGroup("Action Settings")]
    protected bool hasCooldown;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("hasCooldown")]
    protected float cooldownLength;
    [SerializeField, FoldoutGroup("Action Settings")]
    protected bool playAnimation;

    [SerializeField, FoldoutGroup("Action Feedback")]
    private MMF_Player actionStarted;
    [SerializeField, FoldoutGroup("Action Feedback")]
    private MMF_Player actionEnded;

    [SerializeField] private IActionCondition[] conditions;
    #endregion

    #region Fields
    protected NPCFSM FSM;
    protected Timer actionTimer;
    protected Timer actionCooldown;
    protected float targetDirection;
    #endregion

    public virtual void Awake()
    {
        InitialiseComponents();
        InitialiseTimers();
    }

    protected virtual void InitialiseComponents()
    {
        FSM = GetComponentInParent<NPCFSM>(true);
        if (targetSelection == null)
        {
            targetSelection = GetComponentInParent<AITargetSelector>(true);
        }
    }

    protected virtual void InitialiseTimers()
    {
        actionTimer = timed ? new Timer(timeLength, true) : null;
        actionCooldown = hasCooldown ? new Timer(cooldownLength) : null;
    }

    public virtual void Update() => actionCooldown?.Countdown();

    #region Abstract Methods
    public virtual bool Executable()
    {
        if (conditions == null || conditions.Length == 0) return true;

        foreach(var condition in conditions)
        {
            if (condition == null) continue;

            if (!condition.Met(this)) return false;
        }
        return true;
    }

    public abstract bool Over();
    #endregion

    #region Action Update Methods
    public virtual void UpdateAction()
    {
        actionTimer?.Countdown();
    }

    public virtual void FixedUpdateAction() { }
    #endregion

    #region Action Enter & Exit
    public virtual void StartAction() => actionStarted?.PlayFeedbacks();
    public virtual void EndAction() => actionEnded?.PlayFeedbacks();
    #endregion

    #region Data
    public bool TimerDone() => !timed || actionTimer.Done() == true;
    public bool CooldownDone() => !hasCooldown || actionCooldown.Done() == true;
    public bool Animates() => playAnimation;
    #endregion
}