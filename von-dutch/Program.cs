/*
 * Проект #3.2 - словарь Von Dutch
 *
 * ФИО студента - Селчук Эмин Абдулкеримович
 * Дата сдачи - 20.03.2025 до 18:00
 * Вариант проекта: 7
 * Выбранный путь: B-SIDE
 */

using von_dutch.Menu;

namespace von_dutch
{
    /// <summary>
    /// Главный класс программы, представляющий точку входа в приложение.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Terminal terminal = new();
            terminal.Run();
        }
    }
}