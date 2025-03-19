using System.Text;
using System.Text.Json;

namespace von_dutch
{
    public static class TerminalUiExtensions
    {
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