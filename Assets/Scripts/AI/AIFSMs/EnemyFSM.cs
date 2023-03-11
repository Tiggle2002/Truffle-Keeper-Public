using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : NPCFSM
{
    #region State Checklist
    [FoldoutGroup("States"), SerializeField] protected bool celebrateState = true;
    #endregion

    #region FSM Methods
    protected override void ConstructStates()
    {
        base.ConstructStates();

        if (celebrateState)
        {
            CelebratingState celebrateState = new(this);
            AddState(celebrateState);
        }
    }

    protected override void AddCommonTransitions()
    {
        AddTransitionToAllStates(AITransition.BeenDamaged, AIStateID.Damaged, AIStateID.Death, AIStateID.Celebrate);
        AddTransitionToAllStates(AITransition.NoHealth, AIStateID.Death, AIStateID.Death, AIStateID.Celebrate);
        AddTransitionToAllStates(AITransition.TargetsDefeated, AIStateID.Celebrate, AIStateID.Death, AIStateID.Celebrate);
    }

    #endregion
}
