namespace von_dutch
{
    public class AdvancedTranslationTask : TaskCore
    {
        public override string Title { get; } = "Продвинутый перевод фраз и предложений (NIY)";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            throw new NotImplementedException();
        }
    }
}