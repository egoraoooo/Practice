namespace task09;
using System;
using System.Reflection;
using System.Linq;
using CommandLib;

public class MetadataViewer
{
static void Main(string[] args)
    {
        Console.WriteLine("=== ПРОСМОТР МЕТАДАННЫХ БИБЛИОТЕКИ ===\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Использование: MetadataViewer.exe <путь_к_dll>");
            Console.WriteLine("Пример: MetadataViewer.exe ..\\..\\FileSystemCommands.dll");
            return;
        }

        string dllPath = args[0];
        
        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"ОШИБКА: Файл не найден: {dllPath}");
            return;
        }

        try
        {
            // Загружаем сборку
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"Загружена сборка: {assembly.FullName}\n");

            // Получаем все публичные типы
            Type[] types = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic)
                .OrderBy(t => t.Name)
                .ToArray();

            if (types.Length == 0)
            {
                Console.WriteLine("Публичные классы не найдены.");
                return;
            }

            Console.WriteLine($"Найдено классов: {types.Length}\n");
            Console.WriteLine(new string('=', 80));

            // Выводим информацию по каждому классу
            foreach (Type type in types)
            {
                PrintClassInfo(type);
                Console.WriteLine(new string('-', 80));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
            }
        }
    }

    static void PrintClassInfo(Type type)
    {
        Console.WriteLine($"\nКЛАСС: {type.Name}");
        Console.WriteLine($"Полное имя: {type.FullName}");
        Console.WriteLine($"Пространство имен: {type.Namespace ?? "Нет"}");
        Console.WriteLine($"Является ли класс абстрактным: {(type.IsAbstract ? "Да" : "Нет")}");
        Console.WriteLine($"Является ли класс статическим: {(type.IsAbstract && type.IsSealed ? "Да" : "Нет")}");

        // Выводим атрибуты класса
        PrintAttributes(type);

        // Выводим конструкторы
        PrintConstructors(type);

        // Выводим свойства
        PrintProperties(type);

        // Выводим методы
        PrintMethods(type);
    }

    static void PrintAttributes(Type type)
    {
        var attributes = type.GetCustomAttributes();
        Console.WriteLine("\nАТРИБУТЫ КЛАССА:");
        
        if (!attributes.Any())
        {
            Console.WriteLine("  - Атрибуты отсутствуют");
            return;
        }

        foreach (var attr in attributes)
        {
            Console.WriteLine($"  - {attr.GetType().Name}");
            
            // Проверяем только атрибуты из задания 07
            if (attr is DisplayNameAttribute displayName)
            {
                Console.WriteLine($"      Отображаемое имя: {displayName.DisplayName}");
            }
            else if (attr is VersionAttribute version)
            {
                Console.WriteLine($"      Версия: {version.Major}.{version.Minor}");
            }
        }
    }

    static void PrintConstructors(Type type)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        Console.WriteLine("\nКОНСТРУКТОРЫ:");
        
        if (constructors.Length == 0)
        {
            Console.WriteLine("  - Конструкторы отсутствуют");
            return;
        }

        foreach (var ctor in constructors)
        {
            // Атрибуты конструктора
            var ctorAttrs = ctor.GetCustomAttributes();
            if (ctorAttrs.Any())
            {
                Console.WriteLine("  Атрибуты конструктора:");
                foreach (var attr in ctorAttrs)
                {
                    Console.WriteLine($"    - {attr.GetType().Name}");
                    if (attr is DisplayNameAttribute displayName)
                    {
                        Console.WriteLine($"        Отображаемое имя: {displayName.DisplayName}");
                    }
                }
            }

            var parameters = ctor.GetParameters();
            if (parameters.Length == 0)
            {
                Console.WriteLine($"  - {type.Name}()");
            }
            else
            {
                string paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {type.Name}({paramList})");
                
                // Выводим информацию о параметрах
                Console.WriteLine("    Параметры:");
                foreach (var param in parameters)
                {
                    Console.WriteLine($"      - Тип: {param.ParameterType.Name}, Имя: {param.Name}");
                }
            }
        }
    }

    static void PrintProperties(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        Console.WriteLine("\nСВОЙСТВА:");
        
        if (properties.Length == 0)
        {
            Console.WriteLine("  - Свойства отсутствуют");
            return;
        }

        foreach (var property in properties)
        {
            Console.WriteLine($"  - {property.PropertyType.Name} {property.Name}");
            
            // Атрибуты свойства
            var propAttrs = property.GetCustomAttributes();
            if (propAttrs.Any())
            {
                Console.WriteLine("    Атрибуты свойства:");
                foreach (var attr in propAttrs)
                {
                    Console.WriteLine($"      - {attr.GetType().Name}");
                    if (attr is DisplayNameAttribute displayName)
                    {
                        Console.WriteLine($"          Отображаемое имя: {displayName.DisplayName}");
                    }
                }
            }
        }
    }

    static void PrintMethods(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName) // Исключаем свойства и события
            .OrderBy(m => m.Name)
            .ToArray();

        Console.WriteLine("\nМЕТОДЫ:");
        
        if (methods.Length == 0)
        {
            Console.WriteLine("  - Публичные методы отсутствуют");
            return;
        }

        foreach (var method in methods)
        {
            Console.WriteLine($"\n  МЕТОД: {method.Name}");
            
            // Атрибуты метода
            var methodAttrs = method.GetCustomAttributes();
            if (methodAttrs.Any())
            {
                Console.WriteLine("    Атрибуты метода:");
                foreach (var attr in methodAttrs)
                {
                    Console.WriteLine($"      - {attr.GetType().Name}");
                    if (attr is DisplayNameAttribute displayName)
                    {
                        Console.WriteLine($"          Отображаемое имя: {displayName.DisplayName}");
                    }
                    else if (attr is VersionAttribute version)
                    {
                        Console.WriteLine($"          Версия: {version.Major}.{version.Minor}");
                    }
                }
            }

            // Параметры метода
            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                Console.WriteLine("    Параметры:");
                foreach (var param in parameters)
                {
                    Console.WriteLine($"      - Тип: {param.ParameterType.Name}, Имя: {param.Name}");
                }
            }
            else
            {
                Console.WriteLine("    Параметры: нет");
            }

            // Возвращаемый тип
            Console.WriteLine($"    Возвращаемый тип: {method.ReturnType.Name}");
            
            // Проверяем, является ли метод виртуальным/абстрактным/переопределенным
            if (method.IsVirtual)
            {
                Console.WriteLine($"    Модификатор: {(method.IsAbstract ? "абстрактный" : "виртуальный")}");
            }
            if (method.IsStatic)
            {
                Console.WriteLine("    Модификатор: статический");
            }
        }
    }
}
