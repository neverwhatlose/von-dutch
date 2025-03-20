using Color = Spectre.Console.Color;

namespace von_dutch
{
    public class AdvancedTranslationTask : TaskCore
    {
        public override string Title { get; } = "Продвинутый перевод фраз и предложений";
        public override bool NeedsData { get; } = false;

        public override void Execute(AppContext context)
        {
            GigaChatAiService aiService = GigaChatAiService.Instance;

            string? sentence = TerminalUi.PromptText("Введите [yellow]фразу[/] или [yellow]предложение[/]:");

            if (sentence is null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(sentence))
            {
                TerminalUi.DisplayMessageWaiting("Предложение не может быть пустым", Color.Red);
                return;
            }

            string translation = aiService.TranslateSentence(sentence).GetAwaiter().GetResult();
            TerminalUi.DisplayMessageWaiting($"[green]Предложение или фраза может быть переведено так[/]: {translation}", Color.Grey);
            
            HistoryManager.Log(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "en-ru.json",
                sentence,
                TerminalUiExtensions.GetTranslationAsString(translation),
                "✅"
            );
        }
    }
}