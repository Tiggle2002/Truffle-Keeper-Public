using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : NPCFSMState
{
    protected AIAction action;

    public PatrolState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Patrol;

        action = FSM.GetAction(stateID);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        FSM.PlayAnimation(Walking, 0);
        action.StartAction();
    }

    public override void CheckForChange()
    {
        if (CanTransitionToState(AIStateID.Flee))
        {
            FSM.TransitionToState(AITransition.Scared);
        }
        if (CanTransitionToState(AIStateID.Attack))
        {
            FSM.TransitionToState(AITransition.TargetAttackable);
        }
        if (CanTransitionToState(AIStateID.Chase))
        {
            FSM.TransitionToState(AITransition.TargetSpotted);
        }
        if (CanTransitionFromState(AIStateID.Patrol) && CanTransitionToState(AIStateID.Idle))
        {
            FSM.TransitionToState(AITransition.NothingToDo);
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
}
