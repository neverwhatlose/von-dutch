using Spectre.Console;

namespace von_dutch
{
    public class ExitTask : TaskCore
    {
        public override string Title { get; } = "Завершить работу и сохранить данные";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            // меня бесит то что тут ожидается ввод от пользователя из-за DisplayMessage
            TerminalUi.DisplayMessage("Сохранение данных...", Color.Green);
            DataController.SaveData(context);
            TerminalUi.DisplayMessage("Данные успешно сохранены!", Color.Green);
            Environment.Exit(0);
        }
    }
}