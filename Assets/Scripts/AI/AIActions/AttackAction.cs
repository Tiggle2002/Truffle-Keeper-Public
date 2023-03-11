using Sirenix.OdinInspector;
using UnityEngine;

public class AttackAction : AIAction
{
    [SerializeField, FoldoutGroup("Attack")] 
    protected DamageAbility damageAbility;
    [SerializeField, FoldoutGroup("Attack")] 
    protected float attackStartTime;
    [SerializeField, FoldoutGroup("Attack")]
    protected Coroutine attackCoroutine;

    public override bool Executable() => damageAbility.TargetsAvailable() && AIMovement.IsGrounded() && CooldownDone() && base.Executable();

    public override void StartAction()
    {
        actionTimer.ResetCountdown();
        attackCoroutine = StartCoroutine(damageAbility.PerformAbilityCoroutine(attackStartTime));
    }

    public override void FixedUpdateAction()
    {
         AIMovement.rb.HaltHorizontalMovementImmediate();
    }

    public override bool Over()
    {
        if (timed)
        {
            if (!actionTimer.Done())
            {
                return false;
            }
        }
        return true;
    }

    public override void EndAction()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            damageAbility.ResetAbility();
        }
    }
}



