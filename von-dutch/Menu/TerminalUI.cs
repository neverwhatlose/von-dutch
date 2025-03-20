using System.Text;
using Spectre.Console;

namespace von_dutch
{
    public static class TerminalUi
    {
        public static TaskCore ShowMainMenu(List<TaskCore> tasks)
        {
            Console.Clear();

            FigletText titleText = new FigletText("VON DUTCH")
                .LeftJustified()
                .Color(Color.Blue);
            
            AnsiConsole.Write(titleText);

            TaskCore selectedTask = AnsiConsole.Prompt(
                new SelectionPrompt<TaskCore>()
                    .Title("[grey]Главное меню[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Используйте стрелки для навигации)[/]")
                    .AddChoices(tasks)
                    .UseConverter(task => task.Title)
            );

            return selectedTask;
        }

        public static string? PromptText(string promptText)
        {
            AnsiConsole.MarkupLine("[grey]" + promptText + "[/]");
            StringBuilder input = new();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return null;
                }

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length <= 0)
                    {
                        continue;
                    }

                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else
                {
                    input.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                }
            }

            return input.ToString();
        }

        public static void DisplayMessageWaiting(string message, Color messageColor)
        {
            AnsiConsole.MarkupLine($"[{messageColor.ToString().ToLower()}]{message}[/]");
            Console.ReadKey();
        }

        public static void DisplayMessage(string message, Color messageColor)
        {
            AnsiConsole.MarkupLine($"[{messageColor.ToString().ToLower()}]{message}[/]");
        }

        /// <summary>
        /// Универсальный метод для ленивого создания и вывода таблицы.
        /// </summary>
        public static void PrintTable(string tableTitle, List<TableColumn> columns, List<List<string>> rows)
        {
            Table table = new()
            {
                Title = new TableTitle(tableTitle, new Style(foreground: Color.Green)), Border = TableBorder.Rounded
            };

            foreach (TableColumn column in columns)
            {
                table.AddColumn(column);
            }

            foreach (List<string> row in rows)
            {
                table.AddRow(row.ToArray());
            }

            AnsiConsole.Write(table);
        }

        /// <summary>
        /// Универсальный метод для вывода таблицы с пагинацией.
        /// Принимает заголовок таблицы, список названий колонок, все строки таблицы и размер страницы.
        /// Пользователь может переключаться между страницами с помощью стрелок и выйти нажатием Esc.
        /// </summary>
        public static void PrintPaginatedTable(string tableTitle, List<TableColumn> columns, List<List<string>> rows,
            int pageSize)
        {
            int currentPage = 0;
            int totalRows = rows.Count;
            int totalPages = (totalRows + pageSize - 1) / pageSize;
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                string header = $"[grey]{tableTitle} - Страница {currentPage + 1} из {totalPages}[/]";
                AnsiConsole.MarkupLine(header);

                int startIndex = currentPage * pageSize;
                int rowCount = totalRows - startIndex;
                int countOnPage = rowCount > pageSize ? pageSize : rowCount;
                List<List<string>> pageRows = rows.GetRange(startIndex, countOnPage);

                PrintTable(tableTitle, columns, pageRows);
                AnsiConsole.MarkupLine("[grey]Стрелка влево/вправо для смены страницы, Esc для выхода[/]");

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
    }
}