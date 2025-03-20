using Spectre.Console;
using System.Globalization;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using von_dutch.Menu;

namespace von_dutch.Managers
{
    /// <summary>
    /// Управляет функциями распознавания и синтеза речи.
    /// </summary>
    public static class VoiceManager
    {
        /// <summary>
        /// Распознает речь пользователя с использованием микрофона.
        /// </summary>
        /// <param name="prompt">Сообщение, которое будет отображено пользователю перед началом распознавания.</param>
        /// <param name="culture">Культура (язык), используемая для распознавания речи.</param>
        /// <returns>Распознанный текст или пустую строку, если распознавание не удалось.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если не удалось настроить аудиовход.</exception>
        /// <exception cref="Exception">Выбрасывается, если произошла ошибка при распознавании речи.</exception>
        public static string RecognizeSpeech(string prompt, CultureInfo culture)
        {
            TerminalUi.DisplayMessage(prompt, Color.Yellow);

            string recognizedText = "";
            try
            {
                using SpeechRecognitionEngine recognizer = new(culture);
                recognizer.SetInputToDefaultAudioDevice();
                
                recognizer.LoadGrammar(new DictationGrammar());
                
                recognizer.SpeechRecognized += (sender, e) =>
                {
                    recognizedText = e.Result.Text.ToLower();
                };
                
                recognizer.RecognizeAsync(RecognizeMode.Single);
                Thread.Sleep(5000);
                recognizer.RecognizeAsyncStop();
            }
            catch (Exception ex)
            {
                TerminalUi.DisplayMessage($"Ошибка распознавания: {ex.Message}", Color.Red);
            }

            if (!string.IsNullOrWhiteSpace(recognizedText))
            {
                TerminalUi.DisplayMessage($"Распознано: {recognizedText}", Color.Green);
            }
            return recognizedText.Trim();
        }
        
        /// <summary>
        /// Синтезирует текст в речь и воспроизводит её.
        /// </summary>
        /// <param name="textToSpeak">Текст, который нужно преобразовать в речь.</param>
        /// <exception cref="Exception">Выбрасывается, если произошла ошибка при синтезе речи.</exception>
        public static void SpeakText(string textToSpeak)
        {
            try
            {
                using SpeechSynthesizer synth = new();
                synth.Volume = 100;
                synth.Rate = 0;
                synth.Speak(textToSpeak);
            }
            catch (Exception ex)
            {
                TerminalUi.DisplayMessage($"Ошибка синтеза речи: {ex.Message}", Color.Red);
            }
        }
    }
}