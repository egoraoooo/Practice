using System;
using System.Reflection;

namespace MetadataViewer
{
    public static class MetadataViewer
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ОШИБКА: НЕ УКАЗАН ПУТЬ К БИБЛИОТЕКЕ.");
                Console.WriteLine("Использование: MetadataViewer <путь_к_DLL>");
                return;
            }

            string assemblyPath = args[0];

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                PrintAssemblyMetadata(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА ЗАГРУЗКИ СБОРКИ: {ex.Message}");
            }
        }

        public static void PrintAssemblyMetadata(Assembly assembly)
        {
            Console.WriteLine($"Сборка: {assembly.FullName}");

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                Console.WriteLine();
                Console.WriteLine($"Класс: {type.FullName}");

                // Атрибуты класса
                object[] attributes = type.GetCustomAttributes(false);
                if (attributes.Length > 0)
                {
                    Console.WriteLine("  Атрибуты:");
                    foreach (var attr in attributes)
                    {
                        Console.WriteLine($"    - {attr.GetType().Name}");
                        var properties = attr.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(attr);
                            Console.WriteLine($"      {prop.Name}: {value}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("  Атрибуты: ОТСУТСТВУЮТ");
                }

                // Конструкторы
                ConstructorInfo[] constructors = type.GetConstructors();
                if (constructors.Length > 0)
                {
                    Console.WriteLine("  Конструкторы:");
                    foreach (var ctor in constructors)
                    {
                        Console.WriteLine($"    - {ctor.Name}");
                        ParameterInfo[] parameters = ctor.GetParameters();
                        if (parameters.Length > 0)
                        {
                            Console.WriteLine("      Параметры:");
                            foreach (var param in parameters)
                            {
                                Console.WriteLine($"        {param.ParameterType.Name} {param.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("      Параметры: ОТСУТСТВУЮТ");
                        }
                    }
                }

                // Методы
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methods.Length > 0)
                {
                    Console.WriteLine("  Методы:");
                    foreach (var method in methods)
                    {
                        Console.WriteLine($"    - {method.Name}");
                        Console.WriteLine($"      Возвращаемый тип: {method.ReturnType.Name}");

                        // Атрибуты метода
                        object[] methodAttrs = method.GetCustomAttributes(false);
                        if (methodAttrs.Length > 0)
                        {
                            Console.WriteLine("      Атрибуты:");
                            foreach (var attr in methodAttrs)
                            {
                                Console.WriteLine($"        - {attr.GetType().Name}");
                                var properties = attr.GetType().GetProperties();
                                foreach (var prop in properties)
                                {
                                    var value = prop.GetValue(attr);
                                    Console.WriteLine($"          {prop.Name}: {value}");
                                }
                            }
                        }

                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length > 0)
                        {
                            Console.WriteLine("      Параметры:");
                            foreach (var param in parameters)
                            {
                                Console.WriteLine($"        {param.ParameterType.Name} {param.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("      Параметры: ОТСУТСТВУЮТ");
                        }
                    }
                }
            }
        }
    }
}