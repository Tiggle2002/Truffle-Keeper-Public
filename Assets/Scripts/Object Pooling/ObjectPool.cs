using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IPool<T>
{
    T Pull();

    void Push(T t);
}

public interface IPoolable<T>
{
    void ReturnToPool();

    void Initialise(Action<T> pushObject);
}

public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T>
{
    private Stack<T> pooledObjects = new();
    
    private GameObject prefabToPool;
    private Transform parentTransform;

    public int pooledCount
    {
        get { return pooledObjects.Count; }
    }

    public ObjectPool(GameObject prefabToPool, int poolSize, Transform parentTransform = null)
    {
        this.prefabToPool = prefabToPool;
        this.parentTransform = parentTransform;

        Spawn(poolSize);
    }

    private void Spawn(int poolSize)
    {
        T t;

        for (int i = 0; i < poolSize; i++)
        {
            t = GameObject.Instantiate(prefabToPool)?.GetComponent<T>();
            pooledObjects.Push(t);
            t.gameObject.transform.parent = parentTransform;
            t.gameObject.SetActive(false);
        }
    }

    public T Pull()
    {
        T t;
        if (pooledCount > 0)
        {
            t = pooledObjects.Pop();
        }
        else
        {
            Debug.LogError($"{parentTransform?.name} Exceeding Object Pool Size");
            t = GameObject.Instantiate(prefabToPool).GetComponent<T>();
        }
        t.gameObject.SetActive(true);
        t.Initialise(Push);
        return t;
    }

    public GameObject PullGameObject()
    {
        return Pull().gameObject;
    }

    public void Push(T t)
    {
        pooledObjects.Push(t);

         t.gameObject.SetActive(false);
    }

    public void Clear()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            GameObject pooledObject = pooledObjects.Pop().gameObject;
            if (!pooledObject.activeInHierarchy)
            {
                GameObject.Destroy(pooledObject.gameObject);
            }
        }
    }
}
