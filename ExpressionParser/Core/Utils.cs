using Expresser.Core.Expressions;

namespace Expresser.Core;

internal static class Utils
{
    public static string ToExpressionString(this Expression expression)
    {
        return expression switch
        {
            BooleanConstant constant => $"{constant.Value}",
            NumberConstant constant => $"{constant.Value}",
            StringConstant constant => $"{constant.Value}",

            And operation => $"({operation.Left.ToExpressionString()} & {operation.Rigth.ToExpressionString()})",
            Or operation => $"({operation.Left.ToExpressionString()} | {operation.Rigth.ToExpressionString()})",
            Equal operation => $"({operation.Left.ToExpressionString()} = {operation.Rigth.ToExpressionString()})",
            NotEqual operation => $"({operation.Left.ToExpressionString()} <> {operation.Rigth.ToExpressionString()})",
            LessThan operation => $"({operation.Left.ToExpressionString()} < {operation.Rigth.ToExpressionString()})",
            LessThanOrEqual operation => $"({operation.Left.ToExpressionString()} <= {operation.Rigth.ToExpressionString()})",
            GreaterThan operation => $"({operation.Left} > {operation.Rigth})",
            GreaterThanOrEqual operation => $"({operation.Left} >= {operation.Rigth})",

            Add operation => $"({operation.Left.ToExpressionString()} + {operation.Rigth.ToExpressionString()})",
            Subtract operation => $"({operation.Left.ToExpressionString()} - {operation.Rigth.ToExpressionString()})",
            Multiply operation => $"({operation.Left.ToExpressionString()} * {operation.Rigth.ToExpressionString()})",
            Divide operation => $"({operation.Left.ToExpressionString()} / {operation.Rigth.ToExpressionString()})",

            Negate operation => $"(-{operation.Expression.ToExpressionString()})",
            Not operation => $"(not {operation.Expression.ToExpressionString()})",

            MulTen function => $"mulTen({function.Expression.ToExpressionString()})",
            _ => throw new InvalidOperationException()
        };
    }
}
