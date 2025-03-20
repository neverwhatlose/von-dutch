using System.Text;
using Spectre.Console;

namespace von_dutch
{
    public class LoadDictTask : TaskCore
    {
        public override bool NeedsData { get; } = false;
        public override string Title { get; } = "Выбор текущей языковой пары";
        
        private string _loadedDataPath = string.Empty;

        public override void Execute(AppContext context)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("VON DUTCH").LeftJustified().Color(Color.Blue));
            
            TerminalUi.DisplayMessage("Введите путь к [green]папке[/] со словарями (нажмите [red]Esc[/] для отмены):", Color.Grey);
            
            if (!string.IsNullOrEmpty(_loadedDataPath))
            {
                TerminalUi.DisplayMessage("Обратите внимание! Словари уже загружены из папки: " + _loadedDataPath, Color.Yellow);
            }

            string? dataPath = TerminalUi.PromptText("Папка должна содержать хотя бы один из файлов: [green]en-ru.json[/], [green]es-en.json[/], [green]fr-ru.json[/].");
            
            if (dataPath == null)
            {
                TerminalUi.DisplayMessageWaiting("Операция отменена. Возврат в главное меню.", Color.Yellow);
                return;
            }
            
            if (dataPath.Trim().Length == 0)
            {
                TerminalUi.DisplayMessageWaiting("Путь не может быть пустым", Color.Red);
                return;
            }

            try
            {
                DirectoryInfo dir = new(dataPath);
                if (!dir.Exists)
                {
                    TerminalUi.DisplayMessageWaiting("Указанная папка не существует", Color.Red);
                    return;
                }

                if (!dir.GetFiles().Any(file => file.Name is "en-ru.json" or "es-en.json" or "fr-ru.json"))
                {
                    TerminalUi.DisplayMessageWaiting("Нет ни одного доступного словаря", Color.Red);
                    return;
                }
            }
            catch (Exception ex)
            {
                TerminalUi.DisplayMessageWaiting("Ошибка: " + ex.Message, Color.Red);
                return;
            }

            context.DataPath = dataPath;
            _loadedDataPath = dataPath;
            DataController.LoadData(context);
            if (context.EngRusDict != null || context.EspEngDict != null || context.FreRusDict != null)
            {
                TerminalUi.DisplayMessageWaiting("Словари успешно загружены", Color.Green);
                context.IsDataLoaded = true;
            }
            else
            {
                TerminalUi.DisplayMessageWaiting("При загрузке словарей произошла ошибка", Color.Red);
            }
        }
    }
}
