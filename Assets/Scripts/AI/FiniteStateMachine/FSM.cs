using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class FSM<Transition, State> : SerializedMonoBehaviour, IDisposable where Transition : Enum where State : Enum
{
    #region Public Variables
    [ShowInInspector, ReadOnly, PropertyOrder(1f)] protected string CurrentStateID { get { return CurrentState != null ? CurrentState.ID.ToString() : "None"; } }
    public FSMState<Transition, State> CurrentState { get { return currentState; } }

    public FSM()
    {
        stateList = new List<FSMState<Transition, State>>();
    }
    #endregion
    
    #region Private Variables
    [HideInInspector] public readonly List<FSMState<Transition, State>> stateList;

    private FSMState<Transition, State> currentState;
    #endregion

    #region State Virtual Methods
    protected abstract void AwakeFSM();

    protected abstract void InitialiseFSM();

    protected abstract void UpdateFSM();

    protected abstract void FixedUpdateFSM();

    protected abstract void ConstructStates();
    #endregion

    #region Update Methods
    private void Awake()
    {
        AwakeFSM();
    }

    private void Start()
    {
        InitialiseFSM();
    }

    private void Update()
    {
        UpdateFSM();
    }

    private void FixedUpdate()
    {
        FixedUpdateFSM();
    }
    #endregion

    #region State Management


    public void AddTransitionToAllStates(Transition transition, State stateID, params State[] exceptions)
    {
        var exceptionSet = new HashSet<State>(exceptions);
        foreach (var state in stateList)
        {
            if (exceptionSet.Contains(state.ID))
            {
                continue;
            }
            state.AddTransition(transition, stateID);
        }
    }

    public void AddState(FSMState<Transition, State> stateToAdd)
    {
        if (stateList.Contains(stateToAdd))
        {
            return;
        }

        if (stateList.Count == 0)
        {
            currentState = stateToAdd;
        }
        
        stateList.Add(stateToAdd);
    }

    public void RemoveState(FSMState<Transition, State> stateToRemove)
    {
        if (stateList.Contains(stateToRemove))
        {
            return;
        }
    }

    public void TransitionToState(Transition transition)
    {
        State stateID = CurrentState.GetCorrespondingState(transition);

        for (int i = 0; i < stateList.Count; i++)
        {
            if (stateList[i].ID.Equals(stateID))
            {
                currentState.OnExit();
                currentState = stateList[i];
                currentState.OnEnter();
                break;
            }
        }
    }

    /// <summary>
    ///  OnExit Not Called
    /// </summary>
    public void OverrideState(State state)
    {
        for (int i = 0; i < stateList.Count; i++)
        {
            if (stateList[i].ID.Equals(state))
            {
                currentState = stateList[i];
                currentState.OnEnter();
                break;
            }
        }
    }

    public virtual void InitialiseStates()
    {
        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialise();
        }
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Dispose();
        }
    }
    #endregion

    public void OnDisable()
    {
        Dispose();
    }
}



