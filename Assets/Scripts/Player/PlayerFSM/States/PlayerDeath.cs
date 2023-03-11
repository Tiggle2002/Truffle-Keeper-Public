using System.Collections;
using UnityEngine;

public class PlayerDeath : PlayerFSMState
{
    public PlayerDeath()
    {
        stateID = PlayerStateID.Death;
    }

    #region FSM Methods
    public override void CheckForChange()
    {
        return;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        FSM.DisablePlayer();

        FSM.StartCoroutine(FSM.PlayAnimation(BodyDeath, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmDeath, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmDeath, 0, "RightArm"));
    }

    public override void RunState()
    {
        return;
    }

    public override void FixedRunState()
    {
        base.OnExit();
        base.FixedRunState();
        FSM.rb.HaltHorizontalMovementImmediate();
    }
    #endregion
}
