namespace von_dutch
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

            // Ширина и высота окна консоли
            // Можете подстроить под себя, если нужно больше/меньше
            Console.SetWindowSize(120, 50);
            ////Console.SetBufferSize(120, 50);

            while (true)
            {
                Memset(b, ' ', 1760);
                Memset(z, 0.0, 7040);

                // Рисуем «пончик»
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

                        // Увеличиваем «масштаб» пончика, меняя 40 -> 50 и 30 -> 40 (по x), 
                        // 12 -> 15 и 15 -> 20 (по y)
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

                // Возвращаем курсор в верхний левый угол,
                // чтобы «перерисовать» пончик в одном и том же месте
                Console.SetCursorPosition(0, 0);

                // Выводим пончик построчно с раскраской
                PrintColoredDonut(b);

                // Небольшая пауза
                Thread.Sleep(16);

                // Изменяем углы, чтобы пончик «вращался»
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

            // Выставляем символ перевода строки каждые 80 символов
            private static void Nl(char[] b)
            {
                for (int i = 80; i < 1760; i += 80)
                {
                    b[i] = '\n';
                }
            }

            // Выводим каждый символ с цветом
            private static void PrintColoredDonut(char[] b)
            {
                // Вставляем переносы строк в нужные позиции
                Nl(b);

                for (int i = 0; i < b.Length; i++)
                {
                    // Если символ = перевод строки, просто делаем WriteLine()
                    if (b[i] == '\n')
                    {
                        Console.WriteLine();
                        continue;
                    }

                    // Выбираем цвет в зависимости от символа
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

                // Сброс цвета после вывода
                Console.ResetColor();
            }
    }
}