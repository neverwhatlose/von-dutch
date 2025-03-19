using Spectre.Console;

namespace von_dutch.Menu
{

/*
 * про временный стек изменений
 * я думаю есть смысл копировать файлы словарей во временную папку
 * и изменять их там, а затем просто перезаписывать словари
 */
    public class Terminal
    {
        private readonly List<TaskCore> _handlers = [
            new LoadDictTask(),
            new TranslateWordTask(),
            new AddWordTask(),
            new EditWordTask(),
            new ShowDictInfoTask(),
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
                    TerminalUi.DisplayMessage("Для выполнения этой операции необходимо загрузить словарь.", Color.Red);
                    continue;
                }
                
                selectedTask.Execute(_context);
            }
        }
    }
}
