namespace Expresser.Core.Exceptions;

public class ValidationException : Exception
{
    public string StringExpression { get; }
    public string[] ValidationErrors { get; }

    public ValidationException(string stringExpression, string[] validationErrors)
        :base($"Expression \"{stringExpression}\" is not valid.\r\n" +
            $"Validation errors:\r\n" +
            $"{string.Join(".\r\n", validationErrors)}")
    {
        StringExpression = stringExpression;
        ValidationErrors = validationErrors;
    }
}
