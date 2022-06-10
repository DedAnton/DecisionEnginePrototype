using Expresser.Core.Exceptions;
using Expresser.Core.Tokens;

namespace Expresser.Core;

internal static class Parser
{
    public static Expressions.Expression Parse(string stringExpression)
    {
        var tokens = Tokenizer.Tokenize(stringExpression);
        return Parse(tokens);
    }

    private static Expressions.Expression Parse(in Span<Token> tokens)
    {
        if (tokens.Length == 1)
        {
            return tokens[0] switch
            {
                NumberConstant constant => new Expressions.NumberConstant(constant.Value),
                StringConstant constant => new Expressions.StringConstant(constant.Value),
                BooleanConstant constant => new Expressions.BooleanConstant(constant.Value),
                ParameterPropertyInvocation parameter => new Expressions.ContextAccess($"{parameter.ParameterName}.{parameter.PropertyName}"),

                _ => throw new ParsingException($"Invalid expression. Unexpected token {tokens[0]}.")
            };
        }

        if (TryGetNextOperationPos(tokens, out var pos))
        {
            var operation = tokens[pos];

            return operation switch
            {
                Equal => new Expressions.Equal(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                NotEqual => new Expressions.NotEqual(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                GreaterThan => new Expressions.GreaterThan(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                LessThan => new Expressions.LessThan(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                GreaterThanOrEqual => new Expressions.Equal(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                LessThanOrEqual => new Expressions.NotEqual(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                And => new Expressions.And(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Or => new Expressions.Or(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Not => new Expressions.Not(Parse(tokens.Slice(0, 1))),

                Add => new Expressions.Add(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Subtract => new Expressions.Subtract(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Divide => new Expressions.Divide(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Multiply => new Expressions.Multiply(Parse(tokens.Slice(0, pos)), Parse(tokens.Slice(pos + 1))),
                Negate => new Expressions.Negate(Parse(tokens.Slice(1, tokens.Length - 1))),

                MulTen => new Expressions.MulTen(Parse(tokens.Slice(1, tokens.Length - 1))),

                _ => throw new NotImplementedException($"Expression for operation {operation} is implemented.")
            };
        }
        else
        {
            if (tokens[0] is OpeningBracket && tokens[tokens.Length - 1] is ClosingBracket)
            {
                return Parse(tokens.Slice(1, tokens.Length - 2));
            }
        }

        throw new ParsingException($"Expression can not be parsed.");
    }

    private static bool TryGetNextOperationPos(in Span<Token> tokens, out int nextOperationPos)
    {
        var bracketDepth = 0;
        (int Position, int Priority) nextOp = (-1, -1);
        for (var i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            if (token is OpeningBracket)
            {
                bracketDepth++;
                continue;
            }
            if (token is ClosingBracket)
            {
                bracketDepth--;
                continue;
            }

            if (bracketDepth == 0 && token is Operator or Function)
            {
                var priority = GetOperationPriority(token);
                if (priority >= nextOp.Priority)
                {
                    nextOp = (i, priority);
                }
            }
        }

        nextOperationPos = nextOp.Position;

        return nextOperationPos != -1;
    }

    private static int GetOperationPriority(Token token)
    {
        return token switch
        {
            Or => 8,
            And => 7,
            Equal => 6,
            NotEqual => 6,
            GreaterThan => 5,
            GreaterThanOrEqual => 5,
            LessThan => 5,
            LessThanOrEqual => 5,
            Add => 4,
            Subtract => 4,
            Multiply => 3,
            Divide => 3,
            Not => 2,
            Negate => 2,
            Function => 1,
            _ => throw new ArgumentException($"Token {token} is not a operator or a function and has no priority")
        };
    }
}
