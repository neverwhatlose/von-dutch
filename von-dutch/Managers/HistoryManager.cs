namespace von_dutch.Managers
{
    /*
     * ваще тут лирическое отступление хочется сделать
     * с синглтонами я познакомился когда учил котлин
     * это было не прям недавно, в ноябре 24-го года
     * и я прям влюбился в концепцию синглтонов
     * типа это намного круче чем статика
     * ты можешь создать класс который будет хранить стейт
     * и работать с ним как с обычным классом
     * но только он не обычный класс, а в единственном экземпляре
     * поэтому я очень надеялся что среди всего синтаксического сахара шарпов
     * я найду какой-то способ объявлять их
     * но увы. его нет
     * kotlin 1 : 0 c#
     *
     * кстати если интересно, в котлине синглтон объявляется так:
     * object Singleton {
           ...
        }
     *
     * ты кстати подписан на цитатник пивных преподов?))))))
     */

    /// <summary>
    /// Класс для управления историей операций, таких как перевод слов или предложений.
    /// Использует шаблон Singleton для обеспечения единственного экземпляра класса.
    /// </summary>
    public class HistoryManager
    {
        // синтаксический сахар для синглтона
        private static readonly Lazy<HistoryManager> SingletonInstance = new(() => new HistoryManager());

        /// <summary>
        /// Единственный экземпляр класса HistoryManager.
        /// </summary>
        public static HistoryManager Instance => SingletonInstance.Value;

        private HistoryManager() { }

        /// <summary>
        /// Список, хранящий историю операций. Каждая операция представлена списком строк.
        /// </summary>
        public List<List<string>> History { get; } = [];

        /// <summary>
        /// Логирует операцию в историю.
        /// </summary>
        /// <param name="date">Дата и время операции.</param>
        /// <param name="dictionary">Название словаря, с которым связана операция.</param>
        /// <param name="word">Слово или фраза, которая была переведена.</param>
        /// <param name="translation">Перевод слова или фразы.</param>
        /// <param name="status">Статус операции (например, успех или ошибка).</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если любой из параметров равен null.</exception>
        public static void Log(
            string date,
            string dictionary,
            string word,
            string translation,
            string status
        )
        {
            List<string> pair = [
                date,
                dictionary,
                word,
                translation,
                status
            ];
            Instance.History.Insert(0, pair);
        }
    }
}