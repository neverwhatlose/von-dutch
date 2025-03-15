using Spectre.Console;

namespace von_dutch;

public class LoadDictTask : TaskCore
{
    public override void Execute()
    {
        Console.WriteLine("executed");
        var dataPath = AnsiConsole.Prompt(
            new TextPrompt<string>("Введите путь к файлу словаря")
                .PromptStyle("grey")
                .DefaultValue("locale-en-us.json")
        );
    }
}