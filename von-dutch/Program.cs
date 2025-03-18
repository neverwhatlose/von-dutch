using von_dutch.Menu;

namespace von_dutch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Terminal terminal = new();
            terminal.Run();
        }
    }
}