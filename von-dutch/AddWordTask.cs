using Spectre.Console;

namespace von_dutch
{
    public class AddWordTask : TaskCore
    {
        public override string Title { get; } = "Добавить новое слово";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            Dictionary<string, object> selectedDict = SelectDictionary(context);
            
            string word = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Введите слово[/]")
                    .Validate(word => word.Length == 0 || string.IsNullOrWhiteSpace(word) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));
            
            string translation = AnsiConsole.Prompt(
                new TextPrompt<string>($"[green]Введите перевод для {word}[/]")
                    .Validate(translation => translation.Length == 0 || string.IsNullOrWhiteSpace(word) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));
            
            selectedDict[word] = translation;
            AnsiConsole.MarkupLine("[green]Слово успешно добавлено![/]");
            
            DataController.UpdateData(context);
        }
    }
}