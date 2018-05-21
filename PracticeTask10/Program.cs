using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTask10
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к файлу, в котором записан граф, который Вы хотите обработать:");
            string Path = Console.ReadLine();

            // Чтение графа из файла
            Graph Graph = Graph.ReadGraph(Path);

            // Если удалось прочитать граф
            if (Graph != null)
            {
                // Значение, записанное в вершинах, которые надо стянуть
                int Value;
                // Флаг правильности ввода
                bool ok;

                // Ввод значения, которое хотим стянуть
                do
                {
                    Console.Write("Введите значение, записанное в вершинах, которые надо стянуть: ");
                    ok = Int32.TryParse(Console.ReadLine(), out Value);
                    if (!ok)
                        Console.WriteLine("В вершинах записаны целые числа.");
                } while (!ok);

                // Стягиваем вершины с указанным значением
                Graph.Contraction(Value);

                // Запись полученного графа в файл
                Graph.WriteGraph(Path + "output");
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
