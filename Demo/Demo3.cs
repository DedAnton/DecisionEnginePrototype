using Demo;
using Demo.Nodes;
using DotNode;
using System.Linq.Expressions;

namespace Demo;

internal class Demo3
{
    public static void Run()
    {
        var userInput = new UserInput(1, "New York", "California");
        var expressions = new Dictionary<string, DispositionType>
        {
            {"mulTen(1 + 1) - mulTen(2*2) = 0", DispositionType.A },
            {"Policy.Sos = true", DispositionType.B }
        };

        var connections = new List<Connection>
        {
            new InputCatalogConnection(input => new ProductIdentifier(input.Identifier)),
            new CatalogPolicyConnection(product => new PolicyId(product.PolicyId)),
            new InputShipmentConnection(input => new ShipmentPath(input.AddressFrom, input.AddressTo)),
            new InputDecisionConnection((input, context) => context with { UserInput = input }),
            new CatalogDecisionConnection((product, context) => context with { Product = product }),
            new PolicyDecisionConnection((policy, context) => context with { Policy = policy }),
            new ShipmentDecisionConnection((shipment, context) => context with { Shipment = shipment }),
            new DicisionOutputConnection(decision => new UserOutput(decision.Disposition))
        };

        var builder = new NodeGraphBuilder();
        var nodeGraph = builder
            .AddInputNode(new RadInputNode())
            .AddNode(new CatalogNode())
            .AddNode(new PolicyNode())
            .AddNode(new ShipmentNode())
            .AddNode(new DecisionNode(expressions))
            .AddOutputNode(new DispositionOutputNode())
            .Build();

        var compiler = new NodeGraphCompliler();
        var graphFunction = compiler.Compile(nodeGraph, connections);
        var result = graphFunction(userInput);
        Console.WriteLine($"Decision: {result}");
    }
}
