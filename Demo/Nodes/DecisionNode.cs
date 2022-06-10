using DotNode;
using Expresser;
using System.Linq.Expressions;

namespace Demo.Nodes;
public class DecisionNode : Node<DecisionContext, DecisionResult>
{
    public DecisionNode(Dictionary<string, DispositionType> expressions)
    {
        Expressions = expressions;
    }

    public Dictionary<string, DispositionType> Expressions { get; }

    public override DecisionResult Run(DecisionContext input)
    {
        var expressions = new List<Expression>();

        var contextParameter = Expression.Parameter(typeof(DecisionContext), "context");
        LabelTarget returnTarget = Expression.Label(typeof(DispositionType));
        foreach (var (expression, disposition) in Expressions)
        {
            var parsedExpression = ExpressionParser.Parse(expression, contextParameter);
            var result = Expression.Return(returnTarget, Expression.Constant(disposition), typeof(DispositionType));
            var ifExp = Expression.IfThen(parsedExpression, result);
            expressions.Add(ifExp);
        }

        expressions.Add(Expression.Label(returnTarget, Expression.Default(typeof(DispositionType))));
        var block = Expression.Block(typeof(DispositionType), expressions);
        var lambda = Expression.Lambda<Func<DecisionContext, DispositionType>>(block, contextParameter);
        var function = lambda.Compile();
        var decision = function(input);

        return new DecisionResult(decision);
    }
}

public enum DispositionType
{
    A,
    B,
    C
}

public record DecisionContext(UserInput? UserInput, Product? Product, Policy? Policy, Shipment? Shipment) : Input
{
    public DecisionContext() : this(null, null, null, null) { }
}
public record DecisionResult(DispositionType Disposition) : Output;
