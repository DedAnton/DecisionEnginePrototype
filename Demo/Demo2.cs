using Demo.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo;

public static class Demo2
{
    public static void Run()
    {
        var userInput = new UserInput(1, "New York", "California");
        var inputNode = new RadInputNode();

        var catalogNode = new CatalogNode();
        var catalogConnection = new InputCatalogConnection(input => new ProductIdentifier(input.Identifier));

        var policyNode = new PolicyNode();
        var policyConnection = new CatalogPolicyConnection(product => new PolicyId(product.PolicyId));

        var shipmentNode = new ShipmentNode();
        var shipmentConnection = new InputShipmentConnection(input => new ShipmentPath(input.AddressFrom, input.AddressTo));

        var expressions = new Dictionary<string, DispositionType>
{
    {"mulTen(1 + 1) - mulTen(2*2) = 0", DispositionType.A },
    {"Policy.Sos = true", DispositionType.B }
};
        var decisionNode = new DecisionNode(expressions);
        var inputDecisionConnection = new InputDecisionConnection((input, context) => context with { UserInput = input });
        var catalogDecisionConnection = new CatalogDecisionConnection((product, context) => context with { Product = product });
        var policyDecisionConnection = new PolicyDecisionConnection((policy, context) => context with { Policy = policy });
        var shipmentDecisionConnection = new ShipmentDecisionConnection((shipment, context) => context with { Shipment = shipment });
        //-----------------------------------------------------------------------------------------------------------------

        var inputResult = inputNode.Run(userInput);
        var catalogConnectionResult = catalogConnection.ConnectFunction!.Invoke(inputResult);
        var catalogResult = catalogNode.Run(catalogConnectionResult);
        var policyConnectionResult = policyConnection.ConnectFunction!.Invoke(catalogResult);
        var policyResult = policyNode.Run(policyConnectionResult);
        var shipmentConnectionResult = shipmentConnection.ConnectFunction!.Invoke(inputResult);
        var shipmentResult = shipmentNode.Run(shipmentConnectionResult);

        var decisionContext = new DecisionContext();
        decisionContext = inputDecisionConnection.ConnectFunction.Invoke(inputResult, decisionContext);
        decisionContext = catalogDecisionConnection.ConnectFunction.Invoke(catalogResult, decisionContext);
        decisionContext = policyDecisionConnection.ConnectFunction.Invoke(policyResult, decisionContext);
        decisionContext = shipmentDecisionConnection.ConnectFunction.Invoke(shipmentResult, decisionContext);
        var decisionResult = decisionNode.Run(decisionContext);

        Console.WriteLine($"Disposition: {decisionResult.Disposition}");
    }
}