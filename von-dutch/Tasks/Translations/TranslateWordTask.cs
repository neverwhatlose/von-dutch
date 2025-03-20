using System.Text.Json;
using Spectre.Console;
using System.Globalization;
using von_dutch.Managers;
using von_dutch.Menu;
using von_dutch.Tasks.SubTasks;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Translations
{
    /// <summary>
    /// Класс, представляющий задачу перевода слова с использованием выбранного словаря.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class TranslateWordTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Перевод слова";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        private CultureInfo? _recognizerCulture;

        /// <summary>
        /// Выполняет задачу перевода слова с использованием выбранного словаря.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если контекст или выбранный словарь равны null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Может возникнуть при работе с голосовым вводом или озвучиванием текста.
        /// </exception>
        public override void Execute(AppContext context)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));

            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            if (selectedDict == null)
            {
                return;
            }
            
            _recognizerCulture = DetermineCultureByDictionary(selectedDict, context);
            if (_recognizerCulture == null)
            {
                TerminalUi.DisplayMessage("Не удалось определить язык распознавания для выбранного словаря. Использую по умолчанию en-US.", Color.Yellow);
                _recognizerCulture = new CultureInfo("en-US");
            }
            
            bool useVoiceForWord = AnsiConsole.Confirm("[grey]Использовать голосовой ввод слова?[/]");
            string? word;
            if (useVoiceForWord)
            {
                word = VoiceManager.RecognizeSpeech("Скажите слово для перевода...", _recognizerCulture);
                if (string.IsNullOrWhiteSpace(word))
                {
                    TerminalUi.DisplayMessageWaiting("Слово не распознано. Операция отменена.", Color.Red);
                    return;
                }
            }
            else
            {
                word = TerminalUi.PromptText("[green]Введите слово для перевода:[/]");
                if (string.IsNullOrWhiteSpace(word))
                {
                    TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                    return;
                }
            }
            
            if (selectedDict.TryGetValue(word, out object? translation))
            {
                string translationText;

                switch (translation)
                {
                    case string value:
                        translationText = value;
                        TerminalUi.DisplayMessage($"Перевод слова: {word} - {value}", Color.Green);
                        break;

                    case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                        {
                            List<string> translations = [];
                            foreach (string? tr in jsonElement.EnumerateArray().Select(element => element.GetString()).Where(tr => !string.IsNullOrEmpty(tr)))
                            {
                                translations.Add(tr!);
                                TerminalUi.DisplayMessage($"Перевод слова: {word} - {tr}", Color.Green);
                            }
                            translationText = string.Join(", ", translations);
                            break;
                        }

                    case JsonElement element1:
                        translationText = element1.GetString() ?? "Ошибка перевода";
                        TerminalUi.DisplayMessage($"Перевод слова: {word} - {translationText}", Color.Green);
                        break;

                    default:
                        TerminalUi.DisplayMessageWaiting("Неподдерживаемый формат перевода", Color.Red);
                        return;
                }
                
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    TerminalUiExtensions.GetTranslationAsString(translation),
                    "✅"
                );
                
                bool speakTranslation = AnsiConsole.Confirm("[grey]Озвучить перевод?[/]");
                if (speakTranslation)
                {
                    VoiceManager.SpeakText($"Перевод слова {word}: {translationText}");
                }
                
                bool changeTranslation = AnsiConsole.Confirm("[grey]Считаете ли вы нужным отредактировать перевод?[/]");
                if (!changeTranslation)
                {
                    return;
                }

                EditTranslationSubTask editTranslationSubTask = new(word, selectedDict);
                editTranslationSubTask.Execute(context);
            }
            else
            {
                TerminalUi.DisplayMessage($"Слово {word} не найдено в словаре", Color.Red);
                
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    string.Empty,
                    "❌"
                );
                
                bool addWord = AnsiConsole.Confirm("[grey]Хотите добавить это слово в словарь?[/]");
                if (!addWord)
                {
                    return;
                }

                AddWordSubTask addWordSubTask = new(word, selectedDict);
                addWordSubTask.Execute(context);
            }
        }
        
        /// <summary>
        /// Определяет культуру распознавания речи на основе имени словаря.
        /// </summary>
        /// <param name="dict">Выбранный словарь.</param>
        /// <param name="context">Контекст приложения.</param>
        /// <returns>Объект CultureInfo, соответствующий языку словаря, или null, если язык не определен.</returns>
        private CultureInfo? DetermineCultureByDictionary(Dictionary<string, object> dict, AppContext context)
        {
            string dictName = GetDictName(dict, context).ToLower();

            if (dictName.Contains("en-ru"))
            {
                return new CultureInfo("en-US");
            }
            
            if (dictName.Contains("fr-ru"))
            {
                return new CultureInfo("fr-FR");
            }
            
            return dictName.Contains("es-en") ? new CultureInfo("es-ES") : null;
        }
    }
}