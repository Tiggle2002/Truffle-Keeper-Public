using System.Collections;
using UnityEngine;

public class PlayerDash : PlayerFSMState
{
    private bool dashOver;

    public PlayerDash()
    {
        stateID = PlayerStateID.Dash;
    }

    private IEnumerator ExitDash()
    {
        dashOver = false;
        float jumpTimer =FSM.dashLength;
        while (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            yield return null;
        }
        dashOver = true;
    }

    #region FSM Methods
    public override void CheckForChange()
    {
        if (!dashOver) return;

        TransitionToWalk();
        TransitionToIdle();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        FSM.canDash = false;
        FSM.StartCoroutine(TimerUtilites.Time(FSM.dashCooldown, () => FSM.canDash = true));
        FSM.StartCoroutine(ExitDash());
        FSM.SetAnimatorSpeed(5f);
        FSM.StartCoroutine(FSM.CrossFadeAnimation(BodyRun, 0, "Body"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(LeftArmRun, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.CrossFadeAnimation(RightArmRun, 0, "RightArm"));
        FSM.rb.HaltMovementImmediate();
        movement.SetHorizontalForceAccordingToInput(FSM.dashSpeed, FSM.MovementDirection);
    }

    public override void RunState()
    {
        base.RunState();
    }

    public override void FixedRunState()
    {
        base.FixedRunState();
    }

    public override void OnExit()
    {
        base.OnExit();
        FSM.rb.HaltHorizontalMovementImmediate();
        FSM.SetAnimatorSpeed(1f);
    }
    #endregion
}
