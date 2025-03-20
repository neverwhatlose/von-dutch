using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Commands
{
    /// <summary>
    /// Класс, представляющий задачу завершения работы приложения и сохранения данных.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class ExitTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Завершить работу и сохранить данные";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = false;

        /// <summary>
        /// Выполняет задачу завершения работы приложения и сохранения данных.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        public override void Execute(AppContext context)
        {
            if (context.IsDataLoaded)
            {
                TerminalUi.DisplayMessage("Сохранение данных...", Color.Green);
                DataController.SaveData(context);
                TerminalUi.DisplayMessage("Данные успешно сохранены!", Color.Green);
            }
            
            TerminalUi.DisplayMessage("Прощай, еще увидимся!", Color.Green);
            Environment.Exit(0);
        }
    }
}