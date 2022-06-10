using DotNode;

namespace Demo.Nodes;

public class InputCatalogConnection : SingleConnection<RadInputNode, UserInput, CatalogNode, ProductIdentifier>
{
    public InputCatalogConnection(Func<UserInput, ProductIdentifier> connectFunction) : base(connectFunction) { }
}
public class InputShipmentConnection : SingleConnection<RadInputNode, UserInput, ShipmentNode, ShipmentPath>
{
    public InputShipmentConnection(Func<UserInput, ShipmentPath> connectFunction) : base(connectFunction) { }
}
public class CatalogPolicyConnection : SingleConnection<CatalogNode, Product, PolicyNode, PolicyId>
{
    public CatalogPolicyConnection(Func<Product, PolicyId> connectFunction) : base(connectFunction) { }
}
public class InputDecisionConnection : MultipleConnection<RadInputNode, UserInput, DecisionNode, DecisionContext>
{
    public InputDecisionConnection(Func<UserInput, DecisionContext, DecisionContext> connectFunction) : base(connectFunction) { }
}
public class CatalogDecisionConnection : MultipleConnection<CatalogNode, Product, DecisionNode, DecisionContext>
{
    public CatalogDecisionConnection(Func<Product, DecisionContext, DecisionContext> connectFunction) : base(connectFunction) { }
}
public class PolicyDecisionConnection : MultipleConnection<PolicyNode, Policy, DecisionNode, DecisionContext>
{
    public PolicyDecisionConnection(Func<Policy, DecisionContext, DecisionContext> connectFunction) : base(connectFunction) { }
}
public class ShipmentDecisionConnection : MultipleConnection<ShipmentNode, Shipment, DecisionNode, DecisionContext>
{
    public ShipmentDecisionConnection(Func<Shipment, DecisionContext, DecisionContext> connectFunction) : base(connectFunction) { }
}
public class DicisionOutputConnection : SingleConnection<DecisionNode, DecisionResult, DispositionOutputNode, UserOutput>
{
    public DicisionOutputConnection(Func<DecisionResult, UserOutput> connectFunction) : base(connectFunction) { }
}
