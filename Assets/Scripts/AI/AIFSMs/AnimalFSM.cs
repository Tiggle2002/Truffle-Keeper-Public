using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType { Flying, Nonflying }
public enum AnimalBehaviourType { Passive, Aggressive, Defensive }

public class AnimalFSM : NPCFSM
{
    #region State Checklist
    public AnimalType AnimalType { get { return animalType; } }
    public AnimalBehaviourType AnimalBehaviourType { get { return animalBehaviourType; } }

    [SerializeField] private AnimalType animalType;
    [SerializeField] private AnimalBehaviourType animalBehaviourType;
    #endregion

    #region FSM Methods
    protected override void ConstructStates()
    {
        if (idleState)
        {
            IdleState idleState = new(this);
            idleState.AddTransition(AITransition.WayPointsAvailable, AIStateID.Patrol);
            idleState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            idleState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            idleState.AddTransition(AITransition.Scared, AIStateID.Flee);
            AddState(idleState);
        }

        if (patrolState)
        {
            PatrolState patrolState = new(this);
            patrolState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            patrolState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            patrolState.AddTransition(AITransition.Scared, AIStateID.Flee);
            patrolState.AddTransition(AITransition.NothingToDo, AIStateID.Idle);
            AddState(patrolState);
        }

        if (chaseState)
        {
            ChaseState walkState = new(this);
            walkState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            walkState.AddTransition(AITransition.Scared, AIStateID.Flee);
            walkState.AddTransition(AITransition.WayPointsAvailable, AIStateID.Patrol);
            AddState(walkState);
        }

        if (fleeState)
        {
            FleeState fleeState = new(this);
            fleeState.AddTransition(AITransition.WayPointsAvailable, AIStateID.Patrol);
            AddState(fleeState);
        }

        if (attackState)
        {
            AttackState attackState = new(this);
            attackState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            attackState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            AddState(attackState);
        }

        if (damagedState)
        {
            DamagedState damagedState = new(this);
            damagedState.AddTransition(AITransition.TargetSpotted, AIStateID.Chase);
            damagedState.AddTransition(AITransition.TargetAttackable, AIStateID.Attack);
            AddState(damagedState);
        }

        if (deathState)
        {
            DeathState deathState = new(this);
            AddState(deathState);
        }
    }
    #endregion
}
