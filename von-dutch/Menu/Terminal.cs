using Spectre.Console;

namespace von_dutch.Menu
{
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
            new ExitTask()
        ];

        private readonly AppContext _context = new();

        public void Run()
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
    }
}
