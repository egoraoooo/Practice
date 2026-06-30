namespace task05tests;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using task05;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
    private void PrivateMethod() {}
    public string MethodWithParams(int param1, string param2)
    {
        return $"{param1} {param2}";
    }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetMethodParams_ReturnsParamsWithReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methodparams = analyzer.GetMethodParams("MethodWithParams").ToList();

        Assert.Contains("param1 (Int32)", methodparams);
        Assert.Contains("param2 (String)", methodparams);
        Assert.Contains("Return: String", methodparams);
    }

    [Fact]
    public void GetMethodParams_IfMethodIsNotExist_ReturnsEmptyCollection()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methodparams = analyzer.GetMethodParams("Buckshot");

        Assert.Empty(methodparams);
    }

    [Fact]
    public void GetMethodParams_IfMethodIsPrivate_ReturnsEmptyCollection()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methodparams = analyzer.GetMethodParams("PrivateMethod");

        Assert.Empty(methodparams);
    }

    [Fact]
    public void GetProperties_ReturnsAllProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var prop = analyzer.GetProperties();

        Assert.Contains("Property", prop);
    }

    [Fact]
    public void HasAttribute_HaveAttribute_ReturnsTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));

        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_HaveAttribute_ReturnsFalse()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }
}
