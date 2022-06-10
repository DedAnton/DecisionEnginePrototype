using Expresser.Core.Exceptions;
using Expresser.Core.Tokens;

namespace Expresser.Core;

internal static class Tokenizer
{
    private static readonly HashSet<char> _operatorsSymbols = new() { '<', '>', '=', '+', '-', '*', '/' };
    private static readonly HashSet<string> _literalOperators = new() { "not", "or", "and" };
    private static readonly HashSet<string> _functions = new() { "mulTen" };

    public static Span<Token> Tokenize(string stringExpression)
    {
        var tokens = new List<Token>();
        var pos = 0;
        for (; pos < stringExpression.Length; pos++)
        {
            var ch = stringExpression[pos];
            var charType = GetCharType(ch);
            var token = charType switch
            {
                CharType.Digit => GetNumberConstantToken(stringExpression, ref pos),
                CharType.StringConstantSymbol => GetStringConstantToken(stringExpression, ref pos),
                CharType.OperatorSymbol => GetOperatorToken(stringExpression, ref pos, tokens),
                CharType.Letter => GetTokenFromString(stringExpression, ref pos),
                CharType.OpeningBracket => new OpeningBracket(),
                CharType.ClosingBracket => new ClosingBracket(),
                CharType.WhiteSpace => null,
                _ => throw new ParsingException($"Unknown symbol {ch}. Position: {pos}")
            };

            if (token != null)
            {
                tokens.Add(token);
            }
        }

        return tokens.ToArray().AsSpan();
    }

    private static NumberConstant GetNumberConstantToken(string expression, ref int pos)
    {
        var startPosition = pos;
        var strNumber = new ValueStringBuilder(16);

        for (; pos < expression.Length; pos++)
        {
            char ch = expression[pos];
            if (char.IsDigit(ch) || ch == '.')
                strNumber.Append(ch);
            else
            {
                pos--;
                break;
            }
        }

        if (double.TryParse(strNumber.AsSpan(), out var value))
        {
            return new NumberConstant(value);
        }
        else
        {
            throw new ParsingException($"Can`t parse {strNumber.ToString()} to number constant. Position: {startPosition}");
        }
    }

    private static StringConstant GetStringConstantToken(string stringExpression, ref int pos)
    {
        var startPosition = pos;
        var value = new ValueStringBuilder(16);

        pos++;//skip first "
        for (; pos < stringExpression.Length; pos++)
        {
            char ch = stringExpression[pos];
            if (ch == '\\')
            {
                if (pos + 1 < stringExpression.Length && stringExpression[pos + 1] == '\\')
                {
                    value.Append('\\');
                    pos++;
                    continue;
                }
                if (pos + 1 < stringExpression.Length && stringExpression[pos + 1] == '"')
                {
                    value.Append('"');
                    pos++;
                    continue;
                }
                
                throw new ParsingException($"Unexpected escape sequence. Expected \\\" or \\\\ . Position: {pos}");
            }
            if (ch == '"')
            {
                return new StringConstant(value.ToString());
            }
            else
            {
                value.Append(ch);
            }
        }

        throw new ParsingException($"String constant has no end. Symbol \" expected in the end of string constant. Position: {startPosition}");
    }

    private static Token GetTokenFromString(string expression, ref int pos)
    {
        var stringBuilder = new ValueStringBuilder(16);
        var dotPosition = -1;

        for (; pos < expression.Length; pos++)
        {
            char ch = expression[pos];
            if (char.IsLetter(ch))
            {
                stringBuilder.Append(ch);
            }
            else if (ch == '.')
            {
                if (dotPosition == -1)
                {
                    dotPosition = pos;
                    stringBuilder.Append(ch);
                }
                else
                {
                    throw new ParsingException($"Variable property must be simple type. Unexpected symbol '.' in Position {pos}");
                }
            }
            else
            {
                pos--;
                break;
            }
        }

        var value = stringBuilder.ToString();

        return value switch
        {
            "true" => new BooleanConstant(true),
            "false" => new BooleanConstant(false),
            "not" => new Not(),
            "or" => new Or(),
            "and" => new And(),
            "mulTen" => new MulTen(),
            _ when dotPosition != -1 => new ParameterPropertyInvocation(value.Substring(0, dotPosition), value.Substring(dotPosition + 1)),
            _ => new Parameter(value)
        };
    }

    private static Token GetOperatorToken(string stringExpression, ref int pos, IEnumerable<Token> previousTokens)
    {
        var op = "";
        if (pos < stringExpression.Length - 1
            && _operatorsSymbols.Contains(stringExpression[pos + 1]))
        {
            op = $"{stringExpression[pos]}{stringExpression[pos + 1]}";
            pos++;
        }
        else
        {
            op = stringExpression[pos].ToString();
        }

        return op switch
        {
            "-" when previousTokens.LastOrDefault() is not { } prevToken 
                || (prevToken is BinaryOperator or OpeningBracket or Function) => new Negate(),
            "=" => new Equal(),
            "<>" => new NotEqual(),
            ">" => new GreaterThan(),
            "<" => new LessThan(),
            ">=" => new GreaterThanOrEqual(),
            "<=" => new LessThanOrEqual(),
            "+" => new Add(),
            "-" => new Subtract(),
            "*" => new Multiply(),
            "/" => new Divide(),
            _ => throw new ParsingException($"Undefined operator {op}. Position: {pos}")
        };
    }

    private static CharType GetCharType(char c)
    {
        if (char.IsDigit(c))
            return CharType.Digit;

        if (char.IsLetter(c))
            return CharType.Letter;

        if (char.IsWhiteSpace(c))
            return CharType.WhiteSpace;

        if (c == '"')
        {
            return CharType.StringConstantSymbol;
        }

        if (c == '(')
        {
            return CharType.OpeningBracket;
        }

        if (c == ')')
        {
            return CharType.ClosingBracket;
        }

        if (_operatorsSymbols.Contains(c))
            return CharType.OperatorSymbol;

        return CharType.Unknown;
    }

    internal enum CharType
    {
        Unknown,
        Digit,
        Letter,
        OperatorSymbol,
        StringConstantSymbol,
        OpeningBracket,
        ClosingBracket,
        WhiteSpace
    }
}
