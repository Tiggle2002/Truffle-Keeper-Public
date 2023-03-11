using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerFSMState
{
    public PlayerAttack()
    {
        stateID = PlayerStateID.Attack;
    }

    #region FSM Methods
    public override void CheckForChange()
    {
        if (jumpAction.triggered && movement.IsGrounded())
        {
            if (usingItem)
            {
                ItemEvent.Trigger(ItemEventType.ItemUseCancelled);
            }
            FSM.TransitionToState(PlayerTransition.JumpPressed);
        }
        if (dashAction.triggered && FSM.MovementDirection != 0 && FSM.canDash)
        {
            if (usingItem)
            {
                ItemEvent.Trigger(ItemEventType.ItemUseCancelled);
            }
            FSM.TransitionToState(PlayerTransition.DashInputRecieved);
        }

        if (usingItem) return;

        TransitionToWalk();
        TransitionToIdle();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        movement.SetHorizontalSpeed(5f);
        //FSM.rb.HaltHorizontalMovementImmediate();
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
        movement.SetDefault();
        FSM.SetAnimatorSpeed(1f);
    }
    #endregion

    public override void OnEvent(ItemEvent eventData)
    {
        switch (eventData.eventType)
        {
            case ItemEventType.ItemInUse:
                SetItemUseEnabled(true);
                movement.FlipInDirectionOfMovement(PlayerManager.Instance.GetProjectileTrajectory().x);
                SetAttackStateIfMelee(eventData.eventItem);
                break;
            case ItemEventType.ItemNoLongerInUse:
                SetItemUseEnabled(false);
                break;
            case ItemEventType.FirstSwing:
                PlayFirstSwing();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[0].boostX);
                break;
            case ItemEventType.SecondSwing:
                PlaySecondSwing();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[1].boostX);
                break;
            case ItemEventType.FirstThrust:
                PlayFirstThrust();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[0].boostX);
                break;
            case ItemEventType.SecondThrust:
                PlaySecondThrust();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[1].boostX);
                break;
            case ItemEventType.FirstStab:
                PlayFirstStab();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[0].boostX);
                break;
            case ItemEventType.SecondStab:
                PlaySecondStab();
                FSM.rb.HaltHorizontalMovementImmediate();
                AddForceIfPlayerIsMoving(eventData.eventItem.combos[0].boostX);
                break;
            case ItemEventType.HaltMovement:
                FSM.rb.HaltHorizontalMovementImmediate();
                break;

        }
    }

    private void AddForceIfPlayerIsMoving(float boostX)
    {
        if (FSM.MovementDirection == 0) return;
        FSM.movement.SetHorizontalForce(boostX);
    }

    #region Attack Animation Methods
    private void PlayFirstSwing()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodyFirstSwing, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmFirstSwing, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmFirstSwing, 0, "RightArm"));
    }

    private void PlaySecondSwing()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodySecondSwing, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmSecondSwing, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmSecondSwing, 0, "RightArm"));
    }

    private void PlayFirstThrust()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodyFirstThrust, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmFirstThurst, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmFirstThurst, 0, "RightArm"));
    }

    private void PlaySecondThrust()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodySecondThrust, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmSecondThrust, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmSecondThrust, 0, "RightArm"));
    }

    private void PlayFirstStab()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodyFirstStab, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmFirstStab, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RightArmFirstStab, 0, "RightArm"));
    }

    private void PlaySecondStab()
    {
        FSM.StartCoroutine(FSM.PlayAnimation(BodySecondStab, 0, "Body"));

        FSM.StartCoroutine(FSM.PlayAnimation(LeftArmSecondStab, 0, "LeftArm"));

        FSM.StartCoroutine(FSM.PlayAnimation(RigthArmSecondStab, 0, "RightArm"));
    }
    #endregion
}
