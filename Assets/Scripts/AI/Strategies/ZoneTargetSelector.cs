using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;


public class ZoneTargetSelector : AITargetSelector
{
    #region Serialized Fields
    [SerializeField, Required, FoldoutGroup("Target Selection Settings")]
    private bool healthBasedTargets;

    [SerializeField, Required, FoldoutGroup("References")]
    private ColliderData targetDetectorData;
    #endregion

    private CircleCollisionDetector targetDetector;

    #region Unity Update Methods
    public void Awake()
    {
        targetDetector = new(targetDetectorData, transform);

        if (healthBasedTargets)
        {
            AddHitRequirement(x => x.GetComponentInChildren<Health>(true).Damagable());
        }
    }
    #endregion

    public override void SelectTarget()
    {
        if (targetDetector.ColliderOverlap())
        {
            Collider2D closestCollider = targetDetector.GetClosestCollider();
            if (closestCollider != null)
            {
                target = closestCollider.bounds.center;
                return;
            }
        }
        target = Vector2.zero;
    }

    public void ChangeColliderData(ColliderData newData)
    {
        if (newData != null)
        {
            targetDetectorData = newData;
            targetDetector.ChangeColliderData(newData);
        }
    }

    public void AddHitRequirement(Func<Collider2D, bool> requirementFunc)
    {
        targetDetector.AddHitRequirment(requirementFunc);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = TargetAvailable() ? Color.red : Color.white;
        Gizmos.DrawWireSphere(transform.position, targetDetectorData.circleRadius);
    }
}
