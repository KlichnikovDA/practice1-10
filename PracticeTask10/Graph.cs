using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PracticeTask10
{
    public class Graph
    {
        // Количество вершин графа
        int Size;
        // Граф задается матрицей смежности
        byte[,] AdjacencyMatrix;
        // Массив значений, записанных в вершинах
        int[] Values;

        // Конструктор
        public Graph(int Size, int[] Val, byte[,] Matrix)
        {
            this.Size = Size;
            AdjacencyMatrix = Matrix;
            Values = Val;
        }

        // Чтение графа из файла
        public static Graph ReadGraph(string P)
        {
            try
            {
                FileStream File = new FileStream(P, FileMode.Open);
                StreamReader sr = new StreamReader(File);
                // Размер графа
                int Size;
                // Флаг правильности ввода
                bool ok = Int32.TryParse(sr.ReadLine(), out Size);
                // Массив значений
                string vals = sr.ReadLine();
                int[] Val = new int[Size];
                if (vals.Length > Size * 2 - 1)
                    vals = vals.Remove(Size * 2 - 1);
                Val = vals.Split(' ').Select(n => Int32.Parse(n)).ToArray();
                // Матрица смежности
                byte[,] Matrix = new byte[Size, Size];
                for (int i = 0; i < Size; i++)
                {
                    vals = sr.ReadLine();
                    if (vals.Length > Size * 2 - 1)
                        vals = vals.Remove(Size * 2 - 1);
                    // Чтение строки матрицы
                    byte[] Row = vals.Split(' ').Select(n => Byte.Parse(n)).ToArray();
                    for (int j = 0; j < Size; j++)
                    {
                        if (Row[j] != 0 && Row[j] != 1)
                        {
                            Console.WriteLine("В файле содержатся некорректные данные.");
                            return null;
                        }
                        Matrix[i, j] = Row[j];
                    }
                }

                sr.Close();
                File.Close();

                return new Graph(Size, Val, Matrix);
            }
            catch
            {
                Console.WriteLine("Не удается открыть файл, проверьте его наличие и правильность пути.");
                return null;
            }
        }

        // Запись графа в файл
        public void WriteGraph(string P)
        {
            P = Path.GetDirectoryName(P) + Path.GetFileNameWithoutExtension(P) + "output" + Path.GetExtension(P);
            FileStream File;
            try
            {
                File = new FileStream(P, FileMode.Truncate);
            }
            catch (FileNotFoundException)
            {
                File = new FileStream(P, FileMode.CreateNew);
            }
            StreamWriter sw = new StreamWriter(File);
            // Размер графа
            sw.WriteLine(Size);
            // Массив значений
            foreach (int item in Values)
                sw.Write(item + " ");
            sw.WriteLine();
            // Матрица смежности
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                    sw.Write(AdjacencyMatrix[i, j] + " ");
                sw.WriteLine();
            }

            Console.WriteLine("Информация об обработанном графе записана в файл " + P);

            sw.Close();
            File.Close();
        }

        // Стягиевание графа
        public void Contraction(int Val)
        {
            // Если искомое значение отсутствует в графе
            if (!Values.Contains(Val))
            {
                Console.WriteLine("В графе нет вершины с указанным значением.");
                return;
            }
            // Номер вершины, где впервые встречается искомое значение
            int FirstVertex = Array.IndexOf(Values, Val);

            int i = FirstVertex+1;
            // Проходим по оставшимся вершинам графа
            while (i < Size)
            {
                // Если нашлась еще одна вершина с искомым значением
                if (Values[i] == Val)
                {
                    // Переносим ребра
                    // Перенос ребер, исходящих из этой вершины
                    for (int col = 0; col < Size; col++)
                        // Если из данной вершины исходит ребро (но не в точку, в которую стягиваем, чтобы не было петлей)
                        if (AdjacencyMatrix[i, col] == 1 && col != FirstVertex)
                            // Переносим начало ребра в вершину, куда стягиваем
                            AdjacencyMatrix[FirstVertex, col] = 1;

                    // Перенос ребер, входящих в эту вершину
                    for (int row = 0; row < Size; row++)
                        // Если в данную вершину входит ребро (но не из точки, в которую стягиваем, чтобы не было петлей)
                        if (AdjacencyMatrix[row, i] == 1 && row != FirstVertex)
                            // Переносим конец ребра в вершину, куда стягиваем
                            AdjacencyMatrix[row, FirstVertex] = 1;

                    // Удаление вершины из графа
                    RemoveVertex(i);
                }
                else
                    i++;
            }
        }

        // Удаление вершины графа
        void RemoveVertex(int index)
        {
            // Удаление значения
            int[] NewValues = new int[Size - 1];
            for (int i = 0; i < index; i++)
                NewValues[i] = Values[i];
            for (int i = index + 1; i < Size; i++)
                NewValues[i - 1] = Values[i];
            Values = NewValues;

            // Удаление вершины из матрицы
            byte[,] NewMatrix = new byte[Size - 1, Size - 1];

            // Копирование незатронутой части
            for (int i = 0; i < index; i++)
                for (int j = 0; j < index; j++)
                    NewMatrix[i, j] = AdjacencyMatrix[i, j];
            // Удаление столбца
            for (int i = 0; i < NewMatrix.GetLength(0); i++)
                for (int j = index; j < NewMatrix.GetLength(1); j++)
                    NewMatrix[i, j] = AdjacencyMatrix[i, j + 1];
            // Удаление строки
            for (int i = index; i < NewMatrix.GetLength(0); i++)
                for (int j = 0; j < NewMatrix.GetLength(1); j++)
                    NewMatrix[i, j] = AdjacencyMatrix[i + 1, j];
            AdjacencyMatrix = NewMatrix;

            Size--;
        }
    }
}