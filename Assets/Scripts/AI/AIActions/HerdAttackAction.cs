using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class HerdAttackAction : AIAction
{
    [SerializeField, FoldoutGroup("Attack")]
    protected DamageAbility damageAbility;
    [SerializeField, FoldoutGroup("Attack")]
    protected float attackStartTime;
    [SerializeField, FoldoutGroup("Attack")]
    protected Transform herdDetectionStart;
    [SerializeField, FoldoutGroup("Attack")]
    protected Transform herdDetectionEnd;
    [SerializeField, FoldoutGroup("Attack")]
    protected ColliderData colliderData;
    protected Coroutine attackCoroutine;

    private Herding herdingBevaviour;
    private bool herding;

    public void Start()
    {
        herdingBevaviour = new(FSM.Health.entityObject, colliderData, herdDetectionStart, herdDetectionEnd);
    }

    public override void Update()
    {
        base.Update();

        bool allyNear = herdingBevaviour.CheckForAlly();
        if (allyNear && !herding)
        {
            herding = true;
            (damageAbility as MeleeWithProjectile).ExtendAttackRange(new(10, 0));
        }
        else if (!allyNear && herding)
        {
            herding = false;
            (damageAbility as MeleeWithProjectile).ExtendAttackRange(new(-10, 0));
        }
    }

    public override bool Executable() => damageAbility.TargetsAvailable() && AIMovement.IsGrounded() && CooldownDone();

    public override void StartAction()
    {
        base.StartAction();
        actionTimer.ResetCountdown();
        actionCooldown.ResetCountdown();
        attackCoroutine = StartCoroutine(damageAbility.PerformAbilityCoroutine(attackStartTime));
    }

    public override void FixedUpdateAction()
    {
        AIMovement.rb.HaltHorizontalMovementImmediate();
    }

    public override bool Over() => TimerDone();

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