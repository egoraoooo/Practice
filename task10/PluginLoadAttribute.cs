namespace PluginSystem;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PluginLoadAttribute : Attribute
{
    public string Name { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string name, params string[] depon)
    {
        Name = name;
        Dependencies = depon ?? Array.Empty<string>(); 
    } 
}