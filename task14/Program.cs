namespace Program;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DefiniteIntegral;
using ScottPlot;

class Program
{
    static void Main(string[] args)
    {
        double a = -100.0;
        double b = 100.0;
        double requiredAccuracy = 1e-4;
        Func<double, double> function = Math.Sin;
        double exactValue = 0.0;

        Console.WriteLine("=== Задача 15. Оптимизация многопоточного интегрирования ===");
        Console.WriteLine($"Функция: sin(x), отрезок: [{a}, {b}]");
        Console.WriteLine($"Требуемая точность: {requiredAccuracy}");
        Console.WriteLine();

        Console.WriteLine("--- Шаг 1: Поиск минимального шага, обеспечивающего точность ---");
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        double optimalStep = 0;

        foreach (var step in steps)
        {
            double result = DefiniteIntegral.SolveSequential(a, b, function, step);
            double error = Math.Abs(result - exactValue);
            Console.WriteLine($"Шаг {step,8:E1}: результат = {result,15:E}, ошибка = {error,15:E}");

            if (optimalStep == 0 && error <= requiredAccuracy)
            {
                optimalStep = step;
                Console.WriteLine($"  ^^ Первый шаг, обеспечивающий точность {requiredAccuracy}: {step}");
            }
        }

        if (optimalStep == 0)
        {
            Console.WriteLine("Ошибка: ни один шаг не обеспечил требуемую точность!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($">>> Выбран оптимальный шаг: {optimalStep}");
        Console.WriteLine($">>> Число интервалов: {(int)Math.Ceiling((b - a) / optimalStep):N0}");
        Console.WriteLine();

        Console.WriteLine("--- Шаг 2: Поиск оптимального количества потоков ---");
        int[] threadCounts = { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 20, 24, 32 };
        int warmupIterations = 3;
        int measureIterations = 10;

        Console.WriteLine("Разогрев (warm-up)...");
        for (int i = 0; i < warmupIterations; i++)
        {
            DefiniteIntegral.SolveSequential(a, b, function, optimalStep);
            DefiniteIntegral.Solve(a, b, function, optimalStep, 4);
        }

        var results = new System.Collections.Generic.List<(int Threads, double AvgTimeMs)>();
        string csvData = "ThreadCount,AverageTimeMs";

        foreach (var tc in threadCounts)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < measureIterations; i++)
            {
                DefiniteIntegral.Solve(a, b, function, optimalStep, tc);
            }
            sw.Stop();
            double avgTime = sw.Elapsed.TotalMilliseconds / measureIterations;
            results.Add((tc, avgTime));
            csvData += $"{tc},{avgTime:F4}";
            Console.WriteLine($"Потоков: {tc,2}, среднее время: {avgTime,10:F4} мс");
        }

        var bestMultiThreaded = results
            .Where(r => r.Threads > 1)
            .OrderBy(r => r.AvgTimeMs)
            .First();

        Console.WriteLine();
        Console.WriteLine($">>> Оптимальное количество потоков: {bestMultiThreaded.Threads}");
        Console.WriteLine($">>> Минимальное время (многопоточное): {bestMultiThreaded.AvgTimeMs:F4} мс");
        Console.WriteLine();

        Console.WriteLine("--- Шаг 3: Сравнение с однопоточной версией ---");

        var swSingle = Stopwatch.StartNew();
        for (int i = 0; i < measureIterations; i++)
        {
            DefiniteIntegral.SolveSequential(a, b, function, optimalStep);
        }
        swSingle.Stop();
        double singleTime = swSingle.Elapsed.TotalMilliseconds / measureIterations;

        double multiTime = bestMultiThreaded.AvgTimeMs;
        double improvement = (singleTime - multiTime) / singleTime * 100.0;

        Console.WriteLine($"Однопоточная версия (SolveSequential, без Thread/Barrier): {singleTime:F4} мс");
        Console.WriteLine($"Многопоточная версия ({bestMultiThreaded.Threads} потоков, исходная):   {multiTime:F4} мс");
        Console.WriteLine($"Ускорение: {improvement:F2}%");
        Console.WriteLine();

        bool usedOptimized = false;
        double optimizedTime = 0;
        int optimizedThreads = bestMultiThreaded.Threads;

        if (improvement < 15.0)
        {
            Console.WriteLine("--- Шаг 4: Оптимизация (ускорение < 15%) ---");
            Console.WriteLine("Применяем оптимизированную версию (локальные суммы, без Interlocked/Barrier)...");
            Console.WriteLine();

            for (int i = 0; i < warmupIterations; i++)
            {
                DefiniteIntegral.SolveOptimized(a, b, function, optimalStep, optimizedThreads);
            }

            var resultsOpt = new System.Collections.Generic.List<(int Threads, double AvgTimeMs)>();
            foreach (var tc in threadCounts)
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < measureIterations; i++)
                {
                    DefiniteIntegral.SolveOptimized(a, b, function, optimalStep, tc);
                }
                sw.Stop();
                double avgTime = sw.Elapsed.TotalMilliseconds / measureIterations;
                resultsOpt.Add((tc, avgTime));
                Console.WriteLine($"[Оптимизировано] Потоков: {tc,2}, среднее время: {avgTime,10:F4} мс");
            }

            var bestOpt = resultsOpt
                .Where(r => r.Threads > 1)
                .OrderBy(r => r.AvgTimeMs)
                .First();

            optimizedTime = bestOpt.AvgTimeMs;
            optimizedThreads = bestOpt.Threads;

            double optImprovement = (singleTime - optimizedTime) / singleTime * 100.0;

            Console.WriteLine();
            Console.WriteLine($">>> Оптимизированное количество потоков: {optimizedThreads}");
            Console.WriteLine($">>> Оптимизированное время: {optimizedTime:F4} мс");
            Console.WriteLine($">>> Ускорение после оптимизации: {optImprovement:F2}%");

            multiTime = optimizedTime;
            improvement = optImprovement;
            usedOptimized = true;
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("--- Шаг 4: Оптимизация не требуется ---");
            Console.WriteLine($"Ускорение {improvement:F2}% уже >= 15%");
            Console.WriteLine();
        }

        File.WriteAllText("benchmark_data.csv", csvData);
        Console.WriteLine(">>> Данные для графиков сохранены: benchmark_data.csv");
        Console.WriteLine();

        // ========== ПОСТРОЕНИЕ ГРАФИКА ЧЕРЕЗ SCOTTPLOT (без Notebook) ==========
        Console.WriteLine("--- Построение графика через ScottPlot ---");
        try
        {
            var plot = new Plot();
            plot.Axes.Title.Label.FontName = "DejaVu Sans";
            plot.Axes.Bottom.Label.FontName = "DejaVu Sans";
            plot.Axes.Left.Label.FontName = "DejaVu Sans";
            var threadCountsArray = results.Select(r => (double)r.Threads).ToArray();
            var timesArray = results.Select(r => r.AvgTimeMs).ToArray();

            // Задание: OX — время, OY — количество потоков
            var scatter = plot.Add.Scatter(timesArray, threadCountsArray);
            scatter.LineWidth = 2;
            scatter.MarkerSize = 8;

            int minIndex = 0;
            double minTime = timesArray[0];
            for (int i = 1; i < timesArray.Length; i++)
            {
                if (timesArray[i] < minTime)
                {
                    minTime = timesArray[i];
                    minIndex = i;
                }
            }

            plot.XLabel("Время выполнения, мс");
            plot.YLabel("Количество потоков");
            plot.Title("Зависимость количества потоков от времени выполнения\n(усреднённые замеры)");

            plot.SavePng("thread_performance.png", 800, 600);
            Console.WriteLine(">>> График сохранён: thread_performance.png");
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> Не удалось построить график: {ex.Message}");
        }
        Console.WriteLine();

        // Сохранение итогового отчёта
        string report = $@"Результаты оптимизации вычисления определённого интеграла
================================================================================
Функция: sin(x)
Отрезок интегрирования: [{a}, {b}]
Аналитическое значение интеграла: {exactValue}
Требуемая точность: {requiredAccuracy}

1. ОПТИМАЛЬНЫЙ ШАГ ИНТЕГРИРОВАНИЯ
Значение: {optimalStep}
Пояснение: Минимальный (самый крупный) шаг из предложенного ряда,
            при котором ошибка не превышает {requiredAccuracy}.
            Число интервалов: {(int)Math.Ceiling((b - a) / optimalStep):N0}.

2. ОПТИМАЛЬНОЕ КОЛИЧЕСТВО ПОТОКОВ
Значение: {optimizedThreads}
Пояснение: При данном количестве потоков достигается минимальное время
            выполнения функции Solve.
            Среднее время: {multiTime:F4} мс.

3. СРАВНЕНИЕ С ОДНОПОТОЧНОЙ ВЕРСИЕЙ
Однопоточная версия (SolveSequential, без Thread/Barrier/Interlocked):
    Время: {singleTime:F4} мс
    Пояснение: Чисто последовательный цикл без многопоточных примитивов.

Многопоточная версия ({optimizedThreads} потоков{(usedOptimized ? ", оптимизированная" : ", исходная")}):
    Время: {multiTime:F4} мс
    Пояснение: Время включает создание потоков и ожидание их завершения.

Разница (ускорение): {improvement:F2}%
Пояснение: Многопоточная версия {(improvement >= 15 ? "быстрее" : "медленнее")} однопоточной.
            {(improvement >= 15 ? "Требование >= 15% ВЫПОЛНЕНО." : "Требование >= 15% НЕ ВЫПОЛНЕНО.")}

4. МЕТОДИКА ЗАМЕРОВ
- Измеритель времени: System.Diagnostics.Stopwatch
- Разогрев (warm-up): {warmupIterations} итерации
- Число замеров для усреднения: {measureIterations}
- Результат: среднее арифметическое

================================================================================
Дата создания отчёта: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
";

        File.WriteAllText("results.txt", report);
        Console.WriteLine(">>> Итоговый отчёт сохранён: results.txt");
        Console.WriteLine();
        Console.WriteLine("=== Работа завершена ===");
    }
}
