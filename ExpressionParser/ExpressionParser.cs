using Expresser.Core;
using Expresser.Core.Exceptions;
using System.Linq.Expressions;

namespace Expresser;

public static class ExpressionParser
{
    public static Expression Parse(string stringExpression, System.Linq.Expressions.ParameterExpression parameter)
    {
        var validator = new ExpressionValidator();
        if (!validator.Validate(stringExpression, out var errors))
        {
            throw new ValidationException(stringExpression, errors);
        }

        var expression = Parser.Parse(stringExpression);

        LinqMapper.Parameter = parameter;
        var linqExpression = LinqMapper.Map(expression);

        return linqExpression;
    }
}
