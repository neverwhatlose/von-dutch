using System.Reflection;
using Spectre.Console;
using von_dutch.Attributes;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks
{
    /// <summary>
    /// Базовый абстрактный класс для всех задач приложения.
    /// Предоставляет общие методы и свойства для работы с задачами.
    /// </summary>
    public abstract class TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public virtual bool NeedsData { get; } = false;

        /// <summary>
        /// Выполняет задачу.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        public abstract void Execute(AppContext context);

        /// <summary>
        /// Выбирает словарь из доступных в контексте приложения.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий словари.</param>
        /// <returns>Выбранный словарь или null, если словари недоступны.</returns>
        protected static Dictionary<string, object>? SelectDictionary(AppContext context)
        {
            List<Dictionary<string, object>> availableDicts = [];
            Dictionary<Dictionary<string, object>, string> dictNames = [];

            foreach (PropertyInfo property in context.GetType().GetProperties())
            {
                if (property.PropertyType != typeof(Dictionary<string, object>))
                {
                    continue;
                }

                if (property.GetValue(context) is not Dictionary<string, object> dict)
                {
                    continue;
                }

                DictFileAttribute? attr = property.GetCustomAttribute<DictFileAttribute>();
                string displayName = attr != null ? attr.FileName : property.Name;
                availableDicts.Add(dict);
                dictNames[dict] = displayName;
            }

            if (availableDicts.Count != 0)
            {
                return AnsiConsole.Prompt(
                    new SelectionPrompt<Dictionary<string, object>>()
                        .Title("[grey]Выберите доступный словарь[/]")
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Нажимайте 🔼 и 🔽, чтобы открыть больше список)[/]")
                        .AddChoices(availableDicts)
                        .UseConverter(dict => dictNames[dict])
                );
            }

            TerminalUi.DisplayMessageWaiting("Нет доступных словарей", Color.Red);
            return null;
        }

        /// <summary>
        /// Получает имя словаря на основе его свойств в контексте приложения.
        /// </summary>
        /// <param name="dict">Словарь, для которого нужно получить имя.</param>
        /// <param name="context">Контекст приложения, содержащий словари.</param>
        /// <returns>Имя словаря или "Unknown", если словарь не найден.</returns>
        protected static string GetDictName(Dictionary<string, object> dict, AppContext context)
        {
            foreach (PropertyInfo property in context.GetType().GetProperties())
            {
                if (property.PropertyType != typeof(Dictionary<string, object>))
                {
                    continue;
                }

                if (property.GetValue(context) is not Dictionary<string, object> dictValue)
                {
                    continue;
                }

                if (dictValue != dict)
                {
                    continue;
                }

                DictFileAttribute? attr = property.GetCustomAttribute<DictFileAttribute>();
                return attr != null ? attr.FileName : property.Name;
            }

            return "Unknown";
        }
    }
}