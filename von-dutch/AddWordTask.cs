using Spectre.Console;

namespace von_dutch
{
    public class AddWordTask : TaskCore
    {
        public override string Title { get; } = "Добавить новое слово";
        public override bool NeedsData { get; } = true;

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
                TerminalUi.DisplayMessage("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (inputWord.Trim().Length == 0)
            {
                TerminalUi.DisplayMessage("Слово не может быть пустым", Color.Red);
                return;
            }

            string? inputTranslation = TerminalUi.PromptText("[green]Введите перевод для " + inputWord + ":[/]");
            
            if (inputTranslation == null)
            {
                TerminalUi.DisplayMessage("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (inputTranslation.Trim().Length == 0)
            {
                TerminalUi.DisplayMessage("Перевод не может быть пустым", Color.Red);
                return;
            }

            selectedDict[inputWord] = inputTranslation;
            TerminalUi.DisplayMessage("Слово успешно добавлено!", Color.Green);

            DataController.UpdateData(context);
        }
    }
}