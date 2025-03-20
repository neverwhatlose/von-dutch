using System.Text;
using System.Text.Json;

namespace von_dutch.Menu
{
    /// <summary>
    /// Статический класс, предоставляющий методы расширения для работы с интерфейсом терминала.
    /// </summary>
    public static class TerminalUiExtensions
    {
        /// <summary>
        /// Преобразует входное значение в строку перевода, учитывая различные типы данных.
        /// </summary>
        /// <param name="value">Входное значение, которое необходимо преобразовать в строку.</param>
        /// <returns>
        /// Возвращает строковое представление входного значения. 
        /// Если значение является массивом JSON, элементы массива объединяются через запятую.
        /// Если значение не может быть преобразовано, возвращается пустая строка.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Может возникнуть, если входное значение или его компоненты равны null.
        /// </exception>
        public static string GetTranslationAsString(object value)
        {
            string result;
            switch (value)
            {
                case string s:
                    result = s;
                    break;
                case JsonElement { ValueKind: JsonValueKind.Array } jsonElement:
                    {
                        StringBuilder sb = new ();
                        foreach (JsonElement item in jsonElement.EnumerateArray())
                        {
                            sb.Append(item.GetString());
                            sb.Append(", ");
                        }
                        if (sb.Length >= 2)
                        {
                            sb.Remove(sb.Length - 2, 2);
                        }
                        result = sb.ToString();
                        break;
                    }
                case JsonElement jsonElement:
                    result = jsonElement.GetString() ?? string.Empty;
                    break;
                default:
                    result = value.ToString() ?? string.Empty;
                    break;
            }
            return result;
        }
    }
}