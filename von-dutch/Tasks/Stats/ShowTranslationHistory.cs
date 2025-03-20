using Spectre.Console;
using von_dutch.Managers;
using von_dutch.Menu;
using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Stats
{
    /// <summary>
    /// Класс, представляющий задачу показа истории переводов.
    /// Наследуется от базового класса TaskCore.
    /// </summary>
    public class ShowTranslationHistory : TaskCore
    {
        /// <summary>
        /// Заголовок задачи, отображаемый в интерфейсе.
        /// </summary>
        public override string Title { get; } = "Показать историю переводов";

        /// <summary>
        /// Флаг, указывающий, требует ли задача данные для выполнения.
        /// </summary>
        public override bool NeedsData { get; } = true;

        /// <summary>
        /// Выполняет задачу показа истории переводов в виде пагинированной таблицы.
        /// </summary>
        /// <param name="context">Контекст приложения, содержащий необходимые данные и состояние.</param>
        /// <exception cref="System.NullReferenceException">
        /// Может возникнуть, если история переводов не инициализирована.
        /// </exception>
        public override void Execute(AppContext context)
        {
            List<TableColumn> columns = [
                new ("[green]Дата[/]"),
                new ("[green]Словарь[/]"), 
                new ("[green]Слово[/]"), 
                new ("[green]Перевод[/]"), 
                new ("[green]Статус[/]")
                {
                    Alignment = Justify.Center
                }
            ];
            TerminalUi.PrintPaginatedTable("История переводов", columns, HistoryManager.Instance.History, 30);
        }
    }
}