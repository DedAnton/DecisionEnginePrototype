using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo;
internal static class Demo1
{
    public static void Run()
    {
        //var sampleExpression1 = "(15/(7-(1+1))*3-(2+(1+1))+15/(7-(1+1))*3-(2+(1+1)))";
        //var sampleExpression2 = "2 * (2 + 2)";
        //var sampleExpression3 = "2*2+2";
        //var sampleExpression4 = "(2+2)*(2+2)";
        //var sampleExpression5 = "20/10*0";
        //var sampleExpression6 = "mulTen(2)";
        //var sampleExpression7 = "\"123\" = \"asd\"";
        //var expression = global::ExpressionParser.ExpressionParser.Parse(sampleExpression1);
        //Console.WriteLine(expression);
        //var func = Expression.Lambda<Func<double>>(expression).Compile();
        //var result = func();
        //Console.WriteLine(result);
        //Func<double> a = () => (15 / (7 - (1 + 1)) * 3 - (2 + (1 + 1)) + 15 / (7 - (1 + 1)) * 3 - (2 + (1 + 1)));
        //Console.WriteLine(a());

        //var context = new Context();
        //context.Input = new Input { Identifier = 1, AddressFrom = "New York", AddressTo = "California" };

        //var expressions = new Dictionary<string, DispositionType>
        //{
        //    {"mulTen(1 + 1) - mulTen(2*2) = -20", DispositionType.A },
        //    {"Policy.Sos = true", DispositionType.B }
        //};
        //var decisionNode = new DecisionTableNode(expressions, context, null);
        //var policyNode = new PolicyNode(context, decisionNode);
        //policyNode.Run();

        //(15/((7-(1+1))*(3-((2+(1+1))+(15/((7-(1+1))*(3-(2+(1+1)))))))))
        //(15/(7-(1+1))*3-(2+(1+1))+15/(7-(1+1))*3-(2+(1+1)))
    }
}
