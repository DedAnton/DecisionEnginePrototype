using DotNode;

namespace Demo.Nodes;


public class DispositionOutputNode : OutputNode<UserOutput>
{
    public override UserOutput Run(UserOutput output) => output;
}

public record UserOutput(DispositionType Disposition) : Input;