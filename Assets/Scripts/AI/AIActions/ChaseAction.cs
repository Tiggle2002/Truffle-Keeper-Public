using Sirenix.OdinInspector;
using UnityEngine;

public class ChaseAction : AIAction
{
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected bool chaseAnimationName;

    [FoldoutGroup("Movement Settings"), SerializeField]
    private float chaseSpeed;

    public override void StartAction()
    {
        AIMovement.SetHorizontalSpeed(chaseSpeed);
    }

    public override bool Executable()
    {
        return base.Executable() && targetSelection.TargetAvailable();
    }

    public override void UpdateAction()
    {
        base.UpdateAction();
        UpdateMovementVariables();
    }

    public override void FixedUpdateAction()
    {
        AIMovement.UpdateGravityScale();
        AIMovement.ApplyHorizontalVelocity(targetDirection);
    }

    protected virtual void UpdateMovementVariables()
    {
        targetSelection.SelectTarget();
        targetDirection = targetSelection.TargetDirectionSign();
        AIMovement.FlipInDirectionOfMovement(targetDirection);
    }

    public override bool Over()
    {
        return !targetSelection.TargetInRange(50f) && TimerDone();
    }

    public override void EndAction()
    {
        throw new System.NotImplementedException();
    }
}
