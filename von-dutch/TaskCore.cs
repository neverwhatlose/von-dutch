using System.Reflection;
using Spectre.Console;

namespace von_dutch
{
    public abstract class TaskCore
    {
        public abstract string Title { get; }
        public virtual bool NeedsData { get; } = false;
        public abstract void Execute(AppContext context);
        
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

            TerminalUi.DisplayMessage("Нет доступных словарей", Color.Red);
            return null;
        }
    }
}
