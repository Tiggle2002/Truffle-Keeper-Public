using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : NPCFSMState
{
    protected AIAction action;

    public IdleState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Idle;

        action = FSM.GetAction(stateID);
    }

    #region FSM Methods
    public override void OnEnter()
    {
        base.OnEnter();
        if (!action.Animates())
        {
            FSM.PlayAnimation(Idle);
        }
        action.StartAction();
    }

    public override void CheckForChange()
    {
        if (CanTransitionToState(AIStateID.Flee))
        {
            FSM.TransitionToState(AITransition.Scared);
        }
        if (CanTransitionFromState(AIStateID.Idle) && CanTransitionToState(AIStateID.Patrol))
        {
            FSM.TransitionToState(AITransition.WayPointsAvailable);
        }
        if (CanTransitionToState(AIStateID.Chase))
        {
            FSM.TransitionToState(AITransition.TargetSpotted);
        }
        if (CanTransitionToState(AIStateID.Attack))
        {
            FSM.TransitionToState(AITransition.TargetAttackable);
        }
    }

    public override void RunState()
    {
        action.UpdateAction();
    }

    public override void FixedRunState()
    {
        action.FixedUpdateAction();
    }

    public override void OnExit()
    {
        base.OnExit();
        action.EndAction();
    }
    #endregion
}
