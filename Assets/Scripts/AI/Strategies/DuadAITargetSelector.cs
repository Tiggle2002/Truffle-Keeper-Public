using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuadAITargetSelector: AITargetSelector
{
    #region Variables
    protected Transform priorityTarget;
    protected Transform alternateTarget;

    [SerializeField] private float priorityTargetRange;
    #endregion

    public void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        priorityTarget = GameObject.FindGameObjectWithTag("Monument")?.transform;
        alternateTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void SelectTarget()
    {
        if (alternateTarget == null || priorityTarget == null)
        {
            target = priorityTarget == null ? alternateTarget.position : priorityTarget.position;
        }

        else if(!alternateTarget.gameObject.activeInHierarchy || !priorityTarget.gameObject.activeInHierarchy)
        {
            target = Vector2.zero;
        }
        else if (priorityTarget != null && Vector2.Distance(transform.position, priorityTarget.transform.position) < priorityTargetRange)
        {
            target = priorityTarget.position;
        }
        else if (alternateTarget != null)
        {
            target = alternateTarget.position;
        }
    }
}
