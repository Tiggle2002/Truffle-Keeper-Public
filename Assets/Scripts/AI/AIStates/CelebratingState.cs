
public class CelebratingState : NPCFSMState
{
    public CelebratingState(EnemyFSM FSM) : base(FSM)
    {
        stateID = AIStateID.Celebrate;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        FSM.PlayAnimation(Celebrating, 0);
    }

    public override void CheckForChange()
    {

    }

    public override void RunState()
    {
 
    }

    public override void FixedRunState()
    {

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

