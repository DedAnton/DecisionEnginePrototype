namespace DotNode;

public class NodeGraph<TInput, TOutput>
    where TInput : Output
    where TOutput : Input
{
    internal NodeGraph(List<Node> nodes)
    {
        Nodes = nodes;
    }

    internal List<Node> Nodes { get; private set; } = new();
}

public class NodeGraphBuilder
{
    public NodeGraphBuilder<TInput> AddInputNode<TInput>(InputNode<TInput> inputNode) where TInput : Output
    {
        return new NodeGraphBuilder<TInput>(inputNode);
    }
}

public class NodeGraphBuilder<TInput> where TInput : Output
{
    private List<Node> nodes = new List<Node>();

    internal NodeGraphBuilder(Node inputNode)
    {
        nodes.Add(inputNode);
    }

    public NodeGraphBuilder<TInput> AddNode(Node node)
    {
        nodes.Add(node);
        return this;
    }

    public NodeGraphBuilder<TInput, TOutput> AddOutputNode<TOutput>(OutputNode<TOutput> outputNode) where TOutput : Input
    {
        nodes.Add(outputNode);
        return new NodeGraphBuilder<TInput, TOutput>(nodes);
    }
}

public class NodeGraphBuilder<TInput, TOutput>
    where TInput : Output
    where TOutput : Input
{
    private readonly List<Node> _nodes;

    internal NodeGraphBuilder(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public NodeGraph<TInput, TOutput> Build()
    {
        return new NodeGraph<TInput, TOutput>(_nodes);
    }
}