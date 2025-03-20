using AppContext = von_dutch.Wrappers.AppContext;

namespace von_dutch.Tasks.Commands
{
    public class GiftTask : TaskCore
    {
        public override string Title { get; } = "Подарок (не рекомендую нажимать, не относится к проекту)";
        public override bool NeedsData { get; } = false;

        public override void Execute(AppContext context)
        {
            double i1 = 0, i2 = 0;
            double i, j;
            double[] z = new double[7040];
            char[] b = new char[1760];
            
            Console.SetWindowSize(120, 50);

            while (true)
            {
                Memset(b, ' ', 1760);
                Memset(z, 0.0, 7040);
                
                for (j = 0; j < 6.28; j += 0.07)
                {
                    for (i = 0; i < 6.28; i += 0.02)
                    {
                        double c = Math.Sin(i);
                        double d = Math.Cos(j);
                        double e = Math.Sin(i1);
                        double f = Math.Sin(j);
                        double g = Math.Cos(i1);
                        double h = d + 2;
                        double d1 = 1 / ((c * h * e) + (f * g) + 5);
                        double l = Math.Cos(i);
                        double m = Math.Cos(i2);
                        double n = Math.Sin(i2);
                        double t = (c * h * g) - (f * e);
                        
                        int x = (int)(50 + (40 * d1 * ((l * h * m) - (t * n))));
                        int y = (int)(15 + (20 * d1 * ((l * h * n) + (t * m))));

                        int o = x + (80 * y);
                        int index = (int)(8 * ((((f * e) - (c * d * g)) * m) - (c * d * e) - (f * g) - (l * d * n)));

                        if (y < 22 && y > 0 && x > 0 && x < 80 && d1 > z[o])
                        {
                            z[o] = d1;
                            b[o] = ".,-~:;=!*#$@"[index > 0 ? index : 0];
                        }
                    }
                }
                
                Console.SetCursorPosition(0, 0);
                
                PrintColoredDonut(b);
                
                Thread.Sleep(16);
                
                i1 += 0.04;
                i2 += 0.02;
            }
        }

        private static void Memset<T>(T[] buf, T val, int bufsz)
        {
            if (buf == null)
            {
                buf = new T[bufsz];
            }

            for (int i = 0; i < bufsz; i++)
            {
                buf[i] = val;
            }
        }
    
        private static void Nl(char[] b)
        {
            for (int i = 80; i < 1760; i += 80)
            {
                b[i] = '\n';
            }
        }
        
        private static void PrintColoredDonut(char[] b)
        {
            Nl(b);

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == '\n')
                {
                    Console.WriteLine();
                    continue;
                }
                
                switch (b[i])
                {
                    case '.': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                    case ',': Console.ForegroundColor = ConsoleColor.Gray; break;
                    case '-': Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case '~': Console.ForegroundColor = ConsoleColor.Green; break;
                    case ':': Console.ForegroundColor = ConsoleColor.Cyan; break;
                    case ';': Console.ForegroundColor = ConsoleColor.Magenta; break;
                    case '=': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                    case '!': Console.ForegroundColor = ConsoleColor.Red; break;
                    case '*': Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                    case '#': Console.ForegroundColor = ConsoleColor.Blue; break;
                    case '$': Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                    case '@': Console.ForegroundColor = ConsoleColor.White; break;
                    default: Console.ForegroundColor = ConsoleColor.White; break;
                }

                Console.Write(b[i]);
            }
            
            Console.ResetColor();
        }
    }
}