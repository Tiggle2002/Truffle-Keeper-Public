using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionDetector
{
    #region Fields
    private List<Collider2D> collidersAlreadyHit = new List<Collider2D>();

    private Action<Collider2D> onHit;

    protected Func<Collider2D, bool> hitRequirement;

    private Action onImpenetrableHit;

    private Action onMaxPenetrationsReached;

    private Collider2D[] collidersToHit;

    private int penetrationCount = 0;

    protected ColliderData colliderData;

    protected Transform colliderTransform;
    #endregion

    public CollisionDetector(ColliderData colliderData, Transform colliderTransform)
    {
        this.colliderData = colliderData;
        this.colliderTransform = colliderTransform;

        Reset();
    }

    public CollisionDetector AddHitRequirment(Func<Collider2D, bool> requirementAction)
    {
        hitRequirement += requirementAction;
        return this;
    }

    public CollisionDetector AddHitAction(Action<Collider2D> actionToAdd)
    {
        onHit += actionToAdd;
        return this;
    }

    public CollisionDetector AddImpenetrableAction(Action actionToAdd)
    {
        onImpenetrableHit += actionToAdd;
        return this;
    }

    public CollisionDetector AddMaxPenetrationsReachedAction(Action actionToAdd)
    {
        onMaxPenetrationsReached += actionToAdd;
        return this;
    }

    public void ChangeColliderData(ColliderData colliderData)
    {
        if (this.colliderData == colliderData || colliderData == null)
        {
            return;
        }

        this.colliderData = colliderData;
    }

    public void EnableRepeatHitsOnCollider(Collider2D collider)
    {
        if (collidersAlreadyHit.Contains(collider))
        {
            collidersAlreadyHit.Remove(collider);
        }
    }

    public void CheckForCollisions()
    {
        collidersToHit = GetCollisions();

        TryHitColliders();
    }

    private void TryHitColliders()
    {
        for (int i = 0; i < collidersToHit.Length; i++)
        {
            if (Utilities.LayerInLayerMask(collidersToHit[i].gameObject.layer, colliderData.impenetrableLayer))
            {
                onImpenetrableHit?.Invoke();
                ImpenetrableLayerHit();
                break;
            }
            else if (!Utilities.LayerInLayerMask(collidersToHit[i].gameObject.layer, colliderData.targetLayers))
            {
                continue;
            }

            if (ColliderAlreadyHit(collidersToHit[i]))
            {
                continue;
            }

            if (hitRequirement != null)
            {
                if (hitRequirement?.Invoke(collidersToHit[i]) == false)
                {
                    continue;
                }
            }

            collidersAlreadyHit.Add(collidersToHit[i]);
            onHit?.Invoke(collidersToHit[i]);

            penetrationCount++;
            if (colliderData.limitedPenetrations && penetrationCount >= colliderData.maxPenetrationCount)
            {
                onMaxPenetrationsReached?.Invoke();
                MaxPenetrationsReached();
                break;
            }
        }
    }

    private bool ColliderAlreadyHit(Collider2D collider)
    {
        return collidersAlreadyHit.Contains(collider);
    }

    public void Reset()
    {
        collidersAlreadyHit.Clear();
        penetrationCount = 0;
    }

    public abstract bool ColliderOverlap();

    public abstract Collider2D[] GetCollisions();

    public abstract Collider2D[] GetTargetLayerCollisions();

    protected abstract void ImpenetrableLayerHit();

    protected abstract void MaxPenetrationsReached();
}

public class CircleCollisionDetector : CollisionDetector
{
    public CircleCollisionDetector(ColliderData colliderData, Transform colliderTransform) : base(colliderData, colliderTransform)
    {

    }

    public Collider2D GetClosestCollider()
    {
        Collider2D[] collisions = GetTargetLayerCollisions();
        Collider2D closestCollider = null;

        float shortestDistance = 0;

        for (int i = 0; i < collisions.Length; i++)
        {
            if (closestCollider == null)
            {
                if (hitRequirement != null && hitRequirement?.Invoke(collisions[i]) == false)
                {
                     continue;
                }

                shortestDistance = Vector2.Distance(colliderTransform.position, collisions[i].bounds.center);
                closestCollider = collisions[i];
                continue;
            }

            if (hitRequirement != null && hitRequirement?.Invoke(collisions[i]) == true)
            {
                float colliderDistance = Vector2.Distance(colliderTransform.position, collisions[i].bounds.center);

                if (colliderDistance <= shortestDistance)
                {
                    shortestDistance = colliderDistance;
                    closestCollider = collisions[i];
                }
                
            }
        }
        return closestCollider;
    }

    public override bool ColliderOverlap()
    {
        return Physics2D.OverlapCircle(colliderTransform.position + colliderData.offset, colliderData.circleRadius, colliderData.targetLayers);
    }

    public override Collider2D[] GetCollisions()
    {
        return Physics2D.OverlapCircleAll(colliderTransform.position + colliderData.offset, colliderData.circleRadius);
    }

    public override Collider2D[] GetTargetLayerCollisions()
    {
        return Physics2D.OverlapCircleAll(colliderTransform.position + colliderData.offset, colliderData.circleRadius, colliderData.targetLayers);
    }

    protected override void ImpenetrableLayerHit()
    {

    }

    protected override void MaxPenetrationsReached()
    {

    }
}

public class SquareCollisionDetector : CollisionDetector
{
    public SquareCollisionDetector(ColliderData colliderData, Transform colliderTransform) : base(colliderData, colliderTransform)
    {

    }

    public override bool ColliderOverlap()
    {
        return Physics2D.OverlapBox(colliderTransform.position + colliderData.offset, colliderData.squareSize, 0, colliderData.targetLayers);
    }

    public override Collider2D[] GetTargetLayerCollisions()
    {
        return Physics2D.OverlapBoxAll(colliderTransform.position + colliderData.offset, colliderData.squareSize, colliderData.targetLayers);
    }

    public override Collider2D[] GetCollisions()
    {
        return Physics2D.OverlapBoxAll(colliderTransform.position + colliderData.offset, colliderData.squareSize, colliderTransform.rotation.z);
    }

    protected override void ImpenetrableLayerHit()
    {

    }

    protected override void MaxPenetrationsReached()
    {

    }
}


public class LineCollisionDetector : CollisionDetector
{
    private Transform endPos;

    public LineCollisionDetector(ColliderData colliderData, Transform colliderTransform, Transform endPos) : base(colliderData, colliderTransform)
    {
        this.colliderData = colliderData;
        this.colliderTransform = colliderTransform;
        this.endPos = endPos;
    }

    public override bool ColliderOverlap()
    {
        return Physics2D.Linecast(colliderTransform.position + colliderData.offset, endPos.transform.position + colliderData.offset, colliderData.targetLayers);
    }

    public override Collider2D[] GetCollisions()
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(colliderTransform.position + colliderData.offset, endPos.transform.position + colliderData.offset, colliderData.targetLayers);
        Collider2D[] colliders = new Collider2D[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            colliders[i] = hits[i].collider;
        }
        return colliders;
    }

    public override Collider2D[] GetTargetLayerCollisions()
    {
        Collider2D[] collisions = GetCollisions();
        List<Collider2D> targetCollisions = new List<Collider2D>();
        foreach (Collider2D collider in collisions)
        {
            if (Utilities.LayerInLayerMask(collider.gameObject.layer, colliderData.targetLayers))
            {
                if (hitRequirement != null && hitRequirement?.Invoke(collider) == false)
                {
                    continue;
                }
                targetCollisions.Add(collider);
            }
        }
        return targetCollisions.ToArray();
    }

    protected override void ImpenetrableLayerHit()
    {
        // Handle impenetrable layer hit
    }

    protected override void MaxPenetrationsReached()
    {
        // Handle max penetrations reached
    }
}
