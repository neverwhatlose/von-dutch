using Spectre.Console;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace von_dutch
{
    public static class DataController
    {
        private static readonly string DictPath = Path.Combine(GetDirectoryPath(), "Dictionaries");
        
        public static void SaveData(AppContext context)
        {
            // ну еще раз на всякий случай в этом нет ничего плохого в принципе я думаю
            UpdateData(context);

            if (!Directory.Exists(context.DataPath))
            {
                Directory.CreateDirectory(context.DataPath);
            }
            
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            PropertyInfo[] properties = typeof(AppContext).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType != typeof(Dictionary<string, object>))
                {
                    continue;
                }
                
                if (property.GetValue(context) is not Dictionary<string, object> dict)
                {
                    continue;
                }
                
                DictFileAttribute? fileAttr = property.GetCustomAttribute<DictFileAttribute>();
                if (fileAttr == null)
                {
                    continue;
                }

                string filePath = Path.Combine(context.DataPath, fileAttr.FileName);
                string serializedData = JsonSerializer.Serialize(dict, options);
                File.WriteAllText(filePath, serializedData);
            }
        }
        
        public static void UpdateData(AppContext context)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            PropertyInfo[] properties = typeof(AppContext).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType != typeof(Dictionary<string, object>))
                {
                    continue;
                }
                
                if (property.GetValue(context) is not Dictionary<string, object> dict)
                {
                    continue;
                }
                
                DictFileAttribute? fileAttr = property.GetCustomAttribute<DictFileAttribute>();
                if (fileAttr == null)
                {
                    continue;
                }

                string filePath = Path.Combine(DictPath, fileAttr.FileName);
                string serializedData = JsonSerializer.Serialize(dict, options);
                File.WriteAllText(filePath, serializedData);
            }
        }
        
        public static void LoadData(AppContext context)
        {
            context.EngRusDict = DeserializeDict<string, object>(context.DataPath, "en-ru.json");
            context.EspEngDict = DeserializeDict<string, object>(context.DataPath, "es-en.json");
            context.FreRusDict = DeserializeDict<string, object>(context.DataPath, "fr-ru.json");

            if (Directory.Exists(DictPath))
            {
                Directory.Delete(DictPath, true);
            }
            Directory.CreateDirectory(DictPath);
            
            foreach (string fileName in new[] { "en-ru.json", "es-en.json", "fr-ru.json" })
            {
                if (!File.Exists(Path.Combine(context.DataPath, fileName)))
                {
                    continue;
                }
                
                File.Copy(Path.Combine(context.DataPath, fileName), Path.Combine(DictPath, fileName));
            }
        }
        
        private static Dictionary<TK, TV>? DeserializeDict<TK, TV>(string path, string fileName) where TK : notnull
        {
            try
            {
                string context = File.ReadAllText(Path.Combine(path, fileName));
                return JsonSerializer.Deserialize<Dictionary<TK, TV>>(context);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[grey][red]Ошибка:[/] При десериализации словаря {fileName} возникла ошибка: {ex.Message}[/]");
                return null;
            }
        }
        
        private static string GetDirectoryPath()
        {
            string[] directories = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);
            return string.Join(Path.DirectorySeparatorChar, directories,
                0, directories.Length - 3) + Path.DirectorySeparatorChar;
        }
    }
}