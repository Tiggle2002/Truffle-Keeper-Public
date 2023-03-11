using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Extends World Bounds On Death 
/// </summary>
[RequireComponent(typeof(Health))]
public class LevelObstacle : MonoBehaviour, ILevelExtender
{
    [SerializeField] private float extensionLength;
    [field: SerializeField] public ExtensionDirection direction { get; set; }

    public void OnEnable()
    {
        GetComponent<Health>().AddListener(ExtendOnDeath);
    }

    public void OnDisable()
    {
        GetComponent<Health>().RemoveListener(ExtendOnDeath);
    }

    public void ExtendOnDeath(int healthPercentage)
    {
        if (healthPercentage ==0)
        {
            Extend();
        }
    }    

    public void Extend()
    {
        LevelBounder.Instance?.SmoothlyIncreaseWorldBounds(this);
    }

    public float ExtensionPointX()
    {
        if (direction == ExtensionDirection.Right)
        {
            return transform.position.x + extensionLength;
        }
        else
        {
            return transform.position.x - extensionLength;
        }
    }

    public void OnDrawGizmos()
    {
        Vector3 endPos;
        if (direction == ExtensionDirection.Left)
        {
            endPos = new(transform.position.x - extensionLength, transform.position.y);
        }
        else
        {
            endPos = new(transform.position.x + extensionLength, transform.position.y);
        }
        Debug.DrawLine(transform.position, endPos);
    }
}

public enum ExtensionDirection { Left, Right }
