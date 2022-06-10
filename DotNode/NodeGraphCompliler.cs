using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNode;

public class NodeGraphCompliler
{
    public Func<TInput, TOutput> Compile<TInput, TOutput>(NodeGraph<TInput, TOutput> graph, List<Connection> connections)
        where TInput : Output
        where TOutput : Input
    {
        var expressions = new List<Expression>();
        var variables = new List<ParameterExpression>();
        var nodeResultVars = new Dictionary<Type, ParameterExpression>();

        var inputNode = graph.Nodes.First();
        var input = Expression.Parameter(inputNode.GetType().BaseType.GenericTypeArguments[0], "input");
        CompileNode(inputNode, input, ref nodeResultVars, ref expressions, ref variables, out var inputNodeResultVar);

        var lastNodeType = inputNode.GetType();
        var lastNodeResultVar = inputNodeResultVar;
        foreach (var node in graph.Nodes.Take(1..^1))
        {
            CompileConnections(connections, node.GetType(), ref nodeResultVars, ref expressions, ref variables, out var connectionResultVar);
            CompileNode(node, connectionResultVar, ref nodeResultVars, ref expressions, ref variables, out var nodeResultVar);
            lastNodeType = node.GetType();
            lastNodeResultVar = nodeResultVar;
        }

        var outputNode = graph.Nodes.Last();
        CompileConnections(connections, outputNode.GetType(), ref nodeResultVars, ref expressions, ref variables, out var outputConnectionResultVar);
        CompileNode(outputNode, outputConnectionResultVar, ref nodeResultVars, ref expressions, ref variables, out var outputNodeResultVar);

        LabelTarget returnTarget = Expression.Label(typeof(TOutput));
        var returnLabel = Expression.Label(returnTarget, outputNodeResultVar);
        expressions.Add(returnLabel);

        var block = Expression.Block(variables, expressions);
        var lambda = Expression.Lambda<Func<TInput, TOutput>>(block, input);
        var func = lambda.Compile();
        return func;
    }

    private void CompileNode(
        Node node, 
        ParameterExpression connectionResultVar,
        ref Dictionary<Type, ParameterExpression> nodeResultVars,
        ref List<Expression> expressions, 
        ref List<ParameterExpression> variables,
        out ParameterExpression nodeResultVar)
    {
        var nodeVar = Expression.Variable(node.GetType(), node.GetType().Name);
        var nodeVarAssing = Expression.Assign(nodeVar, Expression.Constant(node));
        var nodeRun = Expression.Call(nodeVar, node.GetType().GetMethod("Run"), connectionResultVar);

        if (node is InputNode or OutputNode)
        {
            nodeResultVar = Expression.Variable(node.GetType().BaseType.GenericTypeArguments[0], $"{node.GetType().Name}Result");
        }
        else
        {
            nodeResultVar = Expression.Variable(node.GetType().BaseType.GenericTypeArguments[1], $"{node.GetType().Name}Result");
        }
        var nodeResultVarAssign = Expression.Assign(nodeResultVar, nodeRun);

        expressions.Add(nodeVarAssing);
        expressions.Add(nodeResultVarAssign);
        variables.Add(nodeVar);
        variables.Add(nodeResultVar);

        nodeResultVars.Add(node.GetType(), nodeResultVar);
    }

    private void CompileConnections(
        List<Connection> allConnections, 
        Type toNodeType,
        ref Dictionary<Type, ParameterExpression> nodeResultVars,
        ref List<Expression> expressions, 
        ref List<ParameterExpression> variables,
        out ParameterExpression connectionResultVar)
    {
        var connections = allConnections
            .Where(x => x.GetType().BaseType.GenericTypeArguments[2] == toNodeType)
            .ToList();
        if (connections.Count() == 0)
        {
            throw new InvalidOperationException($"Connection for {toNodeType.Name} was not found");
        }

        var connectionResultType = connections[0].GetType().BaseType.GenericTypeArguments[3];
        if (connections.Count() == 1)
        {
            connectionResultVar = Expression.Variable(connectionResultType, $"{connections[0].GetType().Name}Result");
            variables.Add(connectionResultVar);
            var nodeType = connections[0].GetType().BaseType.GenericTypeArguments[0];
            if (!nodeResultVars.TryGetValue(nodeType, out var inputVar))
            {
                throw new InvalidOperationException($"Result of node {nodeType} for connection {connections[0]} was not found. Node not add to graph or order was broken");
            }
            CompileConnection(connections[0], connectionResultVar, inputVar, ref expressions, ref variables);
        }
        else
        {
            connectionResultVar = Expression.Variable(connectionResultType, $"{connections[0].GetType().BaseType.GenericTypeArguments[3].Name}Result");
            var connectionResultVarDefaultAssign = Expression.Assign(connectionResultVar, Expression.New(connectionResultType));
            expressions.Add(connectionResultVarDefaultAssign);
            variables.Add(connectionResultVar);
            foreach (var connection in connections)
            {
                var nodeType = connection.GetType().BaseType.GenericTypeArguments[0];
                if (!nodeResultVars.TryGetValue(nodeType, out var inputVar))
                {
                    throw new InvalidOperationException($"Result of node {nodeType} for connection {connection} was not found. Node not add to graph or order was broken");
                }
                CompileConnection(connection, connectionResultVar, inputVar, ref expressions, ref variables);
            }
        }
    }

    private void CompileConnection(
        Connection connection, 
        ParameterExpression connectionResultVar,
        ParameterExpression fromNodeResultVar,
        ref List<Expression> expressions, 
        ref List<ParameterExpression> variables)
    {
        var connectionVar = Expression.Variable(connection.GetType(), connection.GetType().Name);
        var connectionVarAssign = Expression.Assign(connectionVar, Expression.Constant(connection));
        var connectFunctionProperty = connection.GetType().GetProperty("ConnectFunction");
        var connectFunctionPropertyAccess = Expression.MakeMemberAccess(connectionVar, connectFunctionProperty);
        Expression connectionRun = null;
        if (connection is SingleConnection)
        {
            connectionRun = Expression.Call(connectFunctionPropertyAccess, connectFunctionProperty.PropertyType.GetMethod("Invoke"), fromNodeResultVar);
        }
        else if (connection is MultipleConnection)
        {
            connectionRun = Expression.Call(connectFunctionPropertyAccess, connectFunctionProperty.PropertyType.GetMethod("Invoke"), fromNodeResultVar, connectionResultVar);
        }
        var connectionResultVarAssign = Expression.Assign(connectionResultVar, connectionRun);

        expressions.Add(connectionVarAssign);
        expressions.Add(connectionResultVarAssign);
        variables.Add(connectionVar);
    }
}