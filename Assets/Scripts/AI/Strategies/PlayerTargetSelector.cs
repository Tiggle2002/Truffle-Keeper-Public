using SurvivalElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetSelector : AITargetSelector
{
    Transform playerTransform;

    public void Start()
    {
        playerTransform = PlayerManager.Instance.transform;
        SelectTarget();
    }

    public override void SelectTarget()
    {
        target = playerTransform.position;
    }
}
