using Spectre.Console;
using System.Text;
using System.Text.Json;

namespace von_dutch
{
/*
 * Запрос папки
 * Ищем файлы en-ru.json, es-en.json and fr-ru.json
 * Если файла нет, говорим пользователю, что такой-то словарь недоступен (ну то есть
 * в выборе словаря он не увидит его)
 * если файлов нет в целом, говорим, что словари не найдены и предлагаем запустить заново
 * Не этот класс но другой должен проверять соответствие файлов формату
 * Проверять то действительно ли в файлах словари не будем
 */
    public class LoadDictTask : TaskCore
    {
        public override bool NeedsData => false;
        public override string Title => "Выбор текущей языковой пары";

        public override void Execute(AppContext context)
        {
            Console.Clear();
            
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));
            AnsiConsole.MarkupLine("[grey]Введите путь к [green]папке[/] со словарями (нажмите [red]Esc[/] для отмены)[/]");
            AnsiConsole.MarkupLine("[grey]Обратите внимание, что в папка должна содержать хотя бы [yellow]1 файл из списка[/]: [green]en-ru.json[/], [green]es-en.json[/] и [green]fr-ru.json[/].[/]: ");
            
            StringBuilder input = new();
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length == 0)
                    {
                        continue;
                    }
                    // удаляю последний символ
                    input.Remove(input.Length - 1, 1);
                    // и из консоли
                    Console.Write("\b \b");
                }
                else
                {
                    input.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                }
            }
    
            string dataPath = input.ToString();
            
            if (dataPath.Length == 0)
            {
                AnsiConsole.MarkupLine("[grey][red]Ошибка:[/] Путь не может быть пустым.[/]");
                return;
            }
            
            try
            {
                DirectoryInfo dir = new(dataPath);
                if (!dir.Exists)
                {
                    AnsiConsole.MarkupLine("[grey][red]Ошибка:[/] Указанная папка не существует.[/]");
                    return;
                }

                if (!dir.GetFiles().Any(file => file.Name is "en-ru.json" or "es-en.json" or "fr-ru.json"))
                {
                    AnsiConsole.MarkupLine("[grey][red]Ошибка:[/] Нет ни одного доступного словаря.[/]");
                    return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[grey][red]Ошибка:[/] {ex.Message}[/]");
                return;
            }

            context.DataPath = dataPath;
            
            DataController.LoadData(context);
            
            if (context.EngRusDict is not null || context.EspEngDict is not null || context.FreRusDict is not null)
            {
                AnsiConsole.MarkupLine("[green]Словари успешно загружены[/]");
                context.IsDataLoaded = true;
                return;
            }

            AnsiConsole.MarkupLine("[grey][red]Ошибка:[/] При загрузке словарей произошла ошибка.[/]");
        }
    }
}
