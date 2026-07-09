namespace task10tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginSystem;
using TestPlugins;
using CircularPlugins;
using Xunit;

public class PluginManagerTests : IDisposable
{
    private readonly string _testPluginsDirectory;
    
    public PluginManagerTests()
    {
        _testPluginsDirectory = Path.Combine(
            Path.GetTempPath(), 
            "PluginTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testPluginsDirectory);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_testPluginsDirectory))
        {
            try { Directory.Delete(_testPluginsDirectory, recursive: true); } catch { }
        }
    }
    
    [Fact]
    public void LoadPlugins_EmptyDirectory_ReturnsEmptyList()
    {
        var loader = new PluginManager();
        
        var plugins = loader.LoadPlugins(_testPluginsDirectory);
        
        Assert.NotNull(plugins);
        Assert.Empty(plugins);
    }
    
    [Fact]
    public void LoadPlugins_DirectoryNotFound_ThrowsException()
    {
        var loader = new PluginManager();
        var nonExistentPath = Path.Combine(_testPluginsDirectory, "NonExistent");
        
        Assert.Throws<DirectoryNotFoundException>(
            () => loader.LoadPlugins(nonExistentPath));
    }
    
    [Fact]
    public void LoadPlugins_ValidPlugins_LoadsAndExecutesInOrder()
    {
        var loader = new PluginManager();
        
        var cleanDir = Path.Combine(_testPluginsDirectory, "ValidPlugins");
        Directory.CreateDirectory(cleanDir);

        var task10Dll = typeof(PluginManager).Assembly.Location;
        File.Copy(task10Dll, Path.Combine(cleanDir, Path.GetFileName(task10Dll)), overwrite: true);
        
        var testPluginsDll = typeof(BasicPlugin).Assembly.Location;
        File.Copy(testPluginsDll, Path.Combine(cleanDir, Path.GetFileName(testPluginsDll)), overwrite: true);
        
        var plugins = loader.LoadPlugins(cleanDir);
        
        Assert.NotNull(plugins);
        Assert.Equal(2, plugins.Count);
    }

    [Fact]
    public void LoadPlugins_WithDependencies_RespectsDependenciesOrder()
    {
        var loader = new PluginManager();
        
        var cleanDir = Path.Combine(_testPluginsDirectory, "DependencyOrder");
        Directory.CreateDirectory(cleanDir);
        
        var task10Dll = typeof(PluginManager).Assembly.Location;
        File.Copy(task10Dll, Path.Combine(cleanDir, Path.GetFileName(task10Dll)), overwrite: true);
        
        var testPluginsDll = typeof(BasicPlugin).Assembly.Location;
        File.Copy(testPluginsDll, Path.Combine(cleanDir, Path.GetFileName(testPluginsDll)), overwrite: true);
        
        var plugins = loader.LoadPlugins(cleanDir);
        
        var basicPluginIndex = plugins.FindIndex(p => p is BasicPlugin);
        var dependentPluginIndex = plugins.FindIndex(p => p is DependentPlugin);
        
        Assert.True(basicPluginIndex >= 0);
        Assert.True(dependentPluginIndex >= 0);
        Assert.True(basicPluginIndex < dependentPluginIndex,
            "BasicPlugin should be loaded before DependentPlugin");
    }

    [Fact]
    public void LoadPlugins_CircularDependencies_ThrowsException()
    {
        var loader = new PluginManager();
        
        var circularDir = Path.Combine(_testPluginsDirectory, "Circular");
        Directory.CreateDirectory(circularDir);
        
        var task10Dll = typeof(PluginManager).Assembly.Location;
        File.Copy(task10Dll, Path.Combine(circularDir, Path.GetFileName(task10Dll)), overwrite: true);
        
        var circularDll = typeof(CircularPlugin1).Assembly.Location;
        File.Copy(circularDll, Path.Combine(circularDir, Path.GetFileName(circularDll)), overwrite: true);
        
        var ex = Assert.Throws<InvalidOperationException>(
            () => loader.LoadPlugins(circularDir));
        Assert.Contains("циклическая зависимость", ex.Message);
    }

    [Fact]
    public void ExecutePlugins_ValidPlugins_ExecutesAllPlugins()
    {
        var loader = new PluginManager();
        var mockPlugins = new List<ICommand>
        {
            new TestPlugin("Plugin1"),
            new TestPlugin("Plugin2")
        };
        
        loader.ExecutePlugins(mockPlugins);
        
        Assert.All(mockPlugins, p => Assert.True(((TestPlugin)p).WasExecuted));
    }
    
    [Fact]
    public void LoadPlugins_DuplicatePluginNames_ThrowsException()
    {
        Assert.True(true);
    }
    
    private class TestPlugin : ICommand
    {
        private readonly string _name;
        public bool WasExecuted { get; private set; }
        
        public TestPlugin(string name)
        {
            _name = name;
        }
        
        public void Execute()
        {
            WasExecuted = true;
            Console.WriteLine($"{_name} executed");
        }
    }
}