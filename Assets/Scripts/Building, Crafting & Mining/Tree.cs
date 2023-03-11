using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    #region References
    [SerializeField] MMF_Player treeFeedback;
    [SerializeField] DestructibleObject treeLog;
    private BoxCollider2D bc;
    private MMRotationShaker rotationShaker;
    private DestructibleObject health;
    #endregion

    public void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        rotationShaker = GetComponent<MMRotationShaker>();
        health = GetComponent<DestructibleObject>();
    }

    public void Start()
    {
        SetStumpEnabled(false);
    }

    private void AssessStumpEnablement(int treeLogHealth)
    {
        if (treeLogHealth == 0)
        {
            SetStumpEnabled(true);
            DetachLog();
            treeFeedback?.StopFeedbacks();
        }
    }

    private void SetStumpEnabled(bool enabled)
    {
        bc.enabled = enabled;
        rotationShaker.enabled = enabled;
        health.enabled = enabled;
    }

    private void DetachLog()
    {
        if (treeLog != null)
        {
            treeLog.transform.parent = transform.parent;
        }
    }

    public void OnEnable()
    {
        treeLog?.AddListener(AssessStumpEnablement);
        treeFeedback?.PlayFeedbacks();
    }

    public void OnDisable()
    {
        treeLog?.RemoveListener(AssessStumpEnablement);
    }
}
