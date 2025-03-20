using System;
using System.Collections.Generic;
using System.Speech.Recognition;   
using System.Speech.Synthesis;     
using System.Text.Json;
using Spectre.Console;

namespace von_dutch
{
    public class TranslateWordTask : TaskCore
    {
        public override string Title { get; } = "Перевод слова";
        public override bool NeedsData { get; } = true;

        public override void Execute(AppContext context)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));

            Dictionary<string, object>? selectedDict = SelectDictionary(context);
            if (selectedDict == null)
            {
                return;
            }

            // Выбор способа ввода слова (голосом или текстом)
            bool useVoiceForWord = AnsiConsole.Confirm("[grey]Использовать голосовой ввод слова?[/]");
            string? word;
            if (useVoiceForWord)
            {
                word = RecognizeSpeech("Скажите слово для перевода...");
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
                string translationText = "";

                switch (translation)
                {
                    case string value:
                        translationText = value;
                        TerminalUi.DisplayMessage($"Перевод слова: {word} - {value}", Color.Green);
                        break;

                    case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                        {
                            List<string> translations = new();
                            foreach (JsonElement element in jsonElement.EnumerateArray())
                            {
                                string? tr = element.GetString();
                                if (!string.IsNullOrEmpty(tr))
                                {
                                    translations.Add(tr);
                                    TerminalUi.DisplayMessage($"Перевод слова: {word} - {tr}", Color.Green);
                                }
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

                // Логируем успешный перевод
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    TerminalUiExtensions.GetTranslationAsString(translation),
                    "✅"
                );

                // Голосовой вывод перевода
                bool speakTranslation = AnsiConsole.Confirm("[grey]Озвучить перевод?[/]");
                if (speakTranslation)
                {
                    SpeakText($"Перевод слова {word}: {translationText}");
                }

                // Возможность редактирования перевода
                bool changeTranslation = AnsiConsole.Confirm("[grey]Считаете ли вы нужным отредактировать перевод?[/]");
                if (changeTranslation)
                {
                    EditTranslationSubTask editTranslationSubTask = new(word, selectedDict);
                    editTranslationSubTask.Execute(context);
                }
            }
            else
            {
                TerminalUi.DisplayMessage($"Слово {word} не найдено в словаре", Color.Red);

                // Логируем неудачный поиск
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetDictName(selectedDict, context),
                    word,
                    string.Empty,
                    "❌"
                );

                // Возможность добавить слово в словарь
                bool addWord = AnsiConsole.Confirm("[grey]Хотите добавить это слово в словарь?[/]");
                if (addWord)
                {
                    AddWordSubTask addWordSubTask = new(word, selectedDict);
                    addWordSubTask.Execute(context);
                }
            }
        }

        /// <summary>
        /// Распознавание речи через System.Speech.Recognition.
        /// </summary>
        private string RecognizeSpeech(string prompt)
        {
            TerminalUi.DisplayMessage(prompt, Color.Yellow);
            string recognizedText = "";

            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                try
                {
                    recognizer.SetInputToDefaultAudioDevice();
                    recognizer.LoadGrammar(new DictationGrammar());

                    recognizer.SpeechRecognized += (sender, e) =>
                    {
                        recognizedText = e.Result.Text;
                    };

                    recognizer.RecognizeAsync(RecognizeMode.Single);
                    System.Threading.Thread.Sleep(5000);  // Простая задержка для ожидания распознавания
                    recognizer.RecognizeAsyncStop();
                }
                catch (Exception ex)
                {
                    TerminalUi.DisplayMessage($"Ошибка распознавания: {ex.Message}", Color.Red);
                }
            }

            TerminalUi.DisplayMessage($"Распознано: {recognizedText}", Color.Green);
            return recognizedText.Trim();
        }

        /// <summary>
        /// Голосовой вывод перевода через System.Speech.Synthesis.
        /// </summary>
        private void SpeakText(string textToSpeak)
        {
            try
            {
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.Volume = 100;
                    synth.Rate = 0;
                    synth.Speak(textToSpeak);
                }
            }
            catch (Exception ex)
            {
                TerminalUi.DisplayMessage($"Ошибка синтеза речи: {ex.Message}", Color.Red);
            }
        }
    }
}
