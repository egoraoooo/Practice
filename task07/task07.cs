namespace task07;
using System;
using System.Reflection;
using System.Linq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayname)
    {
        DisplayName = displayname;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() {}
}

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        PrintClassInfo(type);
        PrintPropertiesInfo(type);
        PrintMethodsInfo(type);
    }

    private static void PrintClassInfo(Type type)
    {
        var displayname = type.GetCustomAttribute<DisplayNameAttribute>();
        var version = type.GetCustomAttribute<VersionAttribute>();
        Console.WriteLine("\n=== ИНФОРМАЦИЯ О КЛАССЕ ===");
        if (displayname != null)
        {
            Console.WriteLine("Отображаемое имя: " + displayname.DisplayName);
        }
        else
        {
            Console.WriteLine("Отображаемое имя: ОТСУТСТВУЕТ");
        }
    
        if (version != null)
        {
            Console.WriteLine($"Версия: {version.Major}.{version.Minor}");
        }
        else
        {
            Console.WriteLine("Версия: ОТСУТСТВУЕТ");
        }
    }

    private static void PrintPropertiesInfo(Type type)
    {
        var properties = type.GetProperties();
        Console.WriteLine("\n=== СВОЙСТВА ===");
        
        foreach (var property in properties)
        {
            var displayname = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayname != null)
            {
                Console.WriteLine($"{displayname.DisplayName} ({property.Name})");
            }
        }
    }

    private static void PrintMethodsInfo(Type type)
    {
        var methods = type.GetMethods().Where(m => m.DeclaringType == type);
        Console.WriteLine("\n=== МЕТОДЫ ===");
        
        foreach (var method in methods)
        {
            var displayname = method.GetCustomAttribute<DisplayNameAttribute>();
            if (displayname != null)
            {
                Console.WriteLine($"{displayname.DisplayName} ({method.Name})");
            }
        }
    }
}
