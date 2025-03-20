using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Commands
{
    /// <summary>
    /// Класс, представляющий задачу выбора текущей языковой пары и загрузки словарей.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class LoadDictTask : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Выбор текущей языковой пары";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = false;

        private string _loadedDataPath = string.Empty;

        /// <summary>
        /// Выполняет задачу выбора папки со словарями и загрузки данных.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.Exception">
        /// Может возникнуть при проверке существования папки или файлов.
        /// </exception>
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