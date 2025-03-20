using Spectre.Console;
using von_dutch.Tasks;
using von_dutch.Tasks.Commands;
using von_dutch.Tasks.Stats;
using von_dutch.Tasks.Translations;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Menu
{
    /// <summary>
    /// Основной класс для управления терминалом и выполнения задач.
    /// </summary>
    public class Terminal
    {
        private readonly List<TaskCore> _handlers =
        [
            new LoadDictTask(),
            new TranslateWordTask(),
            new AddWordTask(),
            new EditWordTask(),
            new AdvancedTranslationTask(),
            new ContextualTranslationTask(),
            new ShowDictInfoTask(),
            new ShowTranslationHistory(),
            new ExitTask(),
            new GiftTask()
        ];
        
        private readonly AppContext _context = new();

        /// <summary>
        /// Запускает основной цикл терминала для выполнения задач.
        /// </summary>
        /// <exception cref="Exception">Выбрасывается, если возникает неожиданная ошибка во время выполнения.</exception>
        public void Run()
        {
            try
            {
                while (true)
                {
                    TaskCore selectedTask = TerminalUi.ShowMainMenu(_handlers);
                    
                    if (selectedTask.NeedsData && !_context.IsDataLoaded)
                    {
                        TerminalUi.DisplayMessageWaiting("Для выполнения этой операции необходимо загрузить словарь.",
                            Color.Red);
                        continue;
                    }
                    
                    selectedTask.Execute(_context);
                }
            }
            catch
            {
                TerminalUi.DisplayMessage("Определенно возникла ошибка, о которой я не догадывался...", Color.Red);
            }
        }
    }
}