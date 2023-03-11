using System.Collections;
using UnityEngine;
using System;

public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
{
    private Action<PoolObject> returnToPool;

    protected virtual void OnDisable()
    {
        ReturnToPool();
    }

    public void Initialise(Action<PoolObject> returnAction)
    {
        returnToPool = returnAction;    
    }

    public virtual void ReturnToPool()
    {
        returnToPool?.Invoke(this);
    }
}
