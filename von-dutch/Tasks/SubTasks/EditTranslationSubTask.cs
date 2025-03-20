using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.SubTasks
{
    /// <summary>
    /// Класс, представляющий подзадачу редактирования перевода для заданного слова в выбранном словаре.
    /// </summary>
    public class EditTranslationSubTask(string originalWord, Dictionary<string, object> selectedDict)
    {
        /// <summary>
        /// Выполняет подзадачу редактирования перевода для заданного слова.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если контекст или выбранный словарь равны null.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Может возникнуть, если слово отсутствует в выбранном словаре.
        /// </exception>
        public void Execute(AppContext context)
        {
            TerminalUi.DisplayMessage("Текущий перевод: " + selectedDict[originalWord], Color.Yellow);
            string? newTranslation = TerminalUi.PromptText("Введите новый перевод для " + originalWord + ":");

            if (newTranslation == null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (newTranslation.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                return;
            }
            
            selectedDict[originalWord] = newTranslation;
            TerminalUi.DisplayMessageWaiting("Перевод успешно изменен!", Color.Green);
            
            DataController.UpdateData(context);
        }
    }
}