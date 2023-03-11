using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AITransition { None, TargetSpotted, TargetAttackable, BeenDamaged, TargetsDefeated, NoHealth, Respawn, Scared, WayPointsAvailable,
    NothingToDo
}

public enum AIStateID { None, Chase, Attack, Damaged, Celebrate, Death, Patrol, Flee,
    Idle
}

public abstract class NPCFSM : FSM<AITransition, AIStateID>
{
    [DictionaryDrawerSettings(KeyLabel = "State ID", ValueLabel = "AI Action"), PropertyOrder(10f)]
    public Dictionary<AIStateID, AIAction> actionMap = new Dictionary<AIStateID, AIAction>();

    [SerializeField, PropertyOrder(2f)] private AIStateID initialStateID;

    #region References
    public Health Health { get; private set; }
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Movement movement;
    protected SpriteRenderer minimapIcon;
    #endregion

    #region State Checklist
    [FoldoutGroup("States"), SerializeField, PropertyOrder(3f)] protected bool idleState = false;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(4f)] protected bool patrolState = true;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(5f)] protected bool chaseState = true;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(6f)] protected bool fleeState = true;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(7f)] protected bool attackState = true;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(8f)] protected bool damagedState = true;
    [FoldoutGroup("States"), SerializeField, PropertyOrder(9f)] protected bool deathState = true;
    #endregion

    #region FSM Methods
    protected override void AwakeFSM()
    {
        animator = GetComponentInChildren<Animator>();
        Health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponentInChildren<Movement>(true);
        minimapIcon = transform.Find("Minimap Icon")?.GetComponent<SpriteRenderer>();
        ConstructStates();
        AddCommonTransitions();
    }

    protected override void InitialiseFSM()
    {
        SetInitialState();
    }

    protected override void UpdateFSM()
    {
        CurrentState.RunState();
        CurrentState.CheckForChange();
    }

    protected override void FixedUpdateFSM()
    {
        CurrentState.FixedRunState();
    }
    #endregion

    protected override void ConstructStates()
    {
        if (patrolState)
        {
            PatrolState patrolState = new(this);
            patrolState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            patrolState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            patrolState.AddTransition(AITransition.Scared, AIStateID.Flee);
            AddState(patrolState);
        }

        if (chaseState)
        {
            ChaseState walkState = new(this);
            walkState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            walkState.AddTransition(AITransition.Respawn, AIStateID.Chase);
            walkState.AddTransition(AITransition.Scared, AIStateID.Flee);
            AddState(walkState);
        }

        if (fleeState)
        {
            FleeState fleeState = new(this);
            fleeState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            fleeState.AddTransition(AITransition.Respawn, AIStateID.Chase);
            AddState(fleeState);
        }

        if (attackState)
        {
            AttackState attackState = new(this);
            attackState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            attackState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            attackState.AddTransition(AITransition.Respawn, AIStateID.Chase);
            attackState.AddTransition(AITransition.WayPointsAvailable, AIStateID.Patrol);
            AddState(attackState);
        }

        if (damagedState)
        {
            DamagedState damagedState = new(this);
            damagedState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            damagedState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            damagedState.AddTransition(AITransition.Respawn, AIStateID.Chase);
            damagedState.AddTransition(AITransition.WayPointsAvailable, AIStateID.Patrol);
            AddState(damagedState);
        }

        if (deathState)
        {
            DeathState deathState = new(this);
            deathState.AddTransition(AITransition.Respawn, AIStateID.Chase);
            AddState(deathState);
        }
    }

    protected virtual void AddCommonTransitions()
    {
        if (damagedState)
        {
            AddTransitionToAllStates(AITransition.BeenDamaged, AIStateID.Damaged, exceptions: AIStateID.Death);
        }
        if (deathState)
        {
            AddTransitionToAllStates(AITransition.NoHealth, AIStateID.Death, exceptions: AIStateID.Death);
        }
    }

    public void SetInitialState()
    {
        OverrideState(initialStateID);
    }

    public AIAction GetAction(AIStateID type)
    {
        if (actionMap.TryGetValue(type, out AIAction action))
        {
            return action;
        }
        else
        {
            return null;
        }
    }

    #region Animating Methods
    public void PlayAnimation(int animationCode)
    {
        animator.Play(animationCode);
    }

    public void PlayAnimation(string animationCode)
    {
        animator.Play(animationCode);
    }

    public void PlayAnimation(int animationCode, float time)
    {
        animator.Play(animationCode, -1, time);
    }

    public void PlayAnimation(string animationCode, float time)
    {
        animator.Play(animationCode, -1, time);
    }
    #endregion

    protected virtual void OnEnable()
    {
        InitialiseStates();
        EnableNPC();
        SetInitialState();
    }

    public virtual void EnableNPC()
    {
        Health.enabled = true;
        if (minimapIcon)
        {
            minimapIcon.enabled = true;
        }
    }

    public virtual IEnumerator DisableNPC(float delay)
    {
        Health.enabled = false;
        movement.rb.HaltHorizontalMovementImmediate();
        movement.UpdateGravityScale(2f);
        if (minimapIcon)
        {
            minimapIcon.enabled = false;
        }
        yield return new WaitForSeconds(delay);
        StartCoroutine(DisableWhenGrounded());
    }

    private IEnumerator DisableWhenGrounded()
    {
        yield return new WaitUntil(() => movement.IsGrounded());
        gameObject.SetActive(false);
    }
}
