namespace DefiniteIntegral;
using System;
using System.Threading;

public class DefiniteIntegral
{
    // Многопоточная версия из прошлой задачи (исходная)
    // a, b — границы отрезка
    // function — функция для интегрирования
    // step — размер одного шага разбиения
    // threadsNumber — число потоков
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double totalResult = 0.0;
        int totalSteps = (int)Math.Ceiling((b - a) / step);
        int stepsPerThread = totalSteps / threadsNumber;

        if (stepsPerThread < 1)
        {
            stepsPerThread = totalSteps;
            threadsNumber = 1;
        }

        var barrier = new Barrier(threadsNumber);
        var threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;

            threads[i] = new Thread(() =>
            {
                double threadA = a + threadIndex * stepsPerThread * step;
                double threadB;

                if (threadIndex == threadsNumber - 1)
                {
                    threadB = b;
                }
                else
                {
                    threadB = threadA + stepsPerThread * step;
                }

                double localResult = CalculateTrapezoidIntegral(threadA, threadB, function, step);

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

                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        barrier.Dispose();

        return totalResult;
    }

    // Оптимизированная многопоточная версия.
    // Использует локальные суммы в массиве вместо Interlocked.CompareExchange,
    // убирает Barrier — только Thread.Join в конце.
    public static double SolveOptimized(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 1)
            return SolveSequential(a, b, function, step);

        int totalSteps = (int)Math.Ceiling((b - a) / step);
        int stepsPerThread = totalSteps / threadsNumber;

        if (stepsPerThread < 1)
        {
            stepsPerThread = totalSteps;
            threadsNumber = 1;
        }

        double[] partialSums = new double[threadsNumber];
        var threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;

            threads[i] = new Thread(() =>
            {
                double threadA = a + threadIndex * stepsPerThread * step;
                double threadB;

                if (threadIndex == threadsNumber - 1)
                {
                    threadB = b;
                }
                else
                {
                    threadB = threadA + stepsPerThread * step;
                }

                partialSums[threadIndex] = CalculateTrapezoidIntegral(threadA, threadB, function, step);
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        double totalResult = 0.0;
        for (int i = 0; i < threadsNumber; i++)
            totalResult += partialSums[i];

        return totalResult;
    }

    // Однопоточная версия.
    // Не использует Thread, Task, Barrier, Interlocked — только чистый цикл.
    public static double SolveSequential(double a, double b, Func<double, double> function, double step)
    {
        return CalculateTrapezoidIntegral(a, b, function, step);
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