namespace Expresser.Core;

internal class ExpressionValidator
{
    public bool Validate(string stringExpression, out string[] ValidationErrors)
    {
        ValidationErrors = Array.Empty<string>();
        return true;
    }
}
