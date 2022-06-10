namespace DotNode;

public abstract class Node { }
public abstract class InputNode : Node { }
public abstract class OutputNode : Node { }

public abstract class Node<TInput, TOutput> : Node, IInputable<TInput, Output>, IOutable<Input, TOutput>
    where TInput : Input
    where TOutput : Output
{
    public abstract TOutput Run(TInput input);
}

public interface IInputable<TInput, TOutput> where TOutput : Output { }
public interface IOutable<TInput, TOutput> where TInput : Input{ }

public abstract record Input;
public abstract record Output;
public sealed record EmptyInput : Input;
public sealed record EmptyOutput : Output;

public abstract class InputNode<TInput> : InputNode, IOutable<Input, TInput> where TInput : Output
{
    public abstract TInput Run(TInput input);
}

public abstract class OutputNode<TOutput> : OutputNode, IInputable<TOutput, Output> where TOutput : Input
{
    public abstract TOutput Run(TOutput output);
}