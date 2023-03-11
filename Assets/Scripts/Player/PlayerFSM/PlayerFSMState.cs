using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SurvivalElements;

public class PlayerFSMState : FSMState<PlayerTransition, PlayerStateID>, TEventListener<ItemEvent>, TEventListener<GameEvent>
{
    #region References
    protected PlayerFSM FSM;
    protected PlayerInput playerInput;
    protected PlayerMovement movement;
    protected InputAction jumpAction;
    protected InputAction dashAction;
    #endregion

    #region Variables
    protected bool usingItem;
    protected bool rotationLocked = false;
    #endregion

    #region Animator Hashmap
    #region Attack Animations
    protected static readonly int BodyFirstSwing = Animator.StringToHash("PlayerBodyFirstSwing");
    protected static readonly int BodySecondSwing = Animator.StringToHash("PlayerBodySecondSwing");
    protected static readonly int BodyFirstThrust = Animator.StringToHash("PlayerBodyFirstThrust");
    protected static readonly int BodySecondThrust = Animator.StringToHash("PlayerBodySecondThrust");
    protected static readonly int BodyFirstStab = Animator.StringToHash("PlayerBodyFirstStab");
    protected static readonly int BodySecondStab = Animator.StringToHash("PlayerBodySecondStab");

    protected static readonly int LeftArmFirstSwing = Animator.StringToHash("PlayerLeftArmFirstSwing");
    protected static readonly int LeftArmSecondSwing = Animator.StringToHash("PlayerLeftArmSecondSwing");
    protected static readonly int LeftArmFirstThurst = Animator.StringToHash("PlayerLeftArmFirstThrust");
    protected static readonly int LeftArmSecondThrust = Animator.StringToHash("PlayerLeftArmSecondThrust");
    protected static readonly int LeftArmFirstStab = Animator.StringToHash("PlayerLeftArmFirstStab");
    protected static readonly int LeftArmSecondStab = Animator.StringToHash("PlayerLeftArmSecondStab");

    protected static readonly int RightArmFirstSwing = Animator.StringToHash("PlayerRightArmFirstSwing");
    protected static readonly int RightArmSecondSwing = Animator.StringToHash("PlayerRightArmSecondSwing");
    protected static readonly int RightArmFirstThurst = Animator.StringToHash("PlayerRightArmFirstThrust");
    protected static readonly int RightArmSecondThrust = Animator.StringToHash("PlayerRightArmSecondThrust");
    protected static readonly int RightArmFirstStab = Animator.StringToHash("PlayerRightArmFirstStab");
    protected static readonly int RigthArmSecondStab = Animator.StringToHash("PlayerRightArmSecondStab");

    #endregion

    protected static readonly int BodyIdle = Animator.StringToHash("PlayerBodyIdle");
    protected static readonly int BodyRun = Animator.StringToHash("PlayerBodyRun");

    protected static readonly int BodyJumpRise = Animator.StringToHash("PlayerBodyJump");
    protected static readonly int LeftArmJumpRise = Animator.StringToHash("PlayerLeftArmJump");
    protected static readonly int RightArmJumpRise = Animator.StringToHash("PlayerRightArmJump");

    protected static readonly int BodyJumpFall = Animator.StringToHash("PlayerBodyFall");
    protected static readonly int LeftArmJumpFall = Animator.StringToHash("PlayerLeftArmFall");
    protected static readonly int RightArmJumpFall = Animator.StringToHash("PlayerRightArmFall");

    protected static readonly int LeftArmIdle = Animator.StringToHash("PlayerLeftArmIdle");
    protected static readonly int RightArmIdle = Animator.StringToHash("PlayerRightArmIdle");

    protected static readonly int LeftArmRun = Animator.StringToHash("PlayerLeftArmRun");
    protected static readonly int RightArmRun = Animator.StringToHash("PlayerRightArmRun");



    #region Death Animation
    protected static readonly int BodyDeath = Animator.StringToHash("PlayerBodyDeath");
    protected static readonly int LeftArmDeath = Animator.StringToHash("PlayerLeftArmDeath");
    protected static readonly int RightArmDeath = Animator.StringToHash("PlayerRightArmDeath");
    #endregion
    #endregion

    public PlayerFSMState()
    {
        this.FSM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFSM>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        movement = this.FSM.movement;

        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        this.Subscribe<ItemEvent>();
        this.Subscribe<GameEvent>();
    }

    #region FSM Events
    public override void CheckForChange() { }
    
    public override void OnEnter() { }

    public override void RunState()
    {
        if (!rotationLocked)
        {
            movement.FlipInDirectionOfMovement(FSM.MovementDirection);
        }
    }

    public override void FixedRunState()
    {
        movement.UpdateGravityScale();

    }

    public override void OnExit() { }

    public override void Initialise() { }

    protected void TransitionToJump()
    {
        if (movement.WithinCoyoteTime() && jumpAction.triggered)
        {
            FSM.TransitionToState(PlayerTransition.JumpPressed);
        }
    }

    protected void TransitionToWalk()
    {
        if (movement.IsGrounded() && FSM.MovementDirection != 0)
        {
            FSM.TransitionToState(PlayerTransition.WalkInput);
        }
    }

    protected void TransitionToIdle()
    {
        if (movement.IsGrounded() && FSM.MovementDirection == 0)
        {
            FSM.TransitionToState(PlayerTransition.NoInput);
        }
    }

    protected void TransitionToDash()
    {
        if (FSM.MovementDirection != 0 && dashAction.triggered && FSM.canDash)
        {
            FSM.TransitionToState(PlayerTransition.DashInputRecieved);
        }
    }

    public virtual void OnEvent(ItemEvent eventData)
    {
        switch (eventData.eventType)
        {
            case ItemEventType.ItemInUse:
                SetItemUseEnabled(true);
                movement.FlipInDirectionOfMovement(PlayerManager.Instance.GetProjectileTrajectory().x);
                SetAttackStateIfMelee(eventData.eventItem);
                break;
            case ItemEventType.ItemNoLongerInUse:
                SetItemUseEnabled(false);
                break;
        }
    }
    #endregion

    protected void SetAttackStateIfMelee(ItemObject item)
    {
        if (FSM.CurrentState.ID != PlayerStateID.Attack && !item.ammoBased && item.type == ItemType.Weapon)
        {
            FSM.TransitionToState(PlayerTransition.Attacking);
            FSM.SetAnimatorSpeed(item.AnimatorSpeed());
        }
    }

    public void SetItemUseEnabled(bool enabled)
    {
        usingItem = enabled;
        rotationLocked = enabled;
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerDeath:
            case GameEventType.LevelFailed:
                FSM.SetAnimatorSpeed(1f);
                FSM.OverrideState(PlayerStateID.Death);
                break;
        }
    }

    #region Interface Methods
    public override void Dispose()
    {
        this.Unsubscribe<ItemEvent>();
        this.Unsubscribe<GameEvent>();
    }
    #endregion
}
