using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Translations
{
    /// <summary>
    /// Класс, представляющий задачу редактирования слова в словаре.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class EditWordTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Редактирование слова";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        /// <summary>
        /// Выполняет задачу редактирования слова в словаре.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если контекст или выбранный словарь равны null.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Может возникнуть, если слово не найдено в словаре.
        /// </exception>
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
                TerminalUi.DisplayMessageWaiting("Операция отменена.", Color.Yellow);
                return;
            }
            
            if (wordToEdit.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                return;
            }

            if (!selectedDict.TryGetValue(wordToEdit, out object? value1))
            {
                TerminalUi.DisplayMessageWaiting("Слово не найдено в словаре", Color.Red);
                return;
            }
            
            TerminalUi.PrintTable(string.Empty, [
                new TableColumn("[green]Слово[/]"),
                new TableColumn("[green]Перевод[/]")
            ], [
                new List<string> { wordToEdit, TerminalUiExtensions.GetTranslationAsString(value1) }
            ]);

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
                            TerminalUi.DisplayMessageWaiting("Операция отменена.", Color.Yellow);
                            return;
                        }
                        
                        if (newWord.Trim().Length == 0)
                        {
                            TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                            return;
                        }
                        
                        object value = selectedDict[wordToEdit];
                        selectedDict[newWord] = value;
                        selectedDict.Remove(wordToEdit);
                        TerminalUi.DisplayMessageWaiting("Слово успешно изменено!", Color.Green);
                        break;
                    }
                
                case "Изменить перевод":
                    {
                        string? translation = TerminalUi.PromptText("[green]Введите новый перевод для " + wordToEdit + "[/]");
                        
                        if (translation == null)
                        {
                            TerminalUi.DisplayMessageWaiting("Операция отменена.", Color.Yellow);
                            return;
                        }
                        
                        if (translation.Trim().Length == 0)
                        {
                            TerminalUi.DisplayMessageWaiting("Перевод не может быть пустым", Color.Red);
                            return;
                        }
                        
                        selectedDict[wordToEdit] = translation;
                        TerminalUi.DisplayMessageWaiting("Перевод успешно изменен!", Color.Green);
                        break;
                    }
                case "Удалить слово":
                    {
                        selectedDict.Remove(wordToEdit);
                        TerminalUi.DisplayMessageWaiting("Слово успешно удалено!", Color.Green);
                        break;
                    }
            }

            DataController.UpdateData(context);
        }
    }
}