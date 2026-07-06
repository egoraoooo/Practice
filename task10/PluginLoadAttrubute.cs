namespace PluginLoadAttrubute;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PluginLoadAttrubute : Attribute
{
    public string[] DependsOn { get; }

    public PluginLoadAttrubute(params string[] depon)
    {
        DependsOn = depon ?? Array.Empty<string>(); 
    } 
}