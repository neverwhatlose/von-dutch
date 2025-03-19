namespace von_dutch
{
    public class ShowDictInfoTask : TaskCore
    {
        public override string Title { get; } = "Вывести информацию о словарях";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            
        }
    }
}