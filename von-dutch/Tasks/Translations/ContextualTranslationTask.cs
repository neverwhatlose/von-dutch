using Spectre.Console;

namespace von_dutch
{
    public class ContextualTranslationTask : TaskCore
    {
        public override string Title { get; } = "Контекстный перевод слова в фразе или предложении";
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
            
            string? word = TerminalUi.PromptText("Введите [yellow]слово[/]:");

            if (word is null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(word))
            {
                TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                return;
            }

            string translation = aiService.TranslateWordWithContext(sentence, word).GetAwaiter().GetResult();
            TerminalUi.DisplayMessage($"[green]Слово может быть переведено так[/]: {translation}", Color.Grey);
            
            HistoryManager.Log(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "en-ru.json",
                word,
                TerminalUiExtensions.GetTranslationAsString(translation),
                "✅"
            );

            if (context.EngRusDict!.ContainsKey(word))
            {
                Console.ReadKey();
                return;
            }
            
            bool changeTranslation = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title("[grey]Хотите добавить слово в словарь?[/]")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Используйте стрелки для выбора)[/]")
                    .AddChoices(true, false)
                    .UseConverter(value => value ? "Да" : "Нет")
            );
                
            if (!changeTranslation)
            {
                return;
            }

            EditTranslationSubTask editTranslationSubTask = new(word, context.EngRusDict!);
            editTranslationSubTask.Execute(context);
        }
    }
}