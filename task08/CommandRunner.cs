namespace CommandRunner;
using System.Reflection;
using CommandLib;

class CommandRunner
{
    static void Main(string[] args)
    {
        // Определяем путь к DLL
        string dllpath = Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            @"..\..\..\..\FileSystemCommands\bin\Debug\net8.0\FileSystemCommands.dll"
        ));
        
        Console.WriteLine($"Загрузка сборки из: {dllpath}");
        
        try
        {
            // Загружаем сборку
            Assembly assembly = Assembly.LoadFrom(dllpath);
            
            // Находим все команды
            Type[] commandtypes = assembly.GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();
            
            if (commandtypes.Length == 0)
            {
                Console.WriteLine("Команды не найдены");
                return;
            }
            
            // Показываем список команд
            Console.WriteLine("\nДоступные команды:");
            for (int i = 0; i < commandtypes.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {commandtypes[i].Name}");
            }
            
            // Выбор команды
            Console.Write("\nВыберите команду: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > commandtypes.Length)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }
            
            Type selectedtype = commandtypes[choice - 1];
            ConstructorInfo constructor = selectedtype.GetConstructors()[0];
            ParameterInfo[] parameters = constructor.GetParameters();
            
            // Получаем параметры
            object[] constructorargs = new object[parameters.Length];
            Console.WriteLine($"\nПараметры для {selectedtype.Name}:");
            
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write($"{parameters[i].Name}: ");
                constructorargs[i] = Console.ReadLine();
            }
            
            // Создаём и выполняем команду
            ICommand command = (ICommand)Activator.CreateInstance(selectedtype, constructorargs);
            
            Console.WriteLine($"\n{new string('-', 40)}");
            command.Execute();
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("\nГотово!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"ОШИБКА: DLL НЕ НАЙДЕНА ПО ПУТИ '{dllpath}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
        }
        
        Console.ReadKey();
    }
}