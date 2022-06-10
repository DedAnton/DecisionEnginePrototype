using DotNode;

namespace Demo.Nodes;

public class PolicyNode : Node<PolicyId, Policy>
{
    public override Policy Run(PolicyId input)
    {
        var (id, sos) = GetPolicyData(input.Id);
        var policy = new Policy(id, sos);

        return policy;
    }

    private (int Id, bool Sos) GetPolicyData(int id)
    {
        return id switch
        {
            0 => (0, false),
            1 => (1, true),
            2 => (2, true),
            _ => (999, false)
        };
    }
}

public record PolicyId(int Id) : Input;
public record Policy(int Price, bool Sos) : Output;