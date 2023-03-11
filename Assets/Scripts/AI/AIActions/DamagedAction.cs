using System.Collections;
using UnityEngine;


public class DamagedAction : AIAction
{
    [SerializeField] private bool disableMovement;
    [SerializeField] private bool mustBeGroundedToTransition;


    public override void StartAction()
    {
        throw new System.NotImplementedException();
    }

    public override bool Over()
    {
        if (mustBeGroundedToTransition)
        {
            if (!AIMovement.IsGrounded())
            {
                return false;
            }
        }
        if (timed)
        {
            if (!actionTimer.Done())
            {
                return false;
            }
        }
        actionTimer.ResetCountdown();
        return true;
    }

    public override void FixedUpdateAction()
    {
        if (disableMovement)
        {
            AIMovement.ApplyHorizontalVelocity(0f);
            AIMovement.UpdateGravityScale();
        }
    }

    public override void EndAction()
    {
        throw new System.NotImplementedException();
    }

    public override bool Executable()
    {
        throw new System.NotImplementedException();
    }
}
