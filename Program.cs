using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TriangleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Определение треугольника ===");
            while (true)
            {
                Console.Write("\nВведите сторону A (или 'exit' для выхода): ");
                string inputA = Console.ReadLine();
                if (inputA?.ToLower() == "exit") break;

                Console.Write("Введите сторону B: ");
                string inputB = Console.ReadLine();
                Console.Write("Введите сторону C: ");
                string inputC = Console.ReadLine();

                ProcessRequest(inputA, inputB, inputC);
            }
        }

        static void ProcessRequest(string strA, string strB, string strC)
        {
            DateTime timestamp = DateTime.Now;
            bool success = false;
            string triangleType = "";
            List<(int X, int Y)> coordinates = new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) };
            string errorMessage = null;

            try
            {
                if (!float.TryParse(strA, out float a) || a <= 0 ||
                    !float.TryParse(strB, out float b) || b <= 0 ||
                    !float.TryParse(strC, out float c) || c <= 0)
                {
                    throw new ArgumentException("Все стороны должны быть положительными вещественными числами.");
                }

                triangleType = GetTriangleType(a, b, c);
                if (triangleType == "не треугольник")
                {
                    coordinates = new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) };
                }
                else
                {
                    coordinates = CalculateVertexCoordinates(a, b, c);
                }
                success = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + "\n" + ex.StackTrace;
                triangleType = "";
                coordinates = new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) };
                success = false;
            }

            LogRequest(timestamp, strA, strB, strC, triangleType, coordinates, success, errorMessage);

            if (success)
            {
                Console.WriteLine($"Тип треугольника: {triangleType}");
                Console.WriteLine($"Координаты: A({coordinates[0].X},{coordinates[0].Y}) " +
                                  $"B({coordinates[1].X},{coordinates[1].Y}) " +
                                  $"C({coordinates[2].X},{coordinates[2].Y})");
            }
            else
            {
                Console.WriteLine($"Ошибка: {errorMessage ?? "Некорректный ввод"}");
            }
        }

        static string GetTriangleType(float a, float b, float c)
        {
            if (a + b <= c || a + c <= b || b + c <= a)
                return "не треугольник";

            if (Math.Abs(a - b) < 1e-6 && Math.Abs(b - c) < 1e-6)
                return "равносторонний";
            if (Math.Abs(a - b) < 1e-6 || Math.Abs(a - c) < 1e-6 || Math.Abs(b - c) < 1e-6)
                return "равнобедренный";
            return "разносторонний";
        }

        static List<(int X, int Y)> CalculateVertexCoordinates(float a, float b, float c)
        {
            const int leftX = 10;
            const int rightX = 90;
            const int baseY = 90;

            float scale = 80f / c;
            float scaledA = a * scale;
            float scaledB = b * scale;

            float dx = (scaledB * scaledB - scaledA * scaledA + 80 * 80) / (2 * 80);
            float dy = (float)Math.Sqrt(scaledB * scaledB - dx * dx);

            int Cx = leftX + (int)Math.Round(dx);
            int Cy = baseY - (int)Math.Round(dy);

            Cx = Math.Clamp(Cx, 0, 100);
            Cy = Math.Clamp(Cy, 0, 100);

            return new List<(int, int)> { (leftX, baseY), (rightX, baseY), (Cx, Cy) };
        }

        static void LogRequest(DateTime timestamp, string a, string b, string c, string type,
                               List<(int X, int Y)> coords, bool success, string errorMsg)
        {
            string logEntry = $"[{timestamp:yyyy-MM-dd HH:mm:ss}] " +
                              $"Вход: ({a}, {b}, {c}) -> " +
                              $"Тип: {(success ? type : "ОШИБКА")} " +
                              $"Координаты: {string.Join("; ", coords.Select(p => $"({p.X},{p.Y})"))}";
            if (!success && errorMsg != null)
                logEntry += $"\nОшибка: {errorMsg}";

            Console.WriteLine(logEntry);
            File.AppendAllText("triangle_log.txt", logEntry + Environment.NewLine + Environment.NewLine);
        }
    }
}
