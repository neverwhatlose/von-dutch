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
        private readonly List<TaskCore> _handlers = [
            new LoadDictTask(),
            new TranslateWordTask(),
            new AddWordTask(),
            new EditWordTask(),
            new ExitTask()
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
