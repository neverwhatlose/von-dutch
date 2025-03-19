using Spectre.Console;
using System.Text.Json;

namespace von_dutch
{
    public class TranslateWordTask : TaskCore
    {
        public override string Title { get; } = "Перевод слова";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            Console.Clear();
            
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));

            Dictionary<string, object> selectedDict = SelectDictionary(context);

            string word = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Введите слово[/]")
                    .Validate(word => word.Length == 0 || string.IsNullOrWhiteSpace(word) ? ValidationResult.Error("[red]Слово не может быть пустым[/]") : ValidationResult.Success()));
            
            
            // Нужно проверить чем является translation
            // 1. если строка, то просто вывести
            // 2. если массив, то вывести все элементы
            if (selectedDict.TryGetValue(word, out object? translation))
            {
                switch (translation)
                {
                    case string ans:
                        AnsiConsole.MarkupLine($"[green]Перевод слова[/]: {word} - {ans}");
                        break;
                    case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                        {
                            foreach (JsonElement element in jsonElement.EnumerateArray())
                            {
                                AnsiConsole.MarkupLine($"[green]Перевод слова[/]: {word} - {element.GetString()}");
                            }

                            break;
                        }
                    case JsonElement jsonElement:
                        AnsiConsole.MarkupLine($"[green]Перевод слова[/]: {word} - {jsonElement.GetString()}");
                        break;
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Слово[/] {word} [red]не найдено в словаре[/]");
            }
        }
    }
}