using Sirenix.OdinInspector;
using System;
using Unity.Mathematics;
using UnityEngine;

public class WalkAction : AIAction
{
    [FoldoutGroup("Action Settings"), SerializeField]
    private float walkSpeed;


    public override void StartAction()
    {
        AIMovement.SetHorizontalSpeed(walkSpeed);
    }

    public override bool Executable()
    {
        return targetSelection.TargetAvailable();
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
        return true;
    }

    public override void EndAction()
    {
    
    }
}

public class Herding
{
    private EntityObject entity;
    private LineCollisionDetector collisionDetector;
    private event Action action;

    public Herding(EntityObject entity, ColliderData colliderData, Transform startPos, Transform endPos)
    {
        this.entity = entity;
        collisionDetector = new(colliderData, startPos, endPos);
        collisionDetector.AddHitRequirment(SameSpecies);
    }

    public bool CheckForAlly()
    {
        Collider2D[] allycolliders = collisionDetector.GetTargetLayerCollisions();
        return allycolliders != null && allycolliders.Length > 0;
    }

    public bool SameSpecies(Collider2D collider)
    {
        Health entityHealth = collider.GetComponentInChildren<Health>();

        if (!entityHealth) return false;

        return entityHealth.entityObject == entity;
    }
}