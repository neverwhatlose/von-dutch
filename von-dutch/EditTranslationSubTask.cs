using Spectre.Console;

namespace von_dutch
{
    public class EditTranslationSubTask(string originalWord, Dictionary<string, object> selectedDict)
    {
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