namespace DefiniteIntegralTests;
using System;
using Xunit;
using DefiniteIntegral;
using System.Diagnostics;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_LinearFunction_SymmetricInterval_ReturnsZero()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Solve_SinFunction_SymmetricInterval_ReturnsZero()
    {
        var SIN = (double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Solve_LinearFunction_PositiveInterval_ReturnsValueFromAssignment()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);

        Assert.Equal(12.5, result, 1e-5);
    }

    [Fact]
    public void Solve_QuadraticFunction_ReturnsCorrectValue()
    {
        var square = (double x) => x * x;
        double result = DefiniteIntegral.Solve(0, 3, square, 1e-6, 4);
        Assert.Equal(9, result, 1e-5);
    }

    [Fact]
    public void Solve_CosFunction_ReturnsCorrectValue()
    {
        var cos = (double x) => Math.Cos(x);
        double result = DefiniteIntegral.Solve(0, Math.PI / 2, cos, 1e-6, 4);
        Assert.Equal(1, result, 1e-5);
    }

    [Fact]
    public void Solve_SingleThread_ReturnsCorrectValue()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 1);
        Assert.Equal(12.5, result, 1e-5);
    }

    [Fact]
    public void Solve_ManyThreads_ReturnsCorrectValue()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 10, X, 1e-6, 16);
        Assert.Equal(50, result, 1e-4);
    }

    [Fact]
    public void Solve_ExpFunction_ReturnsCorrectValue()
    {
        var exp = (double x) => Math.Exp(x);
        double result = DefiniteIntegral.Solve(0, 1, exp, 1e-6, 4);
        Assert.Equal(Math.E - 1, result, 1e-5);
    }

    [Fact]
    public void Solve_ConstantFunction_ReturnsCorrectValue()
    {
        var constant = (double x) => 2.0;
        double result = DefiniteIntegral.Solve(0, 5, constant, 1e-6, 8);
        Assert.Equal(10, result, 1e-5);
    }
    private const double Epsilon15 = 1e-4;
    private const double A15 = -100.0;
    private const double B15 = 100.0;
    private const double Step15 = 1e-4;
    private static readonly Func<double, double> SinFunc = Math.Sin;

    [Fact]
    public void SolveSequential_Sin_Accuracy()
    {
        double result = DefiniteIntegral.SolveSequential(A15, B15, SinFunc, Step15);
        double error = Math.Abs(result);
        Assert.True(error <= Epsilon15,
            $"Ошибка интегрирования ({error:E}) превышает допустимую ({Epsilon15})");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void Solve_Sin_Accuracy(int threadCount)
    {
        double result = DefiniteIntegral.Solve(A15, B15, SinFunc, Step15, threadCount);
        double error = Math.Abs(result);
        Assert.True(error <= Epsilon15,
            $"Ошибка ({error:E}) превышает допустимую ({Epsilon15}) для {threadCount} потоков");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void SolveOptimized_Sin_Accuracy(int threadCount)
    {
        double result = DefiniteIntegral.SolveOptimized(A15, B15, SinFunc, Step15, threadCount);
        double error = Math.Abs(result);
        Assert.True(error <= Epsilon15,
            $"Ошибка ({error:E}) превышает допустимую ({Epsilon15}) для оптимизированной версии");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void SolveAndSequential_ResultsMatch(int threadCount)
    {
        double sequential = DefiniteIntegral.SolveSequential(A15, B15, SinFunc, Step15);
        double parallel = DefiniteIntegral.Solve(A15, B15, SinFunc, Step15, threadCount);
        Assert.Equal(sequential, parallel, precision: 10);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void SolveOptimizedAndSequential_ResultsMatch(int threadCount)
    {
        double sequential = DefiniteIntegral.SolveSequential(A15, B15, SinFunc, Step15);
        double optimized = DefiniteIntegral.SolveOptimized(A15, B15, SinFunc, Step15, threadCount);
        Assert.Equal(sequential, optimized, precision: 10);
    }

    [Fact]
    public void SolveOptimized_IsFasterThanSequential()
    {
        int iterations = 5;

        // Warm-up
        for (int i = 0; i < 3; i++)
        {
            DefiniteIntegral.SolveSequential(A15, B15, SinFunc, Step15);
            DefiniteIntegral.SolveOptimized(A15, B15, SinFunc, Step15, 4);
        }

        var swSeq = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            DefiniteIntegral.SolveSequential(A15, B15, SinFunc, Step15);
        swSeq.Stop();

        var swOpt = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            DefiniteIntegral.SolveOptimized(A15, B15, SinFunc, Step15, 4);
        swOpt.Stop();

        double seqTime = swSeq.Elapsed.TotalMilliseconds / iterations;
        double optTime = swOpt.Elapsed.TotalMilliseconds / iterations;
        double improvement = (seqTime - optTime) / seqTime * 100.0;

        Assert.True(improvement >= 15.0,
            $"Многопоточная версия должна быть быстрее однопоточной на >= 15%. " +
            $"Фактическое ускорение: {improvement:F2}% (однопоточная: {seqTime:F2} мс, многопоточная: {optTime:F2} мс)");
    }
}