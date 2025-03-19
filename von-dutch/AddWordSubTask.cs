using Spectre.Console;

namespace von_dutch
{
    public class AddWordSubTask(string originalWord, Dictionary<string, object> selectedDict)
    {
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