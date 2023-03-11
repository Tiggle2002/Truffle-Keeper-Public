using Sirenix.OdinInspector;
using UnityEngine;

public class IdleAction : AIAction
{
    #region Variables
    [SerializeField, FoldoutGroup("Action Settings"), ShowIf("timed"), MinMaxSlider(0.25f, 20f, true)]
    protected Vector2 randomTimeLength;
    [SerializeField, FoldoutGroup("Action Settings")]
    protected bool haltGravity;
    [SerializeField, ShowIf("playAnimation")]
    private string animationName;
    #endregion

    #region Boolean Methods
    public override bool Executable() => true;

    public override bool Over() => TimerDone() || targetSelection.TargetAvailable() == false;
    #endregion

    #region Action Methods
    public override void StartAction()
    {
        base.StartAction();
        actionTimer.SetTimerLength(randomTimeLength.RandomInRange());
        actionTimer.ResetCountdown();
        SuspendMovement();
        FSM.PlayAnimation(animationName);
    }

    public override void EndAction()
    {
        base.EndAction();
        if (haltGravity)
        {
            AIMovement.UpdateGravityScale(1f);
        }
    }
    #endregion

    private void SuspendMovement()
    {
        AIMovement.rb.HaltMovementImmediate();
        if (haltGravity)
        {
            AIMovement.UpdateGravityScale(0f);
        }
    }
}
