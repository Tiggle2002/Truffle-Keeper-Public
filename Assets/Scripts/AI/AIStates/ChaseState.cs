using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : NPCFSMState
{
    #region Variables
    protected AIAction action;
    #endregion

    public ChaseState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Chase;

        action = FSM.GetAction(stateID);
    }

    #region FSM Methods
    public override void OnEnter()
    {
        base.OnEnter();
        FSM.PlayAnimation(Running);
        action.StartAction();
    }

    public override void CheckForChange()
    {
        if (CanTransitionToState(AIStateID.Attack))
        {
            FSM.TransitionToState(AITransition.TargetAttackable);
        }
        if (CanTransitionFromState(AIStateID.Chase) && CanTransitionToState(AIStateID.Patrol))
        {
            FSM.TransitionToState(AITransition.WayPointsAvailable);
        }
        if (CanTransitionToState(AIStateID.Flee))
        {
            FSM.TransitionToState(AITransition.Scared);
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
    }
    #endregion
}
