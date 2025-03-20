using Spectre.Console;
using System.Globalization;
using von_dutch.Managers;
using von_dutch.Menu;
using Color = Spectre.Console.Color;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Translations
{
    /// <summary>
    /// Класс, представляющий задачу продвинутого перевода фраз и предложений с использованием AI-сервиса.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class AdvancedTranslationTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Продвинутый перевод фраз и предложений";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        /// <summary>
        /// Выполняет задачу продвинутого перевода фраз и предложений.
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
            
            bool useVoice = AnsiConsole.Confirm("[grey]Использовать голосовой ввод фразы?[/]");
            string sentence;
            if (useVoice)
            {
                sentence = VoiceManager.RecognizeSpeech("Скажите фразу или предложение на английском:",  new CultureInfo("en-US"));
                if (string.IsNullOrWhiteSpace(sentence))
                {
                    TerminalUi.DisplayMessageWaiting("Фраза не распознана. Возврат в главное меню.", Color.Red);
                    return;
                }
            }
            else
            {
                string? input = TerminalUi.PromptText("Введите [yellow]фразу[/] или [yellow]предложение[/]:");
                if (input == null)
                {
                    TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                    return;
                }
                sentence = input.Trim();
                if (string.IsNullOrWhiteSpace(sentence))
                {
                    TerminalUi.DisplayMessageWaiting("Предложение не может быть пустым", Color.Red);
                    return;
                }
            }
            
            string translation = aiService.TranslateSentence(sentence).GetAwaiter().GetResult();

            TerminalUi.DisplayMessage($"[green]Предложение или фраза может быть переведено так[/]: {translation}", Color.Grey);
            
            HistoryManager.Log(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "en-ru.json",
                sentence,
                TerminalUiExtensions.GetTranslationAsString(translation),
                "✅"
            );
            
            bool speakIt = AnsiConsole.Confirm("[grey]Озвучить перевод?[/]");
            if (speakIt && !string.IsNullOrWhiteSpace(translation))
            {
                VoiceManager.SpeakText(translation);
            }
        }
    }
}