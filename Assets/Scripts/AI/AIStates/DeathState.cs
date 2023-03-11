using System.Collections;
using UnityEngine;

public class DeathState : NPCFSMState
{
    public DeathState(NPCFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Death;
    }

    #region FSM Methods
    public override void OnEnter()
    {
        base.OnEnter();
        FSM.PlayAnimation(Death, 0);
        FSM.StartCoroutine(FSM.DisableNPC(5f));
    }

    public override void CheckForChange()
    {

    }

    public override void RunState()
    {

    }

    public override void FixedRunState()
    {

    }

    public override void OnExit()
    {
        base.OnExit();
    }
    #endregion
}
