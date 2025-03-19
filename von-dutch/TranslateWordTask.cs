using System.Text.Json;
using Spectre.Console;

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

            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            if (selectedDict == null)
            {
                return;
            }

            string? word = TerminalUi.PromptText("[green]Введите слово для перевода[/]");
            
            if (word == null)
            {
                TerminalUi.DisplayMessage("Операция отменена.", Color.Yellow);
                return;
            }
            if (word.Trim().Length == 0)
            {
                TerminalUi.DisplayMessage("Слово не может быть пустым", Color.Red);
                return;
            }

            if (selectedDict.TryGetValue(word, out object? translation))
            {
                switch (translation)
                {
                    case string value:
                        TerminalUi.DisplayMessage($"Перевод слова: {word} - " + value, Color.Green);
                        break;
                    case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                        {
                            foreach (JsonElement element in jsonElement.EnumerateArray())
                            {
                                TerminalUi.DisplayMessage($"Перевод слова: {word} - " + element.GetString(), Color.Green);
                            }

                            break;
                        }
                    case JsonElement element1:
                        TerminalUi.DisplayMessage($"Перевод слова: {word} - " + element1.GetString(), Color.Green);
                        break;
                    default:
                        TerminalUi.DisplayMessage("Неподдерживаемый формат перевода", Color.Red);
                        break;
                }
            }
            else
            {
                TerminalUi.DisplayMessage("Слово " + word + " не найдено в словаре", Color.Red);
            }
        }
    }
}
