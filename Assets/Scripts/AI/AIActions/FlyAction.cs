using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAction : AIAction
{
    float verticalTargetDirection = 0f;
    private Coroutine targetSelectionCoroutine;
    private bool actionOver = false;

    [FoldoutGroup("Flight Settings"), SerializeField, Range(0f, 100f)]
    private float horizontalVelocity;
    [FoldoutGroup("Flight Settings"), SerializeField, Range(0f, 100f)]
    private float verticalVelocity;

    public void Start()
    {
        targetSelection.SelectTarget();
    }

    public override bool Executable()
    {
        return true;
    }

    public override void StartAction()
    {
        AIMovement.SetHorizontalSpeed(horizontalVelocity + Random.Range(-1, 1));
        AIMovement.SetVerticalSpeed(verticalVelocity);
        actionOver = false;
    }

    public override void UpdateAction()
    {
        if (actionOver) return;

        base.UpdateAction();
        SelectTarget();

        targetDirection = targetSelection.TargetDirectionSign();
        verticalTargetDirection = targetSelection.TargetVerticalDirectionSign();
        AIMovement.FlipInDirectionOfMovement();
    }

    private void SelectTarget()
    {
        if (targetSelection.TargetInRange(0.5f) && targetSelectionCoroutine == null)
        {
            SelectNewTargetAfterDelay();
        }
    }

    private void SelectNewTargetAfterDelay()
    {
        if (Random.value < 0.5)
        {
            actionOver = true;
        }
        else
        {
            targetSelection.SelectTarget();
        }
    }

    public override void FixedUpdateAction()
    {
        if (actionOver) return;

        if (targetSelection.TargetDistanceX() > 0.2f)
        {
            AIMovement.ApplyHorizontalVelocity(targetDirection);
        }

        AIMovement.ApplyVerticalVelocity(targetSelection.TargetVerticalDirectionSign());
    }

    public override bool Over() => TimerDone() && actionOver;

    public override void EndAction() { }
}
