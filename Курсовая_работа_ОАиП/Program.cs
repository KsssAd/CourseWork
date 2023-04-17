using System;
using System.Threading;

namespace Курсовая_работа_ОАиП
{
    class Program
    {
        #region Input matrix

        /// <summary>
        /// Ввод матрицы 
        /// </summary>
        /// <returns></returns>
        private int[,] InputMatrix()
        {
            bool manualInput = InputAndCheckUsersAnswer(
                "Вы хотите решить матрицу введенную с клавиатуры или сгенерированную автоматически?", 
                "Ввести с клавиатуры", "Сгенерировать автоматически");

            Separator();

            int numberVariable = InputAndCheckNumber("Введите количество неизвестных: ");
            int numberEquation = numberVariable;

            int[,] matrix = new int[numberEquation, numberVariable + 1];

            if (manualInput != true)
                GenerateRandomMatrix(matrix, numberEquation, numberVariable);
            else
                InputMatrixFromKeyboard(matrix, numberEquation, numberVariable);

            GenerateLinearEquationSystem(matrix, numberEquation, numberVariable);

            return matrix;
        }

        /// <summary>
        /// Генерация системы линейных уравнений на основе введенных аргументов
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="numberEquation"></param>
        /// <param name="numberVariable"></param>
        private void GenerateLinearEquationSystem(int[,] matrix, int numberEquation, int numberVariable)
        {
            Separator();
            Console.WriteLine("Сгенерированная система линейных уравнений:\n");

            for (int i = 0; i < numberEquation; i++)
            {
                string equation = "";

                for (int j = 0; j < numberVariable; j++)
                {
                    int coef = matrix[i, j];
                    string sign = coef >= 0 
                        ? j == 0 
                            ? "" 
                            : "+" 
                        : "-";
                    string term = j == numberVariable 
                        ? $"{Math.Abs(coef)}" 
                        : $"{Math.Abs(coef)}*x{j+1}";
                    equation += $"{sign} {term} ";
                }
                Console.WriteLine($"{equation} = {matrix[i, numberVariable]}");
            }

            Thread.Sleep(1300);
        }

        /// <summary>
        /// Автоматическая генерация матрицы 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="numberEquation"></param>
        /// <param name="numberVariable"></param>
        private void GenerateRandomMatrix(int[,] matrix, int numberEquation, int numberVariable)
        {
            Random random = new Random();
            bool isSolvable = false;

            while (isSolvable != true)
            {
                for (int i = 0; i < numberEquation; i++)
                {
                    for (int j = 0; j < numberVariable + 1; j++)
                    {
                        matrix[i, j] = random.Next(-50, 50);
                    }
                }

                isSolvable = CheckMatrixHasSolution(matrix);
            }
        }

        /// <summary>
        /// Ввод матрицы пользователем с клавиатуры 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="numberEquation"></param>
        /// <param name="numberVariable"></param>
        private void InputMatrixFromKeyboard(int[,] matrix, int numberEquation, int numberVariable)
        {
            bool isSolvable = false;

            while (isSolvable != true)
            {               
                for (int i = 0; i < numberEquation; i++)
                {
                    Console.WriteLine($"\nВвод {i + 1} строки:\n");
                    for (int j = 0; j < numberVariable + 1; j++)
                    {
                        Console.Write(j + 1 != numberVariable + 1
                                            ? $"x{j + 1} = "
                                            : $"Свободный член {i + 1} строки = ");
                        int.TryParse(Console.ReadLine(), out int number);
                        matrix[i, j] = number;
                    }
                }

                isSolvable = CheckMatrixHasSolution(matrix);

                if (!isSolvable)
                    Console.WriteLine($"\nВведенная вами матрица является нерешаемой. Пожалуйста, введите другую матрицу!");
            }
        }

        /// <summary>
        /// Проверка, будет ли полученная матица иметь решение 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private bool CheckMatrixHasSolution(int[,] matrix)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n-1; i++)
            {
                bool isZeroRow = true;
                for (int j = i; j < n; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        isZeroRow = false;
                        break;
                    }
                }

                if (!(!isZeroRow && matrix[i, n] != 0))              
                    return false;                            
            }


            for (int i = 0; i < n; i++)
            {
                int maxElem = Math.Abs(matrix[i, i]);
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(matrix[j,i]) > maxElem)
                    {
                        maxElem = Math.Abs(matrix[j, i]);

                    }
                }

                if (maxElem == 0)
                    return false;
            }

            double det = Determinant(matrix, n);
            if (det <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Вычисление определителя 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private double Determinant(int[,] matrix, int n)
        {
            double det = 0;

            if (n == 2)
            {
                det = Determinant2By2(matrix);
            }
            else
            {
                for (int j = 0; j < n; j++)
                {
                    int[,] submatrix = new int[n - 1, n - 1];
                    for (int i = 1; i < n; i++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            if (k < j)
                                submatrix[i - 1, k] = matrix[i, k];
                            else if (k > j)
                                submatrix[i - 1, k - 1] = matrix[i, k];                           
                        }
                    }

                    det += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * Determinant2By2(submatrix);
                }
            }
            return det;
        }

        /// <summary>
        /// Вычисление определителя второго порядка
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private double Determinant2By2(int[,] matrix)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        /// <summary>
        /// Ввод и проверка введенного размера матрицы 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private int InputAndCheckNumber(string message)
        {
            bool isSizeCorrect = false;
            int number = 0;

            while (isSizeCorrect != true)
            {
                Console.Write(message);
                string inputNumber = Console.ReadLine();

                if (int.TryParse(inputNumber, out number))
                {
                    if (number >= 10)
                    {
                        Console.WriteLine("\nВведите число не больше 10 !\n");
                        continue;
                    }
                    else if (number < 2)
                    {
                        Console.WriteLine("\nВведите число больше 1 !\n");
                        continue;
                    }
                    else                       
                        isSizeCorrect = true;
                }
                else
                {
                    Console.WriteLine("\nВведите целое число !\n");
                    continue;
                }
            }

            return number;
        }

        #endregion

        /// <summary>
        /// Получение и вывод преобразованной (треугольной) матрицы методом Гаусса
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private void GaussMethod(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            double[,] newMatrix = GetDoubleMatrix(matrix);

            ///Прямой ход Гаусса
            for (int j = 0; j < n-1; j++)
            {
                for (int i = j+1; i < n; i++)
                {
                    double coef = newMatrix[i, j] / newMatrix[j, j];
                    for (int k = j; k < n+1; k++)
                    {
                        newMatrix[i, k] = Math.Round(newMatrix[i, k] - coef * newMatrix[j, k], 2);
                    }
                    newMatrix[i, j] = 0;
                }
            }

            ///Обратный ход Гаусса
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                
                for (int j = i + 1; j < n; j++)
                {
                    sum += newMatrix[i, j] * x[j];
                }
                x[i] = Math.Round((newMatrix[i, n] - sum) / newMatrix[i, i], 2);
            }

            Console.WriteLine("Матрица после преобразований:\n");
            PrintMatrix(newMatrix);
            Separator();
            Thread.Sleep(1300);

            Console.WriteLine("Решение: ");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"x[{i+1}] = {x[i]}");
            }
            Console.WriteLine("\nОбратите внимание, что ответы указаны с учетом округления! ");
            Separator();
        }

        /// <summary>
        /// Конвертирование матрицы из int в double 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private double[,] GetDoubleMatrix(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            double[,] doubleMatrix = new double[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    doubleMatrix[i, j] = (double)matrix[i, j];
                }
            }

            return doubleMatrix;
        }

        /// <summary>
        /// Ввод и проверка выбора пользователя 
        /// </summary>
        /// <param name="question"></param>
        /// <param name="firstAnswer"></param>
        /// <param name="secondAnswer"></param>
        /// <returns></returns>
        private bool InputAndCheckUsersAnswer(string question, string firstAnswer, string secondAnswer)
        {
            bool isCorrect = false;
            bool result = false;

            while (isCorrect != true)
            {
                Console.WriteLine(question);
                Console.WriteLine($"1 - {firstAnswer}, 0 - {secondAnswer}\n");
                string answer = Console.ReadLine();

                if (int.TryParse(answer, out int yesInInt) && yesInInt == 1)
                {
                    result = true;
                    isCorrect = true;
                }               
                else if (int.TryParse(answer, out int noInInt) && noInInt == 0)
                {
                    result = false;
                    isCorrect = true;
                }
                else
                {
                    Console.WriteLine("\nВведите ваш ответ в соответствии с инструкцией!\n");
                    isCorrect = false;
                }
            }

            return result;
        }

        private void Separator()
        {
            Console.WriteLine("\n-------------------------------------------------------------------------------\n");
        }

        /// <summary>
        /// Печать матрицы в консоли 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        private void PrintMatrix<T>(T[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (j != m-2)
                        Console.Write("{0,8}", matrix[i, j]);
                    else
                    {
                        Console.Write("{0,8}", matrix[i, j]);
                        Console.Write("{0,8}", "|");
                    }
                }
                Console.WriteLine();
            }
            Thread.Sleep(1300);
        }            

        static void Main(string[] args)
        {
            Program prog = new Program();

            prog.Separator();
            Console.WriteLine("Добро пожаловать в программу расчета системы линейных уравнений методом Гаусса!");

            bool repeat = true;
            while (repeat != false)
            {
                prog.Separator();
                int[,] matrix = prog.InputMatrix();

                prog.Separator();
                Console.WriteLine("Полученная матрица:\n");
                prog.PrintMatrix(matrix);
                prog.Separator();

                prog.GaussMethod(matrix);

                repeat = prog.InputAndCheckUsersAnswer("Хотите решить еще одну систему?", "Да", "Нет");
            }
        }
    }
}