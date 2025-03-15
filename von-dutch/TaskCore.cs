namespace von_dutch;

public abstract class TaskCore
{
    public virtual bool NeedsData => false;
    
    public abstract void Execute();
}