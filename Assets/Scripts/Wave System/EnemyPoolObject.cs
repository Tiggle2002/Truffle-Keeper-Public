using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolObject : MonoBehaviour, IPoolable<EnemyPoolObject>
{
    private Action<EnemyPoolObject> returnToPool;

    protected virtual void OnDisable()
    {
        ReturnToPool();
    }

    public void Initialise(Action<EnemyPoolObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public virtual void ReturnToPool()
    {
        returnToPool?.Invoke(this);
    }
}
