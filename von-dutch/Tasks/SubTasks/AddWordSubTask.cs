using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.SubTasks
{
    /// <summary>
    /// Класс, представляющий подзадачу добавления перевода для заданного слова в выбранный словарь.
    /// </summary>
    public class AddWordSubTask(string originalWord, Dictionary<string, object> selectedDict)
    {
        /// <summary>
        /// Выполняет подзадачу добавления перевода для заданного слова.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если контекст или выбранный словарь равны null.
        /// </exception>
        public void Execute(AppContext context)
        {
            string? translation = TerminalUi.PromptText("Введите перевод для " + originalWord + ":");
            
            if (translation == null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (translation.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Перевод не может быть пустым", Color.Red);
                return;
            }

            selectedDict[originalWord] = translation;
            TerminalUi.DisplayMessageWaiting("Слово успешно добавлено!", Color.Green);

            DataController.UpdateData(context);
        }
    }
}