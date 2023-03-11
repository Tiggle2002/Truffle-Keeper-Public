using System.Collections;
using UnityEngine;

public class AttackState : NPCFSMState
{
    protected AIAction action;

    public AttackState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Attack;

        action = FSM.GetAction(stateID);
    }

    #region FSM Methods
    public override void CheckForChange()
    {
        if (!CanTransitionFromState(AIStateID.Attack)) return;
        
        if (CanTransitionToState(AIStateID.Attack))
        {
            FSM.TransitionToState(AITransition.TargetAttackable);
        }
        else if (CanTransitionToState(AIStateID.Chase))
        {
            FSM.TransitionToState(AITransition.TargetSpotted);
        }
        else if (CanTransitionToState(AIStateID.Patrol))
        {
            FSM.TransitionToState(AITransition.WayPointsAvailable);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (!action.Animates())
        {
            FSM.PlayAnimation(Attacking, 0);
        }
        action.StartAction();
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
        action.EndAction();
        base.OnExit();
    }
    #endregion
}
