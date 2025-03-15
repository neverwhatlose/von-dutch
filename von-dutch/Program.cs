// See https://aka.ms/new-console-template for more information

using von_dutch.Menu;

namespace von_dutch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Terminal terminal = new();
            terminal.Run();
        }
    }
}