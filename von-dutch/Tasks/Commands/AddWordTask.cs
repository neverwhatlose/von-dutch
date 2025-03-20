using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Commands
{
    /// <summary>
    /// Класс, представляющий задачу добавления нового слова в словарь приложения.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class AddWordTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Добавить новое слово";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        /// <summary>
        /// Выполняет задачу добавления нового слова в словарь.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если контекст или выбранный словарь равны null.
        /// </exception>
        public override void Execute(AppContext context)
        {
            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            
            if (selectedDict == null)
            {
                return;
            }

            string? inputWord = TerminalUi.PromptText("[green]Введите слово:[/]");
            
            if (inputWord == null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (inputWord.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                return;
            }

            string? inputTranslation = TerminalUi.PromptText("[green]Введите перевод для " + inputWord + ":[/]");
            
            if (inputTranslation == null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (inputTranslation.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Перевод не может быть пустым", Color.Red);
                return;
            }

            selectedDict[inputWord] = inputTranslation;
            TerminalUi.DisplayMessageWaiting("Слово успешно добавлено!", Color.Green);

            DataController.UpdateData(context);
        }
    }
}