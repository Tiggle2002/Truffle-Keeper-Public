using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerIdle : PlayerFSMState
{
    public PlayerIdle()
    {
        stateID = PlayerStateID.Idle;
    }

    #region FSM Events
    public override void CheckForChange()
    {
        TransitionToJump();
        TransitionToWalk();
        TransitionToDash();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        FSM.StartCoroutine(FSM.CrossFadeAnimation(BodyIdle, 0, "Body"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(LeftArmIdle, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(RightArmIdle, 0, "RightArm"));
        }

    public override void FixedRunState()
    {
        base.FixedRunState();

        movement.ApplyHorizontalVelocity(FSM.MovementDirection);
    }
    #endregion
}
