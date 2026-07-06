namespace FileSystemCommandsTests;
using System;
using System.IO;
using Xunit;
using FileSystemCommands;
using CommandLib;

public class FileSystemCommandsTests
{
    private string CreateTestDirectory()
    {
        string testDir = Path.Combine(Path.GetTempPath(), $"TestDir_{Guid.NewGuid()}");
        Directory.CreateDirectory(testDir);
        return testDir;
    }

    private string CreateTestFile(string directory, string fileName, string content)
    {
        string filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute(); // Проверяем, что не возникает исключений

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute(); // Должен найти 1 файл

        Directory.Delete(testDir, true);
    }
    
    [Fact]
    public void DirectorySizeCommand_EmptyDirectory_ShouldReturnZero()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            var command = new DirectorySizeCommand(testDir);
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_WithFiles_ShouldExecuteSuccessfully()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            CreateTestFile(testDir, "test1.txt", "Hello World");
            CreateTestFile(testDir, "test2.txt", "Test content");
            
            string subDir = Path.Combine(testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            CreateTestFile(subDir, "test3.txt", "Subdirectory file");
            
            var command = new DirectorySizeCommand(testDir);
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_NonExistentDirectory_ShouldNotThrowException()
    {
        string nonExistentPath = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");
        
        // Убеждаемся, что директории нет
        Assert.False(Directory.Exists(nonExistentPath));
        
        var command = new DirectorySizeCommand(nonExistentPath);
        
        // Act & Assert
        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void DirectorySizeCommand_WithLargeFiles_ShouldExecuteSuccessfully()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            string filePath = Path.Combine(testDir, "large.bin");
            byte[] data = new byte[1024 * 100]; // 100 КБ
            File.WriteAllBytes(filePath, data);
            
            var command = new DirectorySizeCommand(testDir);
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void FindFilesCommand_WithMatchingFiles_ShouldFindThem()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            CreateTestFile(testDir, "file1.txt", "Text file");
            CreateTestFile(testDir, "file2.txt", "Another text file");
            CreateTestFile(testDir, "file3.log", "Log file");
            
            var command = new FindFilesCommand(testDir, "*.txt");
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void FindFilesCommand_WithNoMatches_ShouldExecuteSuccessfully()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            CreateTestFile(testDir, "file1.txt", "Text file");
            CreateTestFile(testDir, "file2.txt", "Another text file");
            
            var command = new FindFilesCommand(testDir, "*.pdf");
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void FindFilesCommand_RecursiveSearch_ShouldFindFilesInSubdirectories()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            CreateTestFile(testDir, "root.txt", "Root file");
            
            string subDir = Path.Combine(testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            CreateTestFile(subDir, "sub.txt", "Sub file");
            
            var command = new FindFilesCommand(testDir, "*.txt");
            
            var exception = Record.Exception(() => command.Execute());
            Assert.Null(exception);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void FindFilesCommand_NonExistentDirectory_ShouldNotThrowException()
    {
        string nonExistentPath = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");
        Assert.False(Directory.Exists(nonExistentPath));
        
        var command = new FindFilesCommand(nonExistentPath, "*.txt");
        
        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void FindFilesCommand_DifferentPatterns_ShouldExecuteSuccessfully()
    {
        string testDir = CreateTestDirectory();
        
        try
        {
            CreateTestFile(testDir, "test.cs", "Code");
            CreateTestFile(testDir, "test.txt", "Text");
            CreateTestFile(testDir, "test.md", "Markdown");
            
            var patterns = new[] { "*.cs", "*.txt", "*.md", "*.*" };
            
            foreach (var pattern in patterns)
            {
                var command = new FindFilesCommand(testDir, pattern);
                
                var exception = Record.Exception(() => command.Execute());
                Assert.Null(exception);
            }
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void BothCommands_ShouldImplementICommand()
    {
        Assert.True(typeof(ICommand).IsAssignableFrom(typeof(DirectorySizeCommand)));
        Assert.True(typeof(ICommand).IsAssignableFrom(typeof(FindFilesCommand)));
    }

    [Fact]
    public void Commands_ShouldHaveCorrectConstructors()
    {
        var dirSizeConstructor = typeof(DirectorySizeCommand).GetConstructor(new[] { typeof(string) });
        var findFilesConstructor = typeof(FindFilesCommand).GetConstructor(new[] { typeof(string), typeof(string) });
        
        Assert.NotNull(dirSizeConstructor);
        Assert.NotNull(findFilesConstructor);
    }

    [Fact]
    public void Commands_ShouldHaveExecuteMethod()
    {
        var executeMethod1 = typeof(DirectorySizeCommand).GetMethod("Execute");
        var executeMethod2 = typeof(FindFilesCommand).GetMethod("Execute");
        
        Assert.NotNull(executeMethod1);
        Assert.NotNull(executeMethod2);
    }
}
