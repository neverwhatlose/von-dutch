namespace von_dutch
{
    public class ContextualTranslationTask : TaskCore
    {
        public override string Title { get; } = "Контекстный перевод слова в фразе или предложении (NIY)";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            throw new NotImplementedException();
        }
    }
}