using Spectre.Console;

namespace von_dutch
{
    public class EditWordTask : TaskCore
    {
        public override bool NeedsData { get; } = true;
        public override string Title { get; } = "Редактирование слова";

        public override void Execute(AppContext context)
        {
            Dictionary<string, object> selectedDict = SelectDictionary(context);
            
            string wordToEdit = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Введите слово[/]")
                    .Validate(word => word.Length == 0 || string.IsNullOrWhiteSpace(word) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));

            if (!selectedDict.TryGetValue(wordToEdit, out object? value))
            {
                AnsiConsole.MarkupLine("[red]Слово не найдено в словаре[/]");
                return;
            }

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[grey]Выберите действие[/]")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Нажимайте 🔼 и 🔽, чтобы открыть больше список)[/]")
                    .AddChoices("Изменить слово", "Изменить перевод", "Удалить слово")
            );
            
            switch (choice)
            {
                case "Изменить слово":
                {
                    string newWord = AnsiConsole.Prompt(
                        new TextPrompt<string>("[green]Введите новое слово[/]")
                            .Validate(word => word.Length == 0 || string.IsNullOrWhiteSpace(word) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));
                    selectedDict[newWord] = value;
                    selectedDict.Remove(wordToEdit);
                    AnsiConsole.MarkupLine("[green]Слово успешно изменено![/]");
                    break;
                }
                case "Изменить перевод":
                {
                    string translation = AnsiConsole.Prompt(
                        new TextPrompt<string>($"[green]Введите новый перевод для {wordToEdit}[/]")
                            .Validate(translation => translation.Length == 0 || string.IsNullOrWhiteSpace(translation) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));
                    selectedDict[wordToEdit] = translation;
                    AnsiConsole.MarkupLine("[green]Перевод успешно изменен![/]");
                    break;
                }
                case "Удалить слово":
                {
                    selectedDict.Remove(wordToEdit);
                    AnsiConsole.MarkupLine("[green]Слово успешно удалено![/]");
                    break;
                }
            }
            
            DataController.UpdateData(context);
        }
    }
}