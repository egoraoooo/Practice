namespace task05;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).Select(m => m.Name);  
    }

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        MethodInfo? method = _type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance);
        
        if (method == null)
        {
            return Enumerable.Empty<string>();
        }
        
        var param = method.GetParameters().Select(p => $"{p.Name} ({p.ParameterType.Name})");
        var returnt = $"Return: {method.ReturnType.Name}";
        
        return param.Append(returnt);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(f => f.Name);   
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttributes(typeof(T), false).Any();
    }
}
