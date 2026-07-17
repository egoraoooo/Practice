namespace task13.Validation;
using task13.Models;

public static class StudentValidator
{
    public static ValidationResult Validate(Student? student)
    {
        var errors = new List<string>();

        if (student is null)
        {
            errors.Add("Объект Student не может быть null.");
            return new ValidationResult(errors);
        }

        if (string.IsNullOrWhiteSpace(student.FirstName))
            errors.Add("FirstName обязателен.");

        if (string.IsNullOrWhiteSpace(student.LastName))
            errors.Add("LastName обязателен.");

        if (student.BirthDate > DateTime.Now || student.BirthDate < DateTime.Now.AddYears(-120))
            errors.Add("BirthDate должен быть в разумном диапазоне.");

        if (student.Grades is null || student.Grades.Count == 0)
        {
            errors.Add("Grades не должен быть пустым.");
        }
        else
        {
            foreach (var grade in student.Grades)
            {
                if (string.IsNullOrWhiteSpace(grade.Name))
                    errors.Add("Название предмета обязательно.");

                if (grade.Grade < 2 || grade.Grade > 5)
                    errors.Add($"Оценка '{grade.Grade}' вне допустимого диапазона (2-5).");
            }
        }

        return new ValidationResult(errors);
    }
}