using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState<Transition, State> : IDisposable
{
    private Dictionary<Transition, State> stateMap = new Dictionary<Transition, State>();

    protected State stateID;

    public State ID {  get { return stateID; } }

    public State GetCorrespondingState(Transition transition)
    {
        if (stateMap.ContainsKey(transition))
        {
            return stateMap[transition];
        }

        return default(State);
    }

    public void AddTransition(Transition transition, State ID)
    {
        stateMap.Add(transition, ID);
    }

    public void RemoveTransition(Transition transition, State ID)
    {
        stateMap.Add(transition, ID);
    }

    public abstract void OnEnter();

    public abstract void RunState();

    public abstract void FixedRunState();

    public abstract void CheckForChange();

    public abstract void OnExit();

    public abstract void Initialise();

    public abstract void Dispose();
}
