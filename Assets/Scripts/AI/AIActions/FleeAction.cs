using Sirenix.OdinInspector;
using SurvivalElements;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FleeAction : AIAction
{
    #region Serialized Fields
    [FoldoutGroup("Movement Settings"), SerializeField]
    private bool scaredByMovement = true;
    [FoldoutGroup("Movement Settings"), SerializeField]
    private bool horizontalMovement;
    [FoldoutGroup("Movement Settings"), SerializeField]
    private bool verticalMovement;
    [FoldoutGroup("Movement Settings"), SerializeField]
    private float fleeSpeed;
    [FoldoutGroup("Movement Settings"), SerializeField]
    private bool randomDirection;

    [FoldoutGroup("Action Settings"), SerializeField, ShowIf("playAnimation")]
    private string animationName;

    private Coroutine delayCoroutine;
    private bool hasDelayed = false;

    #endregion

    public void Start()
    {
        if (scaredByMovement)
        {
            (targetSelection as ZoneTargetSelector).AddHitRequirement(MovingEnemy);
        }
    }

    public bool MovingEnemy(Collider2D collider)
    {
        return  collider.GetComponentInChildren<Rigidbody2D>().velocity != Vector2.zero;
    }

    public override bool Executable()
    {
        if (targetSelection.TargetAvailable() && delayCoroutine == null)
        {
            StartCoroutine(SetExecutableAfterDelay());
        }

        return hasDelayed;
    }

    private IEnumerator SetExecutableAfterDelay()
    {
        yield return new WaitForSeconds(0.25f);
        hasDelayed = true;
    }

    public override void StartAction()
    {
        if (FSM.Health.Alive())
        {
            base.StartAction();
        }
        actionTimer.ResetCountdown();
        if (playAnimation)
        {
            FSM.PlayAnimation(animationName);
        }
        if (randomDirection)
        {
            targetDirection = GetRandomDirection();
        }
        

        if (horizontalMovement)
        {
            AIMovement.SetHorizontalSpeed(fleeSpeed);
        }
        if (verticalMovement)
        {
            AIMovement.SetVerticalSpeed(fleeSpeed);
        }
    }

    public int GetRandomDirection()
    {
        int direction = Random.Range(-1, 2);

        return direction == 0 ? GetRandomDirection() : direction;
    }

    public override void UpdateAction()
    {
        base.UpdateAction();
        if (!randomDirection)
        {
            if (targetSelection.TargetDirectionSign() == 0) return;

            targetDirection = -targetSelection.TargetDirectionSign();

            if (targetDirection == 0) targetDirection = 1;
        }
        ResetFleeTimerIfEnemyIsNear();
    }

    private void ResetFleeTimerIfEnemyIsNear()
    {
        if (targetSelection.TargetAvailable())
        {
            actionTimer.ResetCountdown();
        }
    }

    public override void FixedUpdateAction()
    {
        AIMovement.UpdateGravityScale();

        if (horizontalMovement)
        {
            AIMovement.ApplyHorizontalVelocity(targetDirection);
        }
        if (verticalMovement)
        {
            AIMovement.ApplyVerticalVelocity(1f);
        }
        AIMovement.FlipInDirectionOfMovement();
    }

    public override bool Over()
    {
        return targetSelection.TargetDistanceX() > 50 && TimerDone();
    }

    public void OnDisable()
    {
        hasDelayed = false; 
    }
}

