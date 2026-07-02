namespace task07tests;
using Xunit;
using System;
using System.Linq;
using System.Reflection;
using task07;

public class AttributeReflectionTests
{
    [Fact]
    public void Class_HasDisplayNameAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Пример класса", attribute.DisplayName);
    }

    [Fact]
    public void Method_HasDisplayNameAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");
        var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Тестовый метод", attribute.DisplayName);
    }

    [Fact]
    public void Property_HasDisplayNameAttribute()
    {
        var prop = typeof(SampleClass).GetProperty("Number");
        var attribute = prop.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Числовое свойство", attribute.DisplayName);
    }

    [Fact]
    public void Class_HasVersionAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<VersionAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(1, attribute.Major);
        Assert.Equal(0, attribute.Minor);
    }

    [Fact]
    public void Method_DoesNotHaveVersionAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");
        var attribute = method.GetCustomAttribute<VersionAttribute>();
        Assert.Null(attribute);
    }

    [Fact]
    public void Property_DoesNotHaveVersionAttribute()
    {
        var prop = typeof(SampleClass).GetProperty("Number");
        var attribute = prop.GetCustomAttribute<VersionAttribute>();
        Assert.Null(attribute);
    }

    [Fact]
    public void DisplayNameAttribute_CanBeAppliedToClass()
    {
        var attributeUsage = typeof(DisplayNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void DisplayNameAttribute_CanBeAppliedToMethod()
    {
        var attributeUsage = typeof(DisplayNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Method));
    }

    [Fact]
    public void DisplayNameAttribute_CanBeAppliedToProperty()
    {
        var attributeUsage = typeof(DisplayNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Property));
    }

    [Fact]
    public void VersionAttribute_CanBeAppliedToClass()
    {
        var attributeUsage = typeof(VersionAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void VersionAttribute_CannotBeAppliedToMethod()
    {
        var attributeUsage = typeof(VersionAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.False(attributeUsage.ValidOn.HasFlag(AttributeTargets.Method));
    }

    [Fact]
    public void ReflectionHelper_Exists()
    {
        var type = typeof(ReflectionHelper);
        Assert.NotNull(type);
        Assert.True(type.IsAbstract);
        Assert.True(type.IsSealed);
    }

    [Fact]
    public void ReflectionHelper_HasPrintTypeInfoMethod()
    {
        var method = typeof(ReflectionHelper).GetMethod("PrintTypeInfo");
        Assert.NotNull(method);
        Assert.True(method.IsPublic);
        Assert.True(method.IsStatic);
    }

    [Fact]
    public void DisplayNameAttribute_IsInherited()
    {
        var attributeUsage = typeof(DisplayNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.Inherited);
    }

    [Fact]
    public void VersionAttribute_Constructor_Works()
    {
        var attribute = new VersionAttribute(2, 1);
        Assert.Equal(2, attribute.Major);
        Assert.Equal(1, attribute.Minor);
    }

    [Fact]
    public void DisplayNameAttribute_Constructor_Works()
    {
        var attribute = new DisplayNameAttribute("Тест");
        Assert.Equal("Тест", attribute.DisplayName);
    }

    [Fact]
    public void VersionAttribute_AllowsZeroVersion()
    {
        var attribute = new VersionAttribute(0, 0);
        Assert.Equal(0, attribute.Major);
        Assert.Equal(0, attribute.Minor);
    }

    [Fact]
    public void VersionAttribute_AllowsLargeNumbers()
    {
        var attribute = new VersionAttribute(999, 999);
        Assert.Equal(999, attribute.Major);
        Assert.Equal(999, attribute.Minor);
    }

    [Fact]
    public void DisplayNameAttribute_AllowsEmptyString()
    {
        var attribute = new DisplayNameAttribute("");
        Assert.Equal("", attribute.DisplayName);
    }

    [Fact]
    public void SampleClass_HasNumberProperty()
    {
        var prop = typeof(SampleClass).GetProperty("Number");
        Assert.NotNull(prop);
        Assert.Equal(typeof(int), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void SampleClass_HasTestMethod()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");
        Assert.NotNull(method);
        Assert.Equal(typeof(void), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
}
