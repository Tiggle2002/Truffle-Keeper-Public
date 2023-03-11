using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Net;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ChargeAttackAction : AttackAction
{
    [SerializeField, FoldoutGroup("Attack"), Range(1f, 100f)]
    private float chargeSpeed;
    [SerializeField, Required, FoldoutGroup("Attack")] 
    protected ColliderData colliderData;

    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected string chargeAnimationName;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected string leapAttackAnimationName;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected string ordinaryAttackAnimationName;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected float ordinaryAttackAnimationLength;
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("playAnimation")]
    protected float leapAttackAnimationLength;

    [SerializeField] private Transform meleePoint;
    [SerializeField] private Transform chargePoint;
    [SerializeField] private float anticipationTime;
    [SerializeField] private float extraDistance;

    private Coroutine chargeCoroutine;

    bool canMove;

    private float initialPosX;
    private float chargeRange => Mathf.Abs(chargePoint.position.x - transform.position.x) + extraDistance;

    public override bool Executable()
    {
        return base.Executable() || ChargeableTarget();
    }

    public bool ChargeableTarget()
    {
        Vector3 chargeVector;
        if (AIMovement.IsFacingRight())
        {
            chargeVector = new Vector3(chargePoint.transform.position.x - 5, chargePoint.transform.position.y);
        }
        else
        {
            chargeVector = new Vector3(chargePoint.transform.position.x + 5, chargePoint.transform.position.y);
        }
     

        RaycastHit2D target = Physics2D.Linecast(transform.position, chargeVector, colliderData.targetLayers);
        if (target)
        {
            return Mathf.Abs(target.transform.position.x - transform.position.x) > 10f && actionCooldown.Done();
        }
        return false;
    }

    public override void StartAction()
    {
        canMove = false;
 
        actionTimer.ResetCountdown();

        PerformChargeOrAttack();
    }

    private void PerformChargeOrAttack()
    {
        if (damageAbility.TargetsAvailable())
        {
            attackCoroutine = StartCoroutine(PerformAttack(ordinaryAttackAnimationName, ordinaryAttackAnimationLength, 0.6f)); //Hardcoded Delay - Needs Fix
        }
        else
        {
            chargeCoroutine = StartCoroutine(BeginCharge());
        }
    }

    private IEnumerator BeginCharge()
    {
        initialPosX = transform.position.x;
        AIMovement.SetHorizontalSpeed(chargeSpeed);
        FSM.PlayAnimation(chargeAnimationName);
        yield return new WaitForSeconds(anticipationTime);
        canMove = true;
        yield return new WaitUntil(ShouldAttack);
        canMove = false;
        attackCoroutine = StartCoroutine(PerformAttack(leapAttackAnimationName, leapAttackAnimationLength, attackStartTime));
        actionCooldown?.ResetCountdown();
    }

    private bool ShouldAttack()
    {
        return Mathf.Abs(transform.position.x - initialPosX) > chargeRange || damageAbility.TargetsAvailable();
    }

    private IEnumerator PerformAttack(string animation, float animationLength, float delay)
    {
        StartCoroutine(damageAbility.PerformAbilityCoroutine(delay));
        FSM.PlayAnimation(animation, 0);
        yield return new WaitForSeconds(animationLength);
        attackCoroutine = chargeCoroutine = null;
    }

    public Vector2 GetChargeDirection()
    {
        if (AIMovement.IsFacingRight())
        {
            return new Vector2(1, 0);
        }
        return new Vector2(-1, 0);
    }

    public override void FixedUpdateAction()
    {
        if (canMove)
        {
            AIMovement.SetHorizontalSpeed(chargeSpeed);
            float chargeDirection = GetChargeDirection().x;
            AIMovement.ApplyHorizontalVelocity(chargeDirection);
        }
        else
        {
            AIMovement.rb.HaltMovementImmediate();
        }
    }

    public override void UpdateAction()
    {
        base.UpdateAction();
        actionCooldown?.ResetCountdown();
    }

    public override void EndAction()
    {
        base.EndAction();

        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
        }
    }

    public override bool Over()
    {
        return attackCoroutine == null && chargeCoroutine == null;
    }

    public void OnDrawGizmos()
    {
        if (AIMovement.IsFacingRight())
        {
            Vector3 chargeRange = new(chargePoint.transform.position.x - 5, chargePoint.transform.position.y);
            Debug.DrawLine(meleePoint.position, chargeRange, Color.red);
        }
        else
        {
            Vector3 chargeRange = new(chargePoint.transform.position.x + 5, chargePoint.transform.position.y);
            Debug.DrawLine(meleePoint.position, chargeRange, Color.red);
        }
        Gizmos.DrawLine(meleePoint.position, chargePoint.position);
    }
}
