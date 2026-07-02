namespace FileSystemCommands;
using System;
using System.IO;
using CommandLib;

public class DirectorySizeCommand : ICommand
{
    private readonly string _directorypath;
    public DirectorySizeCommand(string directorypath)
    {
        _directorypath = directorypath;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directorypath))
        {
            Console.WriteLine($"ОШИБКА: ДИРЕКТОРИЯ '{_directorypath}' НЕ СУЩЕСТВУЕТ");
            return;
        }

        long size = CalculateDirectorySize(_directorypath);
        Console.WriteLine($"Размер директории: {size} байт");
    }

    private long CalculateDirectorySize(string path)
    {
        long total = 0;

        try
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                try
                {
                    FileInfo fileinfo = new FileInfo(file);
                    total += fileinfo.Length;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"ОШИБКА: ФАЙЛ '{file}' НЕ НАЙДЕН");
                }
            }

            string[] subdirs = Directory.GetDirectories(path);
            foreach (string subdir in subdirs)
            {
                total += CalculateDirectorySize(subdir);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"ОШИБКА: НЕТ ДОСТУПА К '{path}'");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"ОШИБКА: ДИРЕКТОРИЯ '{path}' НЕ НАЙДЕНА");
        }

        return total;
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _directorypath;
    private readonly string _searchpattern;

    public FindFilesCommand(string directorypath, string searchpattern)
    {
        _directorypath = directorypath;
        _searchpattern = searchpattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directorypath))
        {
            Console.WriteLine($"ОШИБКА: ДИРЕКТОРИЯ '{_directorypath}' НЕ СУЩЕСТВУЕТ");
            return;
        }

        try
        {
            string[] files = Directory.GetFiles(_directorypath, _searchpattern, SearchOption.AllDirectories);
            
            if (files.Length == 0)
            {
                Console.WriteLine($"Файлы по маске '{_searchpattern}' не найдены");
            }
            else
            {
                Console.WriteLine($"Найдено файлов: {files.Length}");
                foreach (string file in files)
                {
                    FileInfo fileinfo = new FileInfo(file);
                    Console.WriteLine($"= {file} ({fileinfo.Length} байт)");
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"ОШИБКА: НЕТ ДОСТУПА К ДИРЕКТОРИИ '{_directorypath}'");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"ОШИБКА: ДИРЕКТОРИЯ '{_directorypath}' НЕ НАЙДЕНА");
        }
    }
}
