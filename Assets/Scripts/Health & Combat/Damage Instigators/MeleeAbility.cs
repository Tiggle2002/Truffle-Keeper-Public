using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using System;

public class MeleeAbility : DamageAbility
{
    #region Fields
    [FoldoutGroup("Ability References"), SerializeField]
    protected Transform meleePoint;

    [FoldoutGroup("Ability Properties"), Range(0.01f, 2f), SerializeField]
    private float meleeActiveTime = 0.05f;

    private Timer damageActiveTimer;
    protected bool damageActive;
    #endregion

    #region Unity Update Methods
    public virtual void Start()
    {
        damageActiveTimer = new(meleeActiveTime);
        damageActiveTimer.ResetCountdown();
    }
    #endregion

    #region Methods
    protected override void CreateCollider()
    {
        collisionDetector = colliderData.type == ColliderType.Circle ? new CircleCollisionDetector(colliderData, meleePoint) : new SquareCollisionDetector(colliderData, meleePoint);
        collisionDetector.AddHitAction(DamageCollider);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public override void PerformAbility()
    {
        abilityFeedback?.PlayFeedbacks();
        collisionDetector.CheckForCollisions();
        collisionDetector.Reset();
    }

    public override IEnumerator PerformAbilityCoroutine(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        damageActive = true;
        abilityFeedback?.PlayFeedbacks();

        do
        {
            damageActiveTimer.Countdown();
            collisionDetector.CheckForCollisions();
            yield return null;
        }
        while (!damageActiveTimer.Done());

        ResetAbility();
    }

    public override void ResetAbility()
    {
        damageActiveTimer.ResetCountdown();
        collisionDetector.Reset();
        damageActive = false;
    }

    public void ChangeCollider(ColliderData newCollider)
    {
        if (newCollider == colliderData)
        {
            return;
        }

        collisionDetector.ChangeColliderData(colliderData);
    }
    #endregion


    #region Data Methods
    public override bool TargetsAvailable()
    {
        Collider2D[] targetLayerCollisions = collisionDetector.GetTargetLayerCollisions();
        if (targetLayerCollisions == null)
        {
            return false;
        }

        for (int i = 0; i < targetLayerCollisions.Length; i++)
        {
            Health targetHealth = targetLayerCollisions[i].GetComponentInChildren<Health>(true);
            if (targetHealth && targetHealth.Alive())
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    protected override void OnDrawGizmos()
    {
        if (damageActive)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }

        switch (colliderData.type)
        {
            case ColliderType.Circle:
                Gizmos.DrawWireSphere(meleePoint.transform.position + colliderData.offset, colliderData.circleRadius);
                break;
            case ColliderType.Square:
                Gizmos.DrawWireCube(meleePoint.position + colliderData.offset, colliderData.squareSize);
                break;
        }
    }
}