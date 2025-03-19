using Spectre.Console;

namespace von_dutch
{
    public class ExitTask : TaskCore
    {
        public override string Title { get; } = "Завершить работу и сохранить данные";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            AnsiConsole.MarkupLine("[green]Сохранение данных...[/]");
            
            // апдейтим на случай если я забыл что-то заапдейтить
            DataController.UpdateData(context);
            
            DataController.SaveData(context);
            
            AnsiConsole.MarkupLine("[green]Данные успешно сохранены![/]");

            Environment.Exit(0);
        }
    }
}