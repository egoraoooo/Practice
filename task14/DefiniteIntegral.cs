namespace DefiniteIntegral;
using System;

//
// Вычисление определённого интеграла
//
public class DefiniteIntegral
{
    //
    // a, b - границы отрезка, на котором происходит вычисление опредленного интеграла
    // function - функция, для которой вычисляется определнный интеграл
    // step - размер одного шага разбиения
    // threadsNumber - число потоков, которые используются для вычислений
    //
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        // Общий результат, к которому потоки будут добавлять свои частичные суммы
        double totalResult = 0.0;

        // Количество шагов на весь отрезок
        int totalSteps = (int)Math.Ceiling((b - a) / step);

        // Количество шагов на один поток (примерно)
        int stepsPerThread = totalSteps / threadsNumber;

        // Если шагов слишком мало, используем один поток
        if (stepsPerThread < 1)
        {
            stepsPerThread = totalSteps;
            threadsNumber = 1;
        }

        // Барьер для синхронизации потоков
        var barrier = new Barrier(threadsNumber);

        // Массив потоков
        var threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;

            threads[i] = new Thread(() =>
            {
                // Определяем границы отрезка для данного потока
                double threadA = a + threadIndex * stepsPerThread * step;
                double threadB;

                if (threadIndex == threadsNumber - 1)
                {
                    // Последний поток обрабатывает оставшуюся часть до конца
                    threadB = b;
                }
                else
                {
                    threadB = threadA + stepsPerThread * step;
                }

                // Вычисляем интеграл на отрезке методом трапеций
                double localResult = CalculateTrapezoidIntegral(threadA, threadB, function, step);

                // Используем Interlocked.CompareExchange для потокобезопасного сложения double
                double currentTotal;
                double newTotal;
                do
                {
                    currentTotal = totalResult;
                    newTotal = currentTotal + localResult;
                } while (Interlocked.CompareExchange(
                    ref totalResult, 
                    newTotal, 
                    currentTotal) != currentTotal);

                // Сигнализируем о достижении барьера
                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        // Основной поток дожидается завершения всех вычислительных потоков
        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Освобождаем ресурсы барьера
        barrier.Dispose();

        return totalResult;
    }

    // Вычисляет определенный интеграл методом трапеций на заданном отрезке.
    private static double CalculateTrapezoidIntegral(double a, double b, Func<double, double> function, double step)
    {
        double result = 0.0;
        double currentX = a;

        while (currentX < b)
        {
            double nextX = Math.Min(currentX + step, b);
            double h = nextX - currentX;

            double area = (function(currentX) + function(nextX)) * h / 2.0;
            result += area;

            currentX = nextX;
        }

        return result;
    }
}