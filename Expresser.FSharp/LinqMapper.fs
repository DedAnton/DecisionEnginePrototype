namespace Expresser.Fsharp

open System.Linq.Expressions

module LinqMapper =

    type Expr = 
        static member GetMethodInfo(e:Expression<System.Func<double, double>>) = 
            if (e.Body :? System.Linq.Expressions.MethodCallExpression) 
            then
                let methodCall = e.Body :?> System.Linq.Expressions.MethodCallExpression
                methodCall.Method
            else
                failwith "Expression is not a method"

    let sin = Expr.GetMethodInfo(fun x -> System.Math.Sin x)
    let cos = Expr.GetMethodInfo(fun x -> System.Math.Cos x)

    let rec Map expression : System.Linq.Expressions.Expression =

        match expression with
        | Parser.Expression.NumberConstant number -> Expression.Constant (number, typeof<double>)
        | Parser.Expression.BooleanConstant bool -> Expression.Constant (bool, typeof<bool>)
        | Parser.Expression.StringConstant string -> Expression.Constant (string, typeof<string>)
        | Parser.Expression.Not expression -> Expression.Not (Map expression)
        | Parser.Expression.Negate expression ->  Expression.Negate (Map expression)
        | Parser.Expression.Add (left, right) ->  Expression.Add (Map left, Map right)
        | Parser.Expression.Subtract (left, right) -> Expression.Subtract (Map left, Map right)
        | Parser.Expression.Divide (left, right) -> Expression.Divide (Map left, Map right)
        | Parser.Expression.Multiply (left, right) -> Expression.Multiply (Map left, Map right)
        | Parser.Expression.And (left, right) -> Expression.And (Map left, Map right)
        | Parser.Expression.Or (left, right) -> Expression.Or (Map left, Map right)
        | Parser.Expression.Equal (left, right) -> Expression.Equal (Map left, Map right)
        | Parser.Expression.NotEqual (left, right) -> Expression.NotEqual (Map left, Map right)
        | Parser.Expression.GreaterThan (left, right) -> Expression.GreaterThan (Map left, Map right)
        | Parser.Expression.LessThan (left, right) -> Expression.LessThan (Map left, Map right)
        | Parser.Expression.GreaterThanOrEqual (left, right) -> Expression.GreaterThanOrEqual (Map left, Map right)
        | Parser.Expression.LessThanOrEqual (left, right) -> Expression.LessThanOrEqual (Map left, Map right)
        | Parser.Expression.Sin expression -> Expression.Call (sin, Map expression)
        | Parser.Expression.Cos expression -> Expression.Call (cos, Map expression)