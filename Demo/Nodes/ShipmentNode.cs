using DotNode;

namespace Demo.Nodes;
public class ShipmentNode : Node<ShipmentPath, Shipment>
{
    public override Shipment Run(ShipmentPath input)
    {
        var cost = GetShipmentCost(input.AddressFrom, input.AddressTo);
        var shipment = new Shipment(cost);

        return shipment;
    }

    private double GetShipmentCost(string addressFrom, string addressTo)
    {
        return (addressFrom, addressTo) switch
        {
            ("New York", "California") => 500,
            _ => 10
        };
    }
}

public record ShipmentPath(string AddressFrom, string AddressTo) : Input;
public record Shipment(double Cost) : Output;