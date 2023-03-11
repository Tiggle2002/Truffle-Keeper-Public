using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWalk : PlayerFSMState
{
    public PlayerWalk()
    {
        stateID = PlayerStateID.Walk;
    }

    #region FSM Methods
    public override void CheckForChange()
    {
        TransitionToJump();
        TransitionToDash();
        TransitionToIdle();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        FSM.SetAnimatorSpeed(1.5f);
        FSM.StartCoroutine(FSM.CrossFadeAnimation(BodyRun, 0, "Body"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(LeftArmRun, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(RightArmRun, 0, "RightArm"));
    }

    public override void RunState()
    {
        base.RunState();
    }

    public override void FixedRunState()
    {
        base .FixedRunState();

        movement.ApplyHorizontalVelocity(FSM.MovementDirection);
    }

    public override void OnExit()
    {
        base.OnExit();
        FSM.SetAnimatorSpeed(1f);
    }
    #endregion
}
