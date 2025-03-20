using Spectre.Console;
using System.Globalization;
using von_dutch.Managers;
using von_dutch.Menu;
using von_dutch.Tasks.SubTasks;
using Color = Spectre.Console.Color;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Translations
{
    /// <summary>
    /// Класс, представляющий задачу контекстного перевода слова в фразе или предложении.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class ContextualTranslationTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Контекстный перевод слова в фразе или предложении";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        /// <summary>
        /// Выполняет задачу контекстного перевода слова в фразе или предложении.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.NullReferenceException">
        /// Может возникнуть, если словарь en-ru.json не загружен или AI-сервис не инициализирован.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Может возникнуть при работе с голосовым вводом или озвучиванием текста.
        /// </exception>
        public override void Execute(AppContext context)
        {
            if (context.EngRusDict is null)
            {
                TerminalUi.DisplayMessageWaiting("Словарь en-ru.json не загружен. Возврат в главное меню.", Color.Red);
                return;
            }

            GigaChatAiService aiService = GigaChatAiService.Instance;
            
            bool useVoiceSentence = AnsiConsole.Confirm("[grey]Использовать голосовой ввод фразы или предложения?[/]");
            string sentence;
            if (useVoiceSentence)
            {
                sentence = VoiceManager.RecognizeSpeech("Скажите фразу или предложение на английском:", new CultureInfo("en-US"));
                if (string.IsNullOrWhiteSpace(sentence))
                {
                    TerminalUi.DisplayMessageWaiting("Предложение не распознано. Возврат в главное меню.", Color.Red);
                    return;
                }
            }
            else
            {
                string? inputSentence = TerminalUi.PromptText("Введите [yellow]фразу[/] или [yellow]предложение[/]:");
                if (inputSentence == null)
                {
                    TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                    return;
                }
                sentence = inputSentence.Trim();
                if (string.IsNullOrWhiteSpace(sentence))
                {
                    TerminalUi.DisplayMessageWaiting("Предложение не может быть пустым", Color.Red);
                    return;
                }
            }
            
            bool useVoiceWord = AnsiConsole.Confirm("[grey]Использовать голосовой ввод слова?[/]");
            string word;
            if (useVoiceWord)
            {
                word = VoiceManager.RecognizeSpeech("Скажите слово, которое хотите перевести:",  new CultureInfo("en-US"));
                if (string.IsNullOrWhiteSpace(word))
                {
                    TerminalUi.DisplayMessageWaiting("Слово не распознано. Возврат в главное меню.", Color.Red);
                    return;
                }
            }
            else
            {
                string? inputWord = TerminalUi.PromptText("Введите [yellow]слово[/]:");
                if (inputWord == null)
                {
                    TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                    return;
                }
                word = inputWord.Trim();
                if (string.IsNullOrWhiteSpace(word))
                {
                    TerminalUi.DisplayMessageWaiting("Слово не может быть пустым", Color.Red);
                    return;
                }
            }
            
            string translation = aiService.TranslateWordWithContext(sentence, word).GetAwaiter().GetResult();

            HistoryManager.Log(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "en-ru.json",
                word,
                TerminalUiExtensions.GetTranslationAsString(translation),
                "✅"
            );
            
            if (!string.IsNullOrWhiteSpace(translation))
            {
                TerminalUi.DisplayMessage($"[green]Слово может быть переведено так[/]: {translation}", Color.Grey);
                bool speakIt = AnsiConsole.Confirm("[grey]Озвучить перевод?[/]");
                if (speakIt && !string.IsNullOrWhiteSpace(translation))
                {
                    VoiceManager.SpeakText(translation);
                }
                
                if (context.EngRusDict!.ContainsKey(word))
                {
                    Console.ReadKey();
                    return;
                }
                
                bool changeTranslation = AnsiConsole.Prompt(
                    new SelectionPrompt<bool>()
                        .Title("[grey]Хотите добавить слово в словарь?[/]")
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Используйте стрелки для выбора)[/]")
                        .AddChoices(true, false)
                        .UseConverter(value => value ? "Да" : "Нет")
                );
                
                if (!changeTranslation)
                {
                    return;
                }

                AddWordSubTask addWordSubTask = new(word, context.EngRusDict!);
                addWordSubTask.Execute(context);
            }

            Console.ReadKey();
        }
    }
}