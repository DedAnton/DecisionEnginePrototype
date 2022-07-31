namespace Expresser.Core.Expressions;

public abstract record Expression;
public abstract record ConstantExpression : Expression;
public sealed record NumberConstant(double Value) : ConstantExpression;
public sealed record StringConstant(string Value) : ConstantExpression;
public sealed record BooleanConstant(bool Value) : ConstantExpression;
public abstract record UnaryExpression : Expression;
public sealed record Not(Expression Expression) : UnaryExpression;
public sealed record Negate(Expression Expression) : UnaryExpression;
public abstract record BinaryExpression : Expression;
public sealed record And(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Or(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Add(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Subtract(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Divide(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Multiply(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record Equal(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record NotEqual(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record GreaterThan(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record GreaterThanOrEqual(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record LessThan(Expression Left, Expression Rigth) : BinaryExpression;
public sealed record LessThanOrEqual(Expression Left, Expression Rigth) : BinaryExpression;
public abstract record FunctionCallExpression : Expression;
public sealed record MulTen(Expression Expression) : FunctionCallExpression;
public sealed record ContextAccess(string Parameter) : Expression; //example: Product.Price