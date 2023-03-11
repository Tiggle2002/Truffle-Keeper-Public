using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : NPCFSMState
{
    protected AIAction action;
    protected bool beenDamaged;

    public FleeState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Flee;

        action = FSM.GetAction(stateID);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (!action.Animates())
        {
            FSM.PlayAnimation(Walking, 0);
        }
        action.StartAction();
    }

    public override void CheckForChange()
    {
        if (CanTransitionFromState(AIStateID.Flee) && CanTransitionToState(AIStateID.Patrol))
        {
             FSM.TransitionToState(AITransition.WayPointsAvailable);
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