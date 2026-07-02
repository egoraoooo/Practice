namespace MetadataViewer;
using System;
using System.Reflection;
using System.Linq;

public class MetadataViewer
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Инструкция: MetadataInspector <path-to-assembly>");
            Console.WriteLine("Пример: MetadataInspector C:\\Projects\\FileSystemCommands.dll");
            return;
        }

        string assemblyPath = args[0];

        try
        {
            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine($"ОШИБКА: ФАЙЛ НЕ НАЙДЕН {assemblyPath}");
                return;
            }

            // Загружаем сборку
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            
            Console.WriteLine($"Сборка: {assembly.GetName().Name}");
            Console.WriteLine($"Путь: {assemblyPath}");

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                // Пропускаем автоматически сгенерированные типы
                if (type.IsCompilerGenerated())
                    continue;

                PrintTypeInfo(type);
            }
        }
        catch (BadImageFormatException)
        {
            Console.WriteLine($"ОШИБКА: {assemblyPath} НЕ ЯВЛЯЕТСЯ ДОПУСТИМОЙ СБОРКОЙ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
        }
    }

    static void PrintTypeInfo(Type type)
    {
        Console.WriteLine();
        Console.WriteLine($"Классы: {type.FullName}");
        
        // Базовый класс
        if (type.BaseType != null && type.BaseType != typeof(object))
        {
            Console.WriteLine($"  Базовый класс: {type.BaseType.Name}");
        }

        // Атрибуты класса
        PrintCustomAttributes(type.GetCustomAttributes(false), "  Атрибуты класса:");

        // Конструкторы
        ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (constructors.Length > 0)
        {
            Console.WriteLine("  Конструкторы:");
            foreach (ConstructorInfo constructor in constructors)
            {
                PrintMethodOrConstructorInfo(constructor, "    ");
            }
        }

        // Методы
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        if (methods.Length > 0)
        {
            Console.WriteLine("  Методы:");
            foreach (MethodInfo method in methods)
            {
                if (method.IsSpecialName)
                    continue;

                PrintMethodOrConstructorInfo(method, "    ");
            }
        }

        // Свойства
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        if (properties.Length > 0)
        {
            Console.WriteLine("  Свойства:");
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"    {property.PropertyType.Name} {property.Name}");
                PrintCustomAttributes(property.GetCustomAttributes(false), "      Атрибуты свойств:");
            }
        }

        Console.WriteLine(new string('-', 60));
    }

    static void PrintMethodOrConstructorInfo(MethodBase methodBase, string indent)
    {
        string name = methodBase.IsConstructor ? methodBase.DeclaringType.Name : methodBase.Name;
        
        // Модификаторы доступа
        string accessModifier = GetAccessModifier(methodBase);
        string staticModifier = methodBase.IsStatic ? "static " : "";
        
        // Параметры
        ParameterInfo[] parameters = methodBase.GetParameters();
        string paramsStr = string.Join(", ", parameters.Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));
        
        if (methodBase.IsConstructor)
        {
            Console.WriteLine($"{indent}{accessModifier} {staticModifier}{name}({paramsStr})");
        }
        else
        {
            MethodInfo method = (MethodInfo)methodBase;
            string returnType = GetTypeName(method.ReturnType);
            Console.WriteLine($"{indent}{accessModifier} {staticModifier}{returnType} {name}({paramsStr})");
        }

        // Атрибуты метода/конструктора
        PrintCustomAttributes(methodBase.GetCustomAttributes(false), $"{indent}  Атрибуты:");

        // Информация о параметрах
        if (parameters.Length > 0)
        {
            Console.WriteLine($"{indent}  Параметры:");
            foreach (ParameterInfo param in parameters)
            {
                Console.WriteLine($"{indent}    - {GetTypeName(param.ParameterType)} {param.Name}" +
                    (param.IsOptional ? $" = {param.DefaultValue ?? "null"}" : "") +
                    (param.HasDefaultValue ? $" [default]" : ""));
            }
        }
    }

    static void PrintCustomAttributes(object[] attributes, string header)
    {
        if (attributes.Length > 0)
        {
            Console.WriteLine($"  {header}");
            foreach (Attribute attr in attributes)
            {
                Console.WriteLine($"    [{attr.GetType().Name}]");
                
                // Выводим свойства атрибутов
                PropertyInfo[] attrProperties = attr.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in attrProperties)
                {
                    try
                    {
                        object value = prop.GetValue(attr);
                        if (value != null)
                        {
                            Console.WriteLine($"      {prop.Name} = {value}");
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    static string GetAccessModifier(MethodBase methodBase)
    {
        if (methodBase.IsPublic)
            return "public";
        if (methodBase.IsPrivate)
            return "private";
        if (methodBase.IsFamily)
            return "protected";
        if (methodBase.IsAssembly)
            return "internal";
        if (methodBase.IsFamilyOrAssembly)
            return "protected internal";
        
        return "unknown";
    }

    static string GetTypeName(Type type)
    {
        if (type == typeof(void))
            return "void";
        if (type == typeof(int))
            return "int";
        if (type == typeof(string))
            return "string";
        if (type == typeof(bool))
            return "bool";
        if (type == typeof(double))
            return "double";
        if (type == typeof(float))
            return "float";
        if (type == typeof(char))
            return "char";
        if (type == typeof(long))
            return "long";
        if (type == typeof(byte))
            return "byte";
        if (type == typeof(object))
            return "object";

        if (type.IsGenericType)
        {
            string genericTypeName = type.Name.Split('`')[0];
            string genericArgs = string.Join(", ", type.GetGenericArguments().Select(t => GetTypeName(t)));
            return $"{genericTypeName}<{genericArgs}>";
        }

        return type.Name;
    }
}

static class TypeExtensions
{
    public static bool IsCompilerGenerated(this Type type)
    {
        return type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Length > 0;
    }
}

