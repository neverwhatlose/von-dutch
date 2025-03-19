using Spectre.Console;

namespace von_dutch
{
    public class ShowTranslationHistory : TaskCore
    {
        public override string Title { get; } = "Показать историю переводов (NIY)";
        public override bool NeedsData { get; } = true;
        
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