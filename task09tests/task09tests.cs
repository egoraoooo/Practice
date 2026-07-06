using System;
using System.Linq;
using System.Reflection;
using Xunit;
using FileSystemCommands;
using CommandLib;

namespace MetadataViewerTests
{
    public class MetadataViewerTests
    {
        [Fact]
        public void Test_DirectorySizeCommand_Exists()
        {
            Type type = typeof(DirectorySizeCommand);
            Assert.NotNull(type);
            Assert.Equal("DirectorySizeCommand", type.Name);
        }

        [Fact]
        public void Test_DirectorySizeCommand_HasDisplayNameAttribute()
        {
            Type type = typeof(DirectorySizeCommand);
            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Команда подсчета размера директории", attr.DisplayName);
        }

        [Fact]
        public void Test_DirectorySizeCommand_HasVersionAttribute()
        {
            Type type = typeof(DirectorySizeCommand);
            var attr = type.GetCustomAttribute<VersionAttribute>();
            Assert.NotNull(attr);
            Assert.Equal(1, attr.Major);
            Assert.Equal(0, attr.Minor);
        }

        [Fact]
        public void Test_DirectorySizeCommand_Constructor()
        {
            Type type = typeof(DirectorySizeCommand);
            ConstructorInfo[] constructors = type.GetConstructors();
            Assert.Single(constructors);

            var ctor = constructors[0];
            var parameters = ctor.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("directorypath", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
        }

        [Fact]
        public void Test_DirectorySizeCommand_ExecuteMethod()
        {
            Type type = typeof(DirectorySizeCommand);
            MethodInfo method = type.GetMethod("Execute");
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);

            var attr = method.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Выполнить команду", attr.DisplayName);

            var parameters = method.GetParameters();
            Assert.Empty(parameters);
        }

        [Fact]
        public void Test_FindFilesCommand_Exists()
        {
            Type type = typeof(FindFilesCommand);
            Assert.NotNull(type);
            Assert.Equal("FindFilesCommand", type.Name);
        }

        [Fact]
        public void Test_FindFilesCommand_HasDisplayNameAttribute()
        {
            Type type = typeof(FindFilesCommand);
            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Команда поиска файлов", attr.DisplayName);
        }

        [Fact]
        public void Test_FindFilesCommand_HasVersionAttribute()
        {
            Type type = typeof(FindFilesCommand);
            var attr = type.GetCustomAttribute<VersionAttribute>();
            Assert.NotNull(attr);
            Assert.Equal(1, attr.Major);
            Assert.Equal(0, attr.Minor);
        }

        [Fact]
        public void Test_FindFilesCommand_Constructor()
        {
            Type type = typeof(FindFilesCommand);
            ConstructorInfo[] constructors = type.GetConstructors();
            Assert.Single(constructors);

            var ctor = constructors[0];
            var parameters = ctor.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("directorypath", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal("searchpattern", parameters[1].Name);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void Test_FindFilesCommand_ExecuteMethod()
        {
            Type type = typeof(FindFilesCommand);
            MethodInfo method = type.GetMethod("Execute");
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);

            var attr = method.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Выполнить команду", attr.DisplayName);

            var parameters = method.GetParameters();
            Assert.Empty(parameters);
        }

        [Fact]
        public void Test_MetadataViewer_LoadsAssembly()
        {
            Assembly assembly = typeof(DirectorySizeCommand).Assembly;
            Assert.NotNull(assembly);

            Type[] types = assembly.GetTypes();
            Assert.Contains(types, t => t.Name == "DirectorySizeCommand");
            Assert.Contains(types, t => t.Name == "FindFilesCommand");
        }

        [Fact]
        public void Test_MetadataViewer_ExtractsAllClasses()
        {
            Assembly assembly = typeof(DirectorySizeCommand).Assembly;
            Type[] types = assembly.GetTypes();

            Assert.Equal(2, types.Length);
            Assert.Contains(types, t => t.Name == "DirectorySizeCommand");
            Assert.Contains(types, t => t.Name == "FindFilesCommand");
        }

        [Fact]
        public void Test_MetadataViewer_ExtractsConstructorsWithParameters()
        {
            Type type = typeof(FindFilesCommand);
            ConstructorInfo[] constructors = type.GetConstructors();

            Assert.Single(constructors);

            var ctor = constructors[0];
            var parameters = ctor.GetParameters();
            Assert.Equal(2, parameters.Length);

            Assert.Equal("directorypath", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);

            Assert.Equal("searchpattern", parameters[1].Name);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void Test_MetadataViewer_ExtractsMethodAttributes()
        {
            Type type = typeof(DirectorySizeCommand);
            MethodInfo method = type.GetMethod("Execute");

            var attrs = method.GetCustomAttributes(false);
            Assert.Single(attrs);

            var displayNameAttr = attrs[0] as DisplayNameAttribute;
            Assert.NotNull(displayNameAttr);
            Assert.Equal("Выполнить команду", displayNameAttr.DisplayName);
        }
    }
}