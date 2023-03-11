using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

public abstract class AITargetSelector : MonoBehaviour
{
    [SerializeField, Required, FoldoutGroup("Target Selection Settings")] 
    private bool selectTargetOnUpdate = false;

    public Vector2 Target { get { return target; } }

    protected Vector2 target;

    public abstract void SelectTarget();

    public void Update()
    {
        if (selectTargetOnUpdate)
        {
            SelectTarget();
        }
    }

    public virtual bool TargetAvailable() => target != Vector2.zero;

    public Vector2 GetTargetDirection() => (Target - (Vector2)transform.position).normalized;

    public Vector2 GetTargetDirectionFrom(Transform fromTransform) => (Target - (Vector2)fromTransform.position).normalized;

    public float TargetDirectionSign() => TargetAvailable() ? Mathf.Sign(Target.x - transform.position.x) : 0;

    public float TargetVerticalDirectionSign() => TargetAvailable() ? Mathf.Sign(Target.y - transform.position.y) : 0;

    public bool TargetInRange(float range) => TargetAvailable() && Vector2.Distance(transform.position, Target) < range;

    public float TargetDistanceX() => TargetAvailable() ? Mathf.Abs(Target.x - transform.position.x) : 0;
}
