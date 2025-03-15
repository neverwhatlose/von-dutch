using Spectre.Console;

namespace von_dutch.Menu;

public class Terminal
{
    private readonly Dictionary<string, TaskCore> _handlers = new()
    {
        { "Выбор текущей языковой пары", new LoadDictTask() },
        { "Перевод слова", new LoadDictTask() },
        { "Добавить новое слово", new LoadDictTask() },
        { "Редактирование слова в словаре", new LoadDictTask() },
        { "Удаление слова", new LoadDictTask() },
        { "Сохранить изменения", new LoadDictTask() },
        { "Завершить", new LoadDictTask() }
    };
    
    public void Run()
    {
        while (true)
        {
            Console.Clear();
            
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<KeyValuePair<string, TaskCore>>()
                    .Title("Добро пожаловать в словарь [green]Von Dutch![/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Нажимайте 🔼 и 🔽, чтобы открыть больше команд)[/]")
                    .AddChoices(_handlers)
                    .UseConverter(kvp => kvp.Key)
                );

            choice.Value.Execute();
            Console.ReadKey();
        }
    }
}