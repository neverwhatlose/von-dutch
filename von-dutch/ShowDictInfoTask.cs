using System.Text;
using System.Text.Json;
using Spectre.Console;

namespace von_dutch
{
    public class ShowDictInfoTask : TaskCore
    {
        public override string Title { get; } = "Вывести информацию о словарях";
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
                    case JsonElement value:
                        {
                            if (value.ValueKind == JsonValueKind.Array)
                            {
                                int count = value.EnumerateArray().Count();
                                if (count > 1)
                                {
                                    multipleTranslations++;
                                }
                                else
                                {
                                    singleTranslation++;
                                }
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
            
            Table infoTable = new() { Border = TableBorder.Rounded };
            infoTable.AddColumn("[green]Показатель[/]");
            infoTable.AddColumn("[green]Значение[/]");
            infoTable.AddRow("Всего слов", totalWords.ToString());
            infoTable.AddRow("С единственным переводом", singleTranslation.ToString());
            infoTable.AddRow("Со множественными переводами", multipleTranslations.ToString());
            AnsiConsole.Write(infoTable);
            
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
            
            const int pageSize = 30;
            int currentPage = 0;
            int totalPages = (entries.Count + pageSize - 1) / pageSize;
            bool exit = false;
            
            while (!exit)
            {
                Console.Clear();
            
                string header = $"[green]Словарь[/] Страница {currentPage + 1} из {totalPages}";
                AnsiConsole.MarkupLine($"[grey]{header}[/]");

                Table pageTable = new () { Border = TableBorder.Rounded };
                pageTable.AddColumn("[green]Слово[/]");
                pageTable.AddColumn("[green]Перевод[/]");

                int startIndex = currentPage * pageSize;
                int endIndex = Math.Min(startIndex + pageSize, entries.Count);
                for (int i = startIndex; i < endIndex; i++)
                {
                    KeyValuePair<string, object> kvp = entries[i];
                    string translationStr = GetTranslationAsString(kvp.Value);
                    pageTable.AddRow(kvp.Key, translationStr);
                }
                
                AnsiConsole.Write(pageTable);
                
                const string footer = "[grey]Стрелка влево/вправо для смены страницы, Esc для выхода из просмотра[/]";
                AnsiConsole.MarkupLine(footer);
                
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentPage > 0)
                            {
                                currentPage--;
                            }

                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentPage < totalPages - 1)
                            {
                                currentPage++;
                            }

                            break;
                        }
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        continue;
                }
            }
        }
        
        private static string GetTranslationAsString(object value)
        {
            string result;
            switch (value)
            {
                case string s:
                    result = s;
                    break;
                case JsonElement jsonElement:
                    {
                        if (jsonElement.ValueKind == JsonValueKind.Array)
                        {
                            StringBuilder sb = new ();
                            foreach (JsonElement item in jsonElement.EnumerateArray())
                            {
                                sb.Append(item.GetString());
                                sb.Append(", ");
                            }
                            if (sb.Length > 2)
                            {
                                sb.Remove(sb.Length - 2, 2);
                            }
                            result = sb.ToString();
                        }
                        else
                        {
                            result = jsonElement.GetString()!;
                        }

                        break;
                    }
                default:
                    result = value.ToString()!;
                    break;
            }
            return result;
        }
    }
}
