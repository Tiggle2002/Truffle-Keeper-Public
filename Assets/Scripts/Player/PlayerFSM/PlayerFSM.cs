using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerTransition { None, NoInput, WalkInput, RunInput, JumpPressed, Attacking,
    NoHealth,
    DashInputRecieved
}

public enum PlayerStateID { None, Idle, Walk, Jump, Attack, Death,
    Dash
}

public class PlayerFSM : FSM<PlayerTransition, PlayerStateID>
{
    public bool canDash = true;
    public float dashCooldown;
    public float dashLength;
    public float dashSpeed;

    #region References 
    public Rigidbody2D rb { get; private set; }
    public BoxCollider2D bc { get; private set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerMovement movement { get; private set; }
    public Animator torsoAnimator { get; private set; }

    public SpriteRenderer leftArmRenderer;
    public Animator leftArmAnimator;

    public SpriteRenderer rightArmRenderer;
    public Animator rightArmAnimator;
    public float MovementDirection { get { return playerInput.actions["Move"].ReadValue<Vector2>().x; } }
    #endregion

    #region Update Methods
    protected override void AwakeFSM()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        movement = transform.Find("Player Movement").GetComponent<PlayerMovement>();
        
        torsoAnimator = GameObject.Find("Player Body Sprite").GetComponent<Animator>();

        leftArmRenderer = GameObject.Find("PlayerLeftArm").GetComponent<SpriteRenderer>();
        leftArmAnimator = GameObject.Find("PlayerLeftArm").GetComponent<Animator>();

        rightArmRenderer = GameObject.Find("PlayerRightArm").GetComponent<SpriteRenderer>();
        rightArmAnimator = GameObject.Find("PlayerRightArm").GetComponent<Animator>();
        
        playerInput = GetComponent<PlayerInput>();

        ConstructStates();
    }

    protected override void InitialiseFSM()
    {
        
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
        PlayerIdle idleState = new PlayerIdle();
        idleState.AddTransition(PlayerTransition.WalkInput, PlayerStateID.Walk);
        idleState.AddTransition(PlayerTransition.JumpPressed, PlayerStateID.Jump);
        idleState.AddTransition(PlayerTransition.DashInputRecieved, PlayerStateID.Dash);
        idleState.AddTransition(PlayerTransition.Attacking, PlayerStateID.Attack);
        idleState.AddTransition(PlayerTransition.NoHealth, PlayerStateID.Death);

        AddState(idleState);

        PlayerWalk walkState = new PlayerWalk();
        walkState.AddTransition(PlayerTransition.NoInput, PlayerStateID.Idle);
        walkState.AddTransition(PlayerTransition.JumpPressed, PlayerStateID.Jump);
        walkState.AddTransition(PlayerTransition.DashInputRecieved, PlayerStateID.Dash);
        walkState.AddTransition(PlayerTransition.Attacking, PlayerStateID.Attack);
        walkState.AddTransition(PlayerTransition.NoHealth, PlayerStateID.Death);

        AddState(walkState);

        PlayerJump jumpState = new PlayerJump();
        jumpState.AddTransition(PlayerTransition.NoInput, PlayerStateID.Idle);
        jumpState.AddTransition(PlayerTransition.WalkInput, PlayerStateID.Walk);
        jumpState.AddTransition(PlayerTransition.JumpPressed, PlayerStateID.Jump);
        jumpState.AddTransition(PlayerTransition.Attacking, PlayerStateID.Attack);
        jumpState.AddTransition(PlayerTransition.DashInputRecieved, PlayerStateID.Dash);
        jumpState.AddTransition(PlayerTransition.NoHealth, PlayerStateID.Death);

        AddState(jumpState);

        PlayerAttack attackState = new PlayerAttack();
        attackState.AddTransition(PlayerTransition.NoInput, PlayerStateID.Idle);
        attackState.AddTransition(PlayerTransition.WalkInput, PlayerStateID.Walk);
        attackState.AddTransition(PlayerTransition.DashInputRecieved, PlayerStateID.Dash);
        attackState.AddTransition(PlayerTransition.JumpPressed, PlayerStateID.Jump);
        attackState.AddTransition(PlayerTransition.NoHealth, PlayerStateID.Death);

        AddState(attackState);

        PlayerDash dashState = new PlayerDash();
        dashState.AddTransition(PlayerTransition.NoInput, PlayerStateID.Idle);
        dashState.AddTransition(PlayerTransition.WalkInput, PlayerStateID.Walk);
        dashState.AddTransition(PlayerTransition.JumpPressed, PlayerStateID.Jump);
        dashState.AddTransition(PlayerTransition.NoHealth, PlayerStateID.Death);

        //AddState(dashState);

        PlayerDeath playerDeath = new PlayerDeath();

        AddState(playerDeath);
    }

    public IEnumerator PlayAnimation(int animationCode, float playDelay, string bodyPart)
    {
        yield return new WaitForSeconds(playDelay);
        if (bodyPart == "Body")
        {
            torsoAnimator.Play(animationCode, -1, 0);
        }
        if (bodyPart == "LeftArm")
        {
            leftArmAnimator.Play(animationCode, -1, 0);
        }
         if (bodyPart == "RightArm")
        {
            rightArmAnimator.Play(animationCode, -1, 0);
        }
    }

    public IEnumerator CrossFadeAnimation(int animationCode, float playDelay, string bodyPart)
    {
        yield return new WaitForSeconds(playDelay);
        if (bodyPart == "Body")
        {
            torsoAnimator.CrossFade(animationCode, 0.2f,-1, 0);
        }
        if (bodyPart == "LeftArm")
        {
            leftArmAnimator.CrossFade(animationCode, 0.2f, -1, 0);
        }
        if (bodyPart == "RightArm")
        {
            rightArmAnimator.CrossFade(animationCode, 0.2f, -1, 0);
        }
    }

    public void DisablePlayer()
    {
        playerInput.enabled = false;
        movement.enabled = false;
    }

    public void SetAnimatorSpeed(float animatorSpeed)
    {
        torsoAnimator.speed = animatorSpeed;
    
        leftArmAnimator.speed = animatorSpeed;

        rightArmAnimator.speed = animatorSpeed;
    }
}
