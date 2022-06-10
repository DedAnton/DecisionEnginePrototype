namespace DotNode;

public abstract class Connection { }
public abstract class SingleConnection : Connection { }
public abstract class MultipleConnection : Connection { }

public abstract class SingleConnection<TFromNode, TFromOutput, TToNode, TToInput> : SingleConnection
    where TFromNode : Node, IOutable<Input, TFromOutput>
    where TToNode : Node, IInputable<TToInput, Output>
    where TFromOutput : Output
    where TToInput : Input
{
    public SingleConnection(Func<TFromOutput, TToInput> connectFunction)
    {
        ConnectFunction = connectFunction;
    }

    public Func<TFromOutput, TToInput> ConnectFunction { get; private set; }
}

public abstract class MultipleConnection<TFromNode, TFromOutput, TToNode, TToInput> : MultipleConnection
    where TFromNode : Node, IOutable<Input, TFromOutput>
    where TToNode : Node, IInputable<TToInput, Output>
    where TFromOutput : Output
    where TToInput : Input, new()
{
    protected MultipleConnection(Func<TFromOutput, TToInput, TToInput> connectFunction)
    {
        ConnectFunction = connectFunction;
    }

    public Func<TFromOutput, TToInput, TToInput> ConnectFunction { get; private set; }
}
