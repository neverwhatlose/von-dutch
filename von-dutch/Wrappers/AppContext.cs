using von_dutch.Attributes;

namespace von_dutch.Wrappers
{
    /// <summary>
    /// Класс, представляющий контекст приложения.
    /// Содержит данные и состояние приложения, включая загруженные словари и путь к данным.
    /// </summary>
    public class AppContext
    {
        /// <summary>
        /// Флаг, указывающий, загружены ли данные в приложение.
        /// </summary>
        public bool IsDataLoaded { get; set; } = false;

        /// <summary>
        /// Путь к папке с данными (словарями).
        /// </summary>
        public string DataPath { get; set; } = string.Empty;

        /// <summary>
        /// Словарь английско-русских переводов.
        /// </summary>
        [DictFile("en-ru.json")]
        public Dictionary<string, object>? EngRusDict { get; set; }

        /// <summary>
        /// Словарь испанско-английских переводов.
        /// </summary>
        [DictFile("es-en.json")]
        public Dictionary<string, object>? EspEngDict { get; set; }

        /// <summary>
        /// Словарь французско-русских переводов.
        /// </summary>
        [DictFile("fr-ru.json")]
        public Dictionary<string, object>? FreRusDict { get; set; }
    }
}