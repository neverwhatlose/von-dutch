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
                TerminalUi.DisplayMessageWaiting("Операция отменена.", Color.Yellow);
                return;
            }
            if (word.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
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
                        TerminalUi.DisplayMessageWaiting("Неподдерживаемый формат перевода", Color.Red);
                        break;
                }
                
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    TerminalUiExtensions.GetTranslationAsString(translation),
                    "✅"
                    );
                
                bool changeTranslation = AnsiConsole.Prompt(
                    new SelectionPrompt<bool>()
                        .Title("[grey]Считаете ли вы нужным отредактировать перевод?[/]")
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Используйте стрелки для выбора)[/]")
                        .AddChoices(true, false)
                        .UseConverter(value => value ? "Да" : "Нет")
                );
                
                if (!changeTranslation)
                {
                    return;
                }

                EditTranslationSubTask editTranslationSubTask = new(word, selectedDict);
                editTranslationSubTask.Execute(context);
            }
            else
            {
                TerminalUi.DisplayMessage("Слово " + word + " не найдено в словаре", Color.Red);
                
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    string.Empty,
                    "❌"
                );
                
                bool changeTranslation = AnsiConsole.Prompt(
                    new SelectionPrompt<bool>()
                        .Title("[grey]Хотите добавить это слово в словарь?[/]")
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Используйте стрелки для выбора)[/]")
                        .AddChoices(true, false)
                        .UseConverter(value => value ? "Да" : "Нет")
                );

                if (!changeTranslation)
                {
                    return;
                }

                AddWordSubTask addWordSubTask = new(word, selectedDict);
                addWordSubTask.Execute(context);
            }
        }
    }
}
