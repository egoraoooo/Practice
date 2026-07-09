using System.Text.Json;
using FluentAssertions;
using task13.Models;
using task13.Services;
using Xunit;

namespace task13tests;

public class StudentJsonServiceTests
{
    private readonly StudentJsonService _service = new();

    private static Student GetValidStudent() => new()
    {
        FirstName = "Иван",
        LastName = "Иванов",
        BirthDate = new DateTime(2000, 5, 15),
        Grades = new List<Subject>
        {
            new() { Name = "Математика", Grade = 5 },
            new() { Name = "Физика", Grade = 4 }
        }
    };

    [Fact]
    public void Serialize_Should_ReturnValidJson_WithCustomDateFormat()
    {
        var student = GetValidStudent();
        var json = _service.Serialize(student);

        json.Should().Contain("\"birthDate\": \"15.05.2000\"");
        json.Should().Contain("\"firstName\": \"Иван\"");
        json.Should().Contain("\"lastName\": \"Иванов\"");
    }

    [Fact]
    public void Serialize_Should_IgnoreNullValues()
    {
        var student = new Student
        {
            FirstName = "Test",
            LastName = null!,
            BirthDate = new DateTime(2000, 1, 1),
            Grades = null!
        };

        var json = _service.Serialize(student);

        json.Should().NotContain("lastName");
        json.Should().NotContain("grades");
    }

    [Fact]
    public void Deserialize_Should_ReturnValidStudent()
    {
        var json = @"{
            ""firstName"": ""Петр"",
            ""lastName"": ""Петров"",
            ""birthDate"": ""20.08.1999"",
            ""grades"": [
                { ""name"": ""Химия"", ""grade"": 3 }
            ]
        }";

        var student = _service.Deserialize(json);

        student.Should().NotBeNull();
        student.FirstName.Should().Be("Петр");
        student.LastName.Should().Be("Петров");
        student.BirthDate.Should().Be(new DateTime(1999, 8, 20));
        student.Grades.Should().HaveCount(1);
    }

    [Fact]
    public void Deserialize_Should_ThrowValidationException_WhenFirstNameIsEmpty()
    {
        var json = @"{
            ""firstName"": """",
            ""lastName"": ""Петров"",
            ""birthDate"": ""20.08.1999"",
            ""grades"": []
        }";

        Action act = () => _service.Deserialize(json);

        act.Should().Throw<ValidationException>()
           .Where(e => e.Errors.Any(err => err.Contains("FirstName")));
    }

    [Fact]
    public void Deserialize_Should_ThrowValidationException_WhenGradeOutOfRange()
    {
        var json = @"{
            ""firstName"": ""Анна"",
            ""lastName"": ""Сидорова"",
            ""birthDate"": ""10.03.2001"",
            ""grades"": [
                { ""name"": ""Биология"", ""grade"": 6 }
            ]
        }";

        Action act = () => _service.Deserialize(json);

        act.Should().Throw<ValidationException>()
           .Where(e => e.Errors.Any(err => err.Contains("диапазона")));
    }

    [Fact]
    public void SerializeToFile_And_DeserializeFromFile_Should_WorkCorrectly()
    {
        var student = GetValidStudent();
        var filePath = Path.GetTempFileName();

        try
        {
            _service.SerializeToFile(student, filePath);
            var loaded = _service.DeserializeFromFile(filePath);

            loaded.Should().NotBeNull();
            loaded.FirstName.Should().Be(student.FirstName);
            loaded.LastName.Should().Be(student.LastName);
            loaded.BirthDate.Should().Be(student.BirthDate);
            loaded.Grades.Should().HaveCount(student.Grades.Count);
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void DeserializeFromFile_Should_ThrowFileNotFoundException()
    {
        Action act = () => _service.DeserializeFromFile("nonexistent.json");

        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void Deserialize_Should_ThrowJsonException_WhenInvalidDateFormat()
    {
        var json = @"{
            ""firstName"": ""Test"",
            ""lastName"": ""Test"",
            ""birthDate"": ""2000-05-15"",
            ""grades"": []
        }";

        Action act = () => _service.Deserialize(json);

        act.Should().Throw<JsonException>().WithMessage("*формат даты*");
    }

    [Fact]
    public void Deserialize_Should_ThrowJsonException_WhenStudentIsNull()
    {
        var json = "null";

        Action act = () => _service.Deserialize(json);

        act.Should().Throw<JsonException>().WithMessage("*десериализовать*");
    }
}