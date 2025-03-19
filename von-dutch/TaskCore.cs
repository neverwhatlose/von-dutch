using Spectre.Console;
using System.Reflection;

namespace von_dutch
{
    public abstract class TaskCore
    {
        public virtual bool NeedsData => false;
        
        public abstract string Title { get; }
        
        public abstract void Execute(AppContext context);
        
        protected Dictionary<string, object> SelectDictionary(AppContext context)
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

            return AnsiConsole.Prompt(
                new SelectionPrompt<Dictionary<string, object>>()
                    .Title("[grey]Выберите доступный словарь[/]")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Нажимайте 🔼 и 🔽, чтобы открыть больше список)[/]")
                    .AddChoices(availableDicts)
                    .UseConverter(dict => dictNames[dict])
            );
        }

    }
}