namespace DefiniteIntegralTests;
using System;
using Xunit;
using DefiniteIntegral;


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
}