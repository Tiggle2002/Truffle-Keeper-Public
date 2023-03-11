using System;
using System.Collections;
using UnityEngine;

public abstract class NPCFSMState : FSMState<AITransition, AIStateID>, TEventListener<HealthEvent>, TEventListener<GameEvent>, IDisposable
{
    #region References
    protected NPCFSM FSM;
    #endregion

    #region Animator Hashmap
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Walking = Animator.StringToHash("Walk");
    protected static readonly int Running = Animator.StringToHash("Run");
    protected static readonly int Attacking = Animator.StringToHash("Attacking");
    protected static readonly int Damaged = Animator.StringToHash("Damaged");
    protected static readonly int Death = Animator.StringToHash("Death");
    protected static readonly int Celebrating = Animator.StringToHash("Celebrating");
    #endregion

    public NPCFSMState(NPCFSM FSM)
    {
        this.FSM = FSM;
    }

    #region FSM Methods
    public override void OnEnter()
    {
        this.Subscribe<HealthEvent>();
    }

    public override void OnExit()
    {
        this.Unsubscribe<HealthEvent>();
    }
    #endregion

    #region State Permissions
    protected virtual bool CanTransitionFromState(AIStateID stateID)
    {
        AIAction action = FSM.GetAction(stateID);

        return action == null ? false : action.Over();
    }

    protected virtual bool CanTransitionToState(AIStateID stateID)
    {
        AIAction action = FSM.GetAction(stateID);

        return action == null ? false : action.Executable();
    }
    #endregion

    //protected IEnumerator QueueStateTransition(AITransition transitionID)
    //{
    //    if (FSM.GetAction(FSM.CurrentState.ID) == null)
    //    {
    //        yield break;
    //    }

    //    yield return new WaitUntil(FSM.GetAction(FSM.CurrentState.ID).Decide);

    //    FSM.TransitionToState(transitionID);
    //}

    protected void TryTriggerOnAttacked(Health NPCHealth)
    {
        if (NPCHealth.RecentDamager && NPCHealth.RecentDamager.PassiveDamager())
        {
            return;
        }

        OnAttacked();
    }

    protected virtual void OnAttacked()
    {
        AIAction actionToInterrupt = FSM.GetAction(FSM.CurrentState.ID);
        if (actionToInterrupt)
        {
            if (actionToInterrupt.InterruptibleByDamage)
            {
                FSM.TransitionToState(AITransition.BeenDamaged);
            }
        }
    }

    #region TEvent Methods
    public override void Initialise()
    {
        this.Subscribe<HealthEvent>();
        this.Subscribe<GameEvent>();
    }

    public override void Dispose()
    {
        this.Unsubscribe<HealthEvent>();
        this.Unsubscribe<GameEvent>();
    }

    public void OnEvent(HealthEvent eventData)
    {
        if (eventData.correspondingHealth != FSM.Health)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case HealthEventType.Damaged when FSM.Health.Alive():
                TryTriggerOnAttacked(eventData.correspondingHealth);
                break;
            case HealthEventType.Death:
                FSM.TransitionToState(AITransition.NoHealth);
                break;
            case HealthEventType.Respawn:
                FSM.SetInitialState();
                break;
        }
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                FSM.OverrideState(AIStateID.Celebrate);
                break;
        }
    }
    #endregion
}
