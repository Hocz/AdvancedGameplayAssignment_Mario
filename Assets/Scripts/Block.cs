using Events;

public class Block : EventHandler.GameEvent
{
    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override bool IsDone()
    {
        return false;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
