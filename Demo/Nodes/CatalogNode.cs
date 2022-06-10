using DotNode;

namespace Demo.Nodes;

public class CatalogNode : Node<ProductIdentifier, Product>
{
    public override Product Run(ProductIdentifier input)
    {
        var (price, policyId) = GetProductPrice(input.Identifier);
        var product = new Product(price, policyId);

        return product;
    }

    private (double Price, int PolicyId) GetProductPrice(int id)
    {
        return id switch
        {
            > 0 and <= 10 => (5, 1),
            > 10 and <= 100 => (60, 2),
            > 100 => (99, 3),
            _ => (1, 100)
        };
    }
}

public record ProductIdentifier(int Identifier) : Input;
public record Product(double Price, int PolicyId) : Output;