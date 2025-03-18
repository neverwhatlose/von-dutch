namespace von_dutch
{
    public abstract class TaskCore
    {
        public virtual bool NeedsData => false;
        
        public abstract string Title { get; }
        
        public abstract void Execute(AppContext context);
    }
}