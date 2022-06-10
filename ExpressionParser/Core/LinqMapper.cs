using Expresser.Core.Expressions;
using System.Reflection;
using LinqExp = System.Linq.Expressions.Expression;

namespace Expresser.Core;

internal static class LinqMapper
{
    public static System.Linq.Expressions.ParameterExpression Parameter = null;

    private static readonly MethodInfo _mulTen = GetMethodInfo(() => Functions.MulTen(0));

    public static LinqExp Map(Expression expression)
    {
        return expression switch
        {
            NumberConstant constant => LinqExp.Constant(constant.Value, typeof(double)),
            StringConstant constant => LinqExp.Constant(constant.Value, typeof(string)),
            BooleanConstant constant => LinqExp.Constant(constant.Value, typeof(bool)),
            Not exp => LinqExp.Not(Map(exp.Expression)),
            Negate exp => LinqExp.Negate(Map(exp.Expression)),
            Add exp => LinqExp.Add(Map(exp.Left), Map(exp.Rigth)),
            Subtract exp => LinqExp.Subtract(Map(exp.Left), Map(exp.Rigth)),
            Divide exp => LinqExp.Divide(Map(exp.Left), Map(exp.Rigth)),
            Multiply exp => LinqExp.Multiply(Map(exp.Left), Map(exp.Rigth)),
            And exp => LinqExp.And(Map(exp.Left), Map(exp.Rigth)),
            Or exp => LinqExp.Or(Map(exp.Left), Map(exp.Rigth)),
            Equal exp => LinqExp.Equal(Map(exp.Left), Map(exp.Rigth)),
            NotEqual exp => LinqExp.NotEqual(Map(exp.Left), Map(exp.Rigth)),
            GreaterThan exp => LinqExp.GreaterThan(Map(exp.Left), Map(exp.Rigth)),
            LessThan exp => LinqExp.LessThan(Map(exp.Left), Map(exp.Rigth)),
            GreaterThanOrEqual exp => LinqExp.GreaterThanOrEqual(Map(exp.Left), Map(exp.Rigth)),
            LessThanOrEqual exp => LinqExp.LessThanOrEqual(Map(exp.Left), Map(exp.Rigth)),
            MulTen exp => LinqExp.Call(_mulTen, Map(exp.Expression)),
            ContextAccess exp => GetPropertyPathAccessor(Parameter, exp.Parameter),
            _ => throw new NotImplementedException($"Mapping for expression {expression} not implemented.")
        };
    }

    private static MethodInfo GetMethodInfo(System.Linq.Expressions.Expression<Action> expression)
    {
        var member = expression.Body as System.Linq.Expressions.MethodCallExpression;

        if (member != null)
            return member.Method;

        throw new ArgumentException("Expression is not a method", "expression");
    }

    private static System.Linq.Expressions.MemberExpression GetPropertyPathAccessor(System.Linq.Expressions.Expression parameter, string path)
    {
        return (System.Linq.Expressions.MemberExpression)path.Split('.').Aggregate(parameter, System.Linq.Expressions.Expression.Property);
    }
}
