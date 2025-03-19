using Spectre.Console;

namespace von_dutch
{
    public class EditWordTask : TaskCore
    {
        public override string Title { get; } = "Редактирование слова";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            
            if (selectedDict == null)
            {
                return;
            }

            string? wordToEdit = TerminalUi.PromptText("[green]Введите слово для редактирования[/]");
            
            if (wordToEdit == null)
            {
                TerminalUi.DisplayMessage("Операция отменена.", Color.Yellow);
                return;
            }
            
            if (wordToEdit.Trim().Length == 0)
            {
                TerminalUi.DisplayMessage("Слово не может быть пустым", Color.Red);
                return;
            }

            if (!selectedDict.ContainsKey(wordToEdit))
            {
                TerminalUi.DisplayMessage("Слово не найдено в словаре", Color.Red);
                return;
            }

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[grey]Выберите действие[/]")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Используйте стрелки для навигации)[/]")
                    .AddChoices("Изменить слово", "Изменить перевод", "Удалить слово")
            );

            switch (choice)
            {
                case "Изменить слово":
                    {
                        string? newWord = TerminalUi.PromptText("[green]Введите новое слово[/]");
                        
                        if (newWord == null)
                        {
                            TerminalUi.DisplayMessage("Операция отменена.", Color.Yellow);
                            return;
                        }
                        
                        if (newWord.Trim().Length == 0)
                        {
                            TerminalUi.DisplayMessage("Слово не может быть пустым", Color.Red);
                            return;
                        }
                        
                        object value = selectedDict[wordToEdit];
                        selectedDict[newWord] = value;
                        selectedDict.Remove(wordToEdit);
                        TerminalUi.DisplayMessage("Слово успешно изменено!", Color.Green);
                        break;
                    }
                
                case "Изменить перевод":
                    {
                        string? translation = TerminalUi.PromptText("[green]Введите новый перевод для " + wordToEdit + "[/]");
                        
                        if (translation == null)
                        {
                            TerminalUi.DisplayMessage("Операция отменена.", Color.Yellow);
                            return;
                        }
                        
                        if (translation.Trim().Length == 0)
                        {
                            TerminalUi.DisplayMessage("Перевод не может быть пустым", Color.Red);
                            return;
                        }
                        
                        selectedDict[wordToEdit] = translation;
                        TerminalUi.DisplayMessage("Перевод успешно изменен!", Color.Green);
                        break;
                    }
                case "Удалить слово":
                    {
                        selectedDict.Remove(wordToEdit);
                        TerminalUi.DisplayMessage("Слово успешно удалено!", Color.Green);
                        break;
                    }
            }

            DataController.UpdateData(context);
        }
    }
}
