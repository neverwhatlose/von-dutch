namespace von_dutch
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
    public class HistoryManager
    {
        // синтаксический сахар для синглтона
        private static readonly Lazy<HistoryManager> SingletonInstance = new(() => new HistoryManager());
        public static HistoryManager Instance => SingletonInstance.Value;
        private HistoryManager() { }

        public List<List<string>> History { get; private set; } = [];

        public void Log(
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
            Instance.History.Insert(0, pair);;
        }
    }

}