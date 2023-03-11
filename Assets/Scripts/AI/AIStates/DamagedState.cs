using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class DamagedState : NPCFSMState
{
    protected AIAction action;

    public DamagedState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Damaged;

        action = FSM.GetAction(stateID);
    }

    #region FSM Methods
    public override void OnEnter()
    {
        base.OnEnter();
        FSM.PlayAnimation(Damaged, 0);
    }

    public override void CheckForChange()
    {
        if (CanTransitionFromState(stateID))
        {
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
