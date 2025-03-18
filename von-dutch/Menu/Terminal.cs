using Spectre.Console;

namespace von_dutch.Menu
{

/*
 * про временный стек изменений
 * я думаю есть смысл копировать файлы словарей во временную папку
 * и изменять их там, а затем просто перезаписывать словари
 */
    public class Terminal
    {
        // private readonly Dictionary<string, TaskCore> _handlers = new()
        // {
        //     { "Выбор текущей языковой пары", new LoadDictTask() },
        //     { "Перевод слова", new LoadDictTask() },
        //     { "Добавить новое слово", new LoadDictTask() },
        //     { "Редактирование слова в словаре", new LoadDictTask() },
        //     { "Удаление слова", new LoadDictTask() },
        //     { "Сохранить изменения", new LoadDictTask() },
        //     { "Завершить", new LoadDictTask() }
        // };

        private readonly List<TaskCore> _handlers = [
            new LoadDictTask(),
            new TranslateWordTask(),
            new LoadDictTask(),
            new LoadDictTask(),
            new LoadDictTask(),
            new LoadDictTask(),
            new LoadDictTask()
        ];
        
        private readonly AppContext _context = new();

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));

                TaskCore choice = AnsiConsole.Prompt(
                    new SelectionPrompt<TaskCore>()
                        .Title("Добро пожаловать в словарь [green]Von Dutch![/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Grey))
                        .MoreChoicesText("[grey](Нажимайте 🔼 и 🔽, чтобы открыть больше команд)[/]")
                        .AddChoices(_handlers)
                        .UseConverter(task => task.Title)
                );
                
                if (choice.NeedsData && !_context.IsDataLoaded)
                {
                    AnsiConsole.MarkupLine("[red]Для выполнения этой команды необходимо загрузить данные[/]");
                    Console.ReadKey();
                    continue;
                }
                choice.Execute(_context);
                Console.ReadKey();
            }
        }
    }
}
