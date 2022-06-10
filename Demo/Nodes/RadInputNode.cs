using DotNode;

namespace Demo.Nodes;

public class RadInputNode : InputNode<UserInput>
{
    public override UserInput Run(UserInput input) => input;
}

public record UserInput(int Identifier, string AddressFrom, string AddressTo) : Output;
