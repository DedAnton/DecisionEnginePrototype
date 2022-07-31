using Expresser.Fsharp;
using System.Linq.Expressions;

var sampleExpression1 = "(15/(7-(1+1))*3-(2+(1+1))+15/(7-(1+1))*3-(2+(1+1)))";
Func<double> test1 = () => (15 / (7 - (1 + 1)) * 3 - (2 + (1 + 1)) + 15 / (7 - (1 + 1)) * 3 - (2 + (1 + 1)));
var sampleExpression2 = "2 * (2 + 2)";
var sampleExpression3 = "2*2+2";
var sampleExpression4 = "(2+2)*(2+2)";
var sampleExpression5 = "20/10*0";
var sampleExpression6 = "sin(1)";
Func<double> test6 = () => Math.Sin(1);
var sampleExpression7 = "\"123\" = \"asd\"";
Func<bool> test7 = () => "123" == "asd";

var expressionString = sampleExpression7;
var testResult = test7();

var parsedExpression = Parser.Parse(expressionString);
var linqExpression = LinqMapper.Map(parsedExpression);
Console.WriteLine(linqExpression);
var compiledFunction = Expression.Lambda<Func<bool>>(linqExpression).Compile();
var result = compiledFunction();
Console.WriteLine(result);
Console.WriteLine(testResult);
