using System.Text;
using System.Text.Json;
using Spectre.Console;

namespace von_dutch
{
    public class ShowDictInfoTask : TaskCore
    {
        public override string Title { get; } = "Вывести информацию о словаре";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            
            if (selectedDict == null)
            {
                TerminalUi.DisplayMessage("Нет доступных словарей.", Color.Red);
                return;
            }
            
            int totalWords = 0;
            int singleTranslation = 0;
            int multipleTranslations = 0;
            
            foreach (KeyValuePair<string, object> kvp in selectedDict)
            {
                totalWords++;
                switch (kvp.Value)
                {
                    case string:
                        singleTranslation++;
                        break;
                    case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                        {
                            int count = jsonElement.EnumerateArray().Count();
                            if (count > 1)
                            {
                                multipleTranslations++;
                            }
                            else
                            {
                                singleTranslation++;
                            }

                            break;
                        }
                    default:
                        singleTranslation++;
                        break;
                }
            }
            
            List<TableColumn> infoColumns = [
                new ("[green]Показатель[/]"), 
                new ("[green]Значение[/]")
            ];
            
            List<List<string>> infoRows =
            [
                new() { "Всего слов", totalWords.ToString() },
                new() { "С единственным переводом", singleTranslation.ToString() },
                new() { "Со множественными переводами", multipleTranslations.ToString() }
            ];

            TerminalUi.PrintTable("Информация о словаре", infoColumns, infoRows);
            
            bool sortWords = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title("[grey]Отсортировать слова?[/]")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Используйте стрелки для выбора)[/]")
                    .AddChoices(true, false)
                    .UseConverter(value => value ? "Да" : "Нет")
            );

            List<KeyValuePair<string, object>> entries = new (selectedDict);
            
            if (sortWords)
            {
                entries.Sort((a, b) => string.CompareOrdinal(a.Key, b.Key));
            }
            
            List<TableColumn> wordColumns = [
                new ("[green]Слово[/]"), 
                new ("[green]Перевод[/]")
            ];
            List<List<string>> wordRows = [];
            wordRows.AddRange(entries.Select(kvp => (List<string>) [kvp.Key, TerminalUiExtensions.GetTranslationAsString(kvp.Value)]));
            
            TerminalUi.PrintPaginatedTable("Словарь", wordColumns, wordRows, 30);
        }
    }
}
