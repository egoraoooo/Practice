namespace task13.Validation;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; }

    public ValidationResult(List<string> errors)
    {
        Errors = errors;
    }
}