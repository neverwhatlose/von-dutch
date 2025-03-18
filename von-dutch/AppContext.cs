namespace von_dutch
{
    public class AppContext
    {
        public bool IsDataLoaded { get; set; } = false;

        [DictFile("en-ru.json")]
        public Dictionary<string, object>? EngRusDict { get; set; }
        
        [DictFile("es-en.json")]
        public Dictionary<string, object>? EspEngDict { get; set; }
        
        [DictFile("fr-ru.json")]
        public Dictionary<string, object>? FreRusDict { get; set; }
    }
}