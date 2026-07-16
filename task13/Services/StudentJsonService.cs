namespace task13.Services;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using task13.Models;
using task13.Serialization;
using task13.Validation;

public class StudentJsonService
{
    private readonly JsonSerializerOptions _options;

    public StudentJsonService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        _options.Converters.Add(new CustomDateTimeConverter());
    }

    public string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public void SerializeToFile(Student student, string filePath)
    {
        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public Student Deserialize(string json)
    {
        var options = new JsonSerializerOptions(_options);
        var student = JsonSerializer.Deserialize<Student>(json, options);

        if (student is null)
            throw new JsonException("Не удалось десериализовать объект Student.");

        var result = StudentValidator.Validate(student);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return student;
    }

    public Student DeserializeFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors)
        : base(string.Join("; ", errors))
    {
        Errors = errors;
    }
}