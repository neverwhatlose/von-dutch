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

            Panel menuPanel = new ("Выберите команду")
            {
                Border = BoxBorder.Double,
                Padding = new Padding(1, 1)
            };
            AnsiConsole.Write(menuPanel);

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
        
        public static void DisplayMessage(string message, Color messageColor)
        {
            AnsiConsole.MarkupLine($"[{messageColor.ToString().ToLower()}]{message}[/]");
            Console.ReadKey();
        }
    }
}
