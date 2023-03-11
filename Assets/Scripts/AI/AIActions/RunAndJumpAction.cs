using System.Collections;
using UnityEngine;

public class RunAndJumpAction : WalkAction
{
    [SerializeField] private float jumpRange;
    [SerializeField] private float tooCloseToJumpRange;

    [SerializeField] private float jumpCooldown;
    [SerializeField] private LayerMask targetLayers;

    private Timer jumpCooldownTimer;

    public override void Awake()
    {
        base.Awake();
        jumpCooldownTimer = new(jumpCooldown);
    }

    public override void FixedUpdateAction()
    {
        base.FixedUpdateAction();

        CheckForTargetToJumpOver();
    }

    private void CheckForTargetToJumpOver()
    {
        if (!jumpCooldownTimer.Done())
        {
            return;
        }

        if (targetSelection.TargetInRange(jumpRange) && !targetSelection.TargetInRange(tooCloseToJumpRange))
        {
            if (AIMovement.IsGrounded() && AIMovement.CanJump())
            {
                //AIMovement.PerformJumpDirectlyProportionalTo(targetSelection.TargetDistanceX() * 2f);
                jumpCooldownTimer.ResetCountdown();
            }
        }
    }

    protected override void UpdateMovementVariables()
    {
        jumpCooldownTimer.Countdown();
        if (AIMovement.IsGrounded())
        {
            base.UpdateMovementVariables();
        }
    }

    public void OnDisable()
    {
        jumpCooldownTimer.SetTimerDone();
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, jumpRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, tooCloseToJumpRange);
    }
}