namespace CalculatorTests;
using System;
using Xunit;
using CalculatorEmitter;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        ICalculator calc = CalculatorEmitter.Create();
        Assert.Equal(8, calc.Add(5, 3));
    }

    [Fact]
    public void Minus_ReturnsDifference()
    {
        ICalculator calc = CalculatorEmitter.Create();
        Assert.Equal(2, calc.Minus(5, 3));
    }

    [Fact]
    public void Mul_ReturnsProduct()
    {
        ICalculator calc = CalculatorEmitter.Create();
        Assert.Equal(15, calc.Mul(5, 3));
    }

    [Fact]
    public void Div_ReturnsQuotient()
    {
        ICalculator calc = CalculatorEmitter.Create();
        Assert.Equal(2, calc.Div(6, 3));
    }

    [Fact]
    public void Div_ByZero_Throws()
    {
        ICalculator calc = CalculatorEmitter.Create();
        Assert.Throws<DivideByZeroException>(() => calc.Div(5, 0));
    }
}