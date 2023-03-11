using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public enum AttractantID { Grass, Flower, Branch }

public class PatrolAction : AIAction
{
    [FoldoutGroup("Action Settings"), SerializeField]
    private float patrolSpeed;
    [FoldoutGroup("Action Settings"), SerializeField]
    private bool randomJumping;
    private bool actionOver = false;

    public override bool Executable()
    {
        return true;
    }

    public override void StartAction()
    {
        base.StartAction();

        targetSelection.SelectTarget();
        AIMovement.SetHorizontalSpeed(patrolSpeed);
        actionTimer?.ResetCountdown();
        actionOver = false;
    }

    public override void UpdateAction()
    {
        actionTimer?.Countdown();
        SelectTarget();
        targetDirection = targetSelection.TargetDirectionSign();
        AIMovement.FlipInDirectionOfMovement();
        TryJump();
        PlayCorrectAnimation();
    }

    private void TryJump()
    {
        if (Random.value <= 0.0025 && randomJumping)
        {
            AIMovement.JumpIfGrounded();
        }
    }

    private void PlayCorrectAnimation()
    {
        if (AIMovement.IsGrounded())
        {
            FSM.PlayAnimation("Walk");
        }
        else
        {
            FSM.PlayAnimation("Jump");
        }
    }    

    private void SelectTarget()
    {
        if (targetSelection.TargetInRange(4f))
        {
            SelectNewTargetOrCompleteAction();
        }
    }

    private void SelectNewTargetOrCompleteAction()
    {
        if (Random.value < 0.5)
        {
            actionOver = true;
        }

        targetSelection.SelectTarget();
    }

    public override void FixedUpdateAction()
    {
        AIMovement.UpdateGravityScale();
        AIMovement.ApplyHorizontalVelocity(targetDirection);
    }

    public override bool Over()
    {
        return actionOver;
    }
}
