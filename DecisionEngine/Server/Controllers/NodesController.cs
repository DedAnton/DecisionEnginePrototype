using DecisionEngine.Shared;
using Demo.Nodes;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DecisionEngine.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class NodesController : ControllerBase
{
    private readonly string[] _allNodes = { nameof(RadInputNode), nameof(CatalogNode), nameof(PolicyNode), nameof(ShipmentNode), nameof(DecisionNode), nameof(DispositionOutputNode) };

    [HttpPost]
    public void CreateNodeGraph(NodeGraph newNodeGraph)
    {
        var asd = newNodeGraph.Nodes.Where(x => x is DecisionTableNode).Select(x => x as DecisionTableNode);
    }
}

public class Class
{
    public string Property { get; set; }
}

public record DecisionExpression(string Expression, DispositionType Disposition);
public record DecisionTableNode(DecisionExpression[] Expressions) : Node(nameof(DecisionNode));
public record CommonNode(string Name) : Node(Name);
public abstract record Node(string Name);
public record NodeGraph(Guid Id, Node[] Nodes);
