namespace task13.Models;
using System.Text.Json.Serialization;
using task13.Serialization;

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    public List<Subject> Grades { get; set; } = new();
}