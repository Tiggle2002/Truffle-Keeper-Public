using System.Collections;
using UnityEngine;


public class PlayerJump : PlayerFSMState
{
    private Coroutine bufferJumpCoroutine;
    private bool fallAnimationPlayed;

    public PlayerJump()
    {
        stateID = PlayerStateID.Jump;
    }

    private IEnumerator JumpWhenGrounded()
    {
        float  jumpTimer = 0.15f;
        while (jumpTimer > 0f && !movement.IsGrounded())
        {
            jumpTimer -= Time.deltaTime;
            yield return null;
        }
        if (movement.IsGrounded())
        {
  
            FSM.TransitionToState(PlayerTransition.JumpPressed);
        }
    }

    #region FSM Events
    public override void CheckForChange()
    {
        if (jumpAction.WasPressedThisFrame())
        {
            if (movement.CanJump())
            {
                FSM.TransitionToState(PlayerTransition.JumpPressed);
            }
            else
            {
                if (bufferJumpCoroutine != null) FSM.StopCoroutine(bufferJumpCoroutine);

                bufferJumpCoroutine = FSM.StartCoroutine(JumpWhenGrounded());
            }
        }
        TransitionToWalk();
        TransitionToIdle();
        TransitionToDash();
    }

    public override void OnEnter()
    {
        movement.PerformJump();
       
        FSM.StartCoroutine(FSM.PlayAnimation(BodyJumpRise, 0, "Body"));
        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmJumpRise, 0, "LeftArm"));
        FSM.StartCoroutine(FSM.PlayAnimation(RightArmJumpRise, 0, "RightArm"));

        fallAnimationPlayed = false;
    }

    public override void RunState()
    {
       base.RunState();

        if (FSM.rb.velocity.y < 0 && !fallAnimationPlayed)
        {
            fallAnimationPlayed = true;
            FSM.StartCoroutine(FSM.PlayAnimation(BodyJumpFall, 0, "Body"));
            FSM.StartCoroutine(FSM.PlayAnimation(LeftArmJumpFall, 0, "LeftArm"));
            FSM.StartCoroutine(FSM.PlayAnimation(RightArmJumpFall, 0, "RightArm"));
        }
    }

    public override void FixedRunState()
    {
        base.FixedRunState();
        
        movement.ApplyHorizontalVelocity(FSM.MovementDirection);
    }
    #endregion
}

