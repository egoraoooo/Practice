namespace PluginSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// Загрузчик плагинов
public class PluginManager
{
    private readonly Dictionary<string, Type> _pluginTypes = new();
    private readonly Dictionary<string, List<string>> _dependencies = new();
    
    public List<ICommand> LoadPlugins(string pluginsDirectory)
    {
        if (!Directory.Exists(pluginsDirectory))
            throw new DirectoryNotFoundException($"Директория с плагинами не найдена: {pluginsDirectory}");
        
        var assemblies = LoadAssemblies(pluginsDirectory);
        var pluginTypes = FindPluginTypes(assemblies);
        
        if (pluginTypes.Count == 0)
            return new List<ICommand>();
        
        var executionOrder = ResolveDependencies(pluginTypes);
        return CreatePluginInstances(executionOrder);
    }
    
    // Загрузить сборки из директории
    private List<Assembly> LoadAssemblies(string pluginsDirectory)
    {
        var assemblies = new List<Assembly>();
        var dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll");
        
        foreach (var dllFile in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllFile);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке сборки {dllFile}: {ex.Message}");
            }
        }
        
        return assemblies;
    }
    
    // Найти типы с атрибутом PluginLoad
    private List<Type> FindPluginTypes(List<Assembly> assemblies)
    {
        _pluginTypes.Clear();
        _dependencies.Clear();
        
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null 
                                && typeof(ICommand).IsAssignableFrom(t)
                                && !t.IsAbstract
                                && t.GetConstructor(Type.EmptyTypes) != null);
                
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    
                    if (_pluginTypes.ContainsKey(attr.Name))
                    {
                        throw new InvalidOperationException(
                            $"Найден дубликат плагина с именем '{attr.Name}'");
                    }
                    
                    _pluginTypes[attr.Name] = type;
                    _dependencies[attr.Name] = new List<string>(attr.Dependencies ?? Array.Empty<string>());
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Обрабатываем типы, которые удалось загрузить
                foreach (var type in ex.Types.Where(t => t != null))
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attr != null && typeof(ICommand).IsAssignableFrom(type))
                    {
                        _pluginTypes[attr.Name] = type;
                        _dependencies[attr.Name] = new List<string>(attr.Dependencies ?? Array.Empty<string>());
                    }
                }
            }
        }
        
        return _pluginTypes.Keys.Select(name => _pluginTypes[name]).ToList();
    }
    
    // Разрешить зависимости между плагинами с помощью топологической сортировки
    private List<Type> ResolveDependencies(List<Type> pluginTypes)
    {
        // Проверяем, что все зависимости существуют
        foreach (var depList in _dependencies.Values)
        {
            foreach (var dep in depList)
            {
                if (!_pluginTypes.ContainsKey(dep))
                {
                    throw new InvalidOperationException(
                        $"Не найдена зависимость '{dep}'. Проверьте наличие плагина.");
                }
            }
        }
        
        // Топологическая сортировка
        var inDegree = new Dictionary<string, int>();
        var adjacencyList = new Dictionary<string, List<string>>();
        
        foreach (var pluginName in _pluginTypes.Keys)
        {
            inDegree[pluginName] = 0;
            adjacencyList[pluginName] = new List<string>();
        }
        
        // Построение графа зависимостей
        foreach (var (pluginName, dependencies) in _dependencies)
        {
            foreach (var dependency in dependencies)
            {
                adjacencyList[dependency].Add(pluginName);
                inDegree[pluginName]++;
            }
        }
        
        // Поиск плагинов без зависимостей
        var queue = new Queue<string>();
        foreach (var (pluginName, degree) in inDegree)
        {
            if (degree == 0)
                queue.Enqueue(pluginName);
        }
        
        var sortedPlugins = new List<Type>();
        
        // Топологическая сортировка
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sortedPlugins.Add(_pluginTypes[current]);
            
            foreach (var neighbor in adjacencyList[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }
        
        // Проверка на циклические зависимости
        if (sortedPlugins.Count != _pluginTypes.Count)
        {
            throw new InvalidOperationException(
                "Обнаружена циклическая зависимость между плагинами.");
        }
        
        return sortedPlugins;
    }
    // Создать экземпляры плагинов
    private List<ICommand> CreatePluginInstances(List<Type> orderedTypes)
    {
        var plugins = new List<ICommand>();
        
        foreach (var type in orderedTypes)
        {
            try
            {
                var instance = Activator.CreateInstance(type) as ICommand;
                if (instance != null)
                {
                    plugins.Add(instance);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Ошибка при создании экземпляра плагина {type.Name}: {ex.Message}", ex);
            }
        }
        
        return plugins;
    }
    
    // Выполнить все загруженные плагин
    public void ExecutePlugins(IEnumerable<ICommand> plugins)
    {
        foreach (var plugin in plugins)
        {
            try
            {
                plugin.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении плагина {plugin.GetType().Name}: {ex.Message}");
                throw;
            }
        }
    }
}